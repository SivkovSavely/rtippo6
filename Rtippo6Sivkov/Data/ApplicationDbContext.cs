using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rtippo6Sivkov.Models;

namespace Rtippo6Sivkov.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<OrganizationType> OrganizationsTypes { get; set; } = null!;
    public DbSet<Locality> Localities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<Organization>()
            .HasKey(x => x.Id);

        builder
            .Entity<Organization>()
            .HasOne(x => x.Type)
            .WithMany(x => x.OrganizationsWithThisType);

        builder
            .Entity<Organization>()
            .HasOne(x => x.Locality)
            .WithMany(x => x.OrganizationsWithThisLocality);

        builder
            .Entity<Locality>()
            .HasOne(x => x.Administration)
            .WithOne(x => x.Administering)
            .HasForeignKey<Locality>("Administration_Organization_Id").IsRequired(false);

        builder
            .Entity<AppUser>()
            .HasOne(x => x.Role)
            .WithMany(x => x.UsersWithRole);
    }
}