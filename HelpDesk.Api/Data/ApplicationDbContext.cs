using Microsoft.EntityFrameworkCore;
using HelpDeskAPI.Models;
using System;

namespace HelpDeskAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Relationships
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedAdmin)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedTo)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed Users с фиксированными хешами паролей
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@helpdesk.com",
                // фиксированный хеш для "admin123"
                PasswordHash = "$2a$11$k8P0Fz3y7sHq0E9o6lQHhOEr0TxX1t0L9xjiy6d4fH7uU8XQ0a9mW",
                Role = "Admin"
            },
            new User
            {
                Id = 2,
                Name = "John Doe",
                Email = "user@helpdesk.com",
                // фиксированный хеш для "user123"
                PasswordHash = "$2a$11$yR8jYDPxz9iRxFlIblpG2OktqF7qPzQnPh1zv4KjDY3dFZ0kuUWhq",
                Role = "User"
            }
        );

        // Seed Tickets
        modelBuilder.Entity<Ticket>().HasData(
            new Ticket
            {
                Id = 1,
                Title = "Cannot login to system",
                Description = "I'm getting error 500 when trying to login",
                Status = "Open",
                CreatedBy = 2,
                CreatedDate = new DateTime(2026, 1, 28)
            },
            new Ticket
            {
                Id = 2,
                Title = "Password reset not working",
                Description = "Password reset email is not arriving",
                Status = "InProgress",
                CreatedBy = 2,
                AssignedTo = 1,
                CreatedDate = new DateTime(2026, 1, 29)
            }
        );
    }
}
