using ApiGenerico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiGenerico.Infrastructure.Context
{
    public class ContextSql : DbContext
    {
        private readonly IConfiguration Config;

        public ContextSql(DbContextOptions<ContextSql> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        public DbSet<State> States { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.ToTable("Task");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(4000);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(e => e.State)
                    .WithMany(s => s.Tasks)
                    .HasForeignKey(e => e.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.StateId);
                entity.HasIndex(e => e.DueDate);
            });

            modelBuilder.Entity<TaskHistory>(entity =>
            {
                entity.ToTable("TaskHistory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChangedAt).IsRequired();

                entity.HasOne(e => e.Task)
                    .WithMany(t => t.TaskHistories)
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PreviousState)
                    .WithMany()
                    .HasForeignKey(e => e.PreviousStateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NewState)
                    .WithMany()
                    .HasForeignKey(e => e.NewStateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.TaskId);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>().HasData(
                new State { Id = 1, Name = "Pendiente", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new State { Id = 2, Name = "En Progreso", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new State { Id = 3, Name = "Completado", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
