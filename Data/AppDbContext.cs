using Microsoft.EntityFrameworkCore;
using CICertSOAR.Models;

namespace CICertSOAR.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Sector> Sectors { get; set; } = null!;
        public DbSet<Ministry> Ministries { get; set; } = null!;
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<Vulnerability> Vulnerabilities { get; set; } = null!;
        public DbSet<Incident> Incidents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sector -> Ministries (1:N)
            modelBuilder.Entity<Ministry>()
                .HasOne(m => m.Sector)
                .WithMany(s => s.Ministries)
                .HasForeignKey(m => m.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ministry -> Organizations (1:N)
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.Ministry)
                .WithMany(m => m.Organizations)
                .HasForeignKey(o => o.MinistryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Assets (1:N)
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Organization)
                .WithMany(o => o.Assets)
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asset -> Incidents (1:N)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Asset)
                .WithMany(a => a.Incidents)
                .HasForeignKey(i => i.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vulnerability -> Incidents (1:N)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Vulnerability)
                .WithMany(v => v.Incidents)
                .HasForeignKey(i => i.VulnerabilityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
