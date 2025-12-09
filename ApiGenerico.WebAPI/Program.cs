using ApiGenerico.Application.Handlers;
using ApiGenerico.Application.Services;
using ApiGenerico.Application.Services.Decorators;
using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Events;
using ApiGenerico.Domain.Models;
using ApiGenerico.Infrastructure.Context;
using ApiGenerico.Infrastructure.Repositories;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using ApiGenerico.Utils.Security;
using ApiGenerico.WebAPI.Filters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanagement-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
IEncryptionService Encrypt = new EncryptionService();
IConfigurationSection seccionConfiguracion = builder.Configuration.GetSection("SectionConfiguration");
IConfigurationSection seccionConnectionStrings = builder.Configuration.GetSection("ConnectionStrings");

builder.Services.Configure<SectionConfiguration>(seccionConfiguracion);
builder.Services.Configure<ConnectionStrings>(seccionConnectionStrings);
var configuracionAppSettings = seccionConfiguracion.Get<SectionConfiguration>();
var configuracionConnectionStrings = seccionConnectionStrings.Get<ConnectionStrings>();

string DecryptConnectionString(string encryptedConnectionString)
{
    return string.IsNullOrEmpty(encryptedConnectionString) ? null : Encrypt.Decrypt(encryptedConnectionString);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionToken").Exists())
{
    string GetConnetionToken = DecryptConnectionString(configuracionConnectionStrings.ConnetionToken);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionGenerico").Exists())
{
    string ConnetionGenerico = DecryptConnectionString(configuracionConnectionStrings.ConnetionGenerico);
    if (!string.IsNullOrEmpty(ConnetionGenerico))
    {
        builder.Services.AddDbContext<ContextSql>(opt => opt.UseSqlServer(ConnetionGenerico));
    }
}

builder.Services.AddScoped<IEncryptionService, EncryptionService>();

#region Repositories Registration
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
#endregion

#region Services Registration with Caching
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<IStateService>(provider =>
{
    var innerService = provider.GetRequiredService<StateService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachedStateService(innerService, cache);
});

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddMemoryCache();
#endregion

#region MediatR Registration
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(TaskStateChangedEvent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(TaskStateChangedEventHandler).Assembly);
});
#endregion

#region Registro dinámico de servicios (Dynamic Services Injection)
var generalServices = typeof(_Service).Assembly.GetTypes()
    .Where(type => !type.Name.StartsWith("_") && type.Name.EndsWith("Service"))
    .ToList();

var serviceInterfaces = generalServices.Where(type => type.IsInterface);
var serviceImplementations = generalServices.Where(type => type.IsClass);

foreach (var implementation in serviceImplementations)
{
    var interfaceName = $"I{implementation.Name}";
    var serviceInterface = serviceInterfaces.FirstOrDefault(i => i.Name == interfaceName);
    if (serviceInterface != null)
    {
        builder.Services.AddScoped(serviceInterface, implementation);
    }
}
#endregion

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "API Generico", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter JWT with bearer format like 'Bearer [Token]'"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    opt.CustomSchemaIds(type => type.FullName);
    opt.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.GroupName == null || !apiDesc.GroupName.Equals("Hidden", StringComparison.OrdinalIgnoreCase);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPolicySecureDomains", policy =>
    {
        policy.WithOrigins(configuracionAppSettings.SecureDomains)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Política permisiva para desarrollo local
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Usar CORS permisivo en desarrollo, restrictivo en producción
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}
else
{
    app.UseCors("AllowPolicySecureDomains");
}

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API Generico V1");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();