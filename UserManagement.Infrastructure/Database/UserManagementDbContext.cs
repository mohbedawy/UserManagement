using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Database
{
    public class UserManagementDbContext : DbContext
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity mappings here if needed
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity(); // for recording message status 
            modelBuilder.AddInboxStateEntity(); // 
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }

    }
}
