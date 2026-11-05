
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database tables
        public DbSet<College> Colleges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Application User configuration
            builder.Entity<ApplicationUser>()
                .Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();

            // College configuration
            builder.Entity<College>()
                .HasIndex(c => c.CollegeCode)
                .IsUnique();
        }
    }
}
