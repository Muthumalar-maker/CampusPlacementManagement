using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CampusPlacementManagement.Repositories;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services;
using CampusPlacementManagement.Services.Interfaces;


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

        public DbSet<Company> Companies { get; set; }

        public DbSet<CompanyHR> CompanyHRs { get; set; }

        public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            //builder.Services.AddScoped<ICompanyService, CompanyService>();


            // Application User configuration
            builder.Entity<ApplicationUser>()
                .Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();

            // College configuration
            builder.Entity<College>()
                .HasIndex(c => c.CollegeCode)
                .IsUnique();

            // Placement Officer - User relationship
            builder.Entity<PlacementOfficer>()
                .HasOne(po => po.User)
                .WithOne()
                .HasForeignKey<PlacementOfficer>(po => po.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Placement Officer - College relationship
            builder.Entity<PlacementOfficer>()
                .HasOne(po => po.College)
                .WithMany()
                .HasForeignKey(po => po.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Identity user can have only one Placement Officer profile
            builder.Entity<PlacementOfficer>()
                .HasIndex(po => po.UserId)
                .IsUnique();

            builder.Entity<CompanyHR>()
    .HasOne(hr => hr.User)
    .WithMany()
    .HasForeignKey(hr => hr.UserId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CompanyHR>()
                .HasOne(hr => hr.Company)
                .WithMany()
                .HasForeignKey(hr => hr.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
