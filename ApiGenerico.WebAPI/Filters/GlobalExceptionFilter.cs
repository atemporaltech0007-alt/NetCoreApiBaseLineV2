using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApiGenerico.WebAPI.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        if (context.Exception is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            message = context.Exception.Message;
        }
        else if (context.Exception is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = context.Exception.Message;
        }
        else if (context.Exception is DbUpdateConcurrencyException)
        {
            statusCode = HttpStatusCode.Conflict;
            message = context.Exception.Message;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "Unauthorized access.";
        }

        _logger.LogError(context.Exception, "Exception occurred: {Message}", context.Exception.Message);

        context.Result = new ObjectResult(new
        {
            error = message,
            details = context.Exception.Message
        })
        {
            StatusCode = (int)statusCode
        };

        context.ExceptionHandled = true;
    }
}
