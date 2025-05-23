using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using XakUjin2025;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<InvalidToken> InvalidTokens { get; set; }
    public DbSet<Home> Homes { get; set; }
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<Signal> Signals { get; set; }
    public DbSet<Indication> Indications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apartment>()
            .HasOne(a => a.Home)
            .WithMany(h => h.Apartments)
            .HasForeignKey(a => a.HomeId);

        modelBuilder.Entity<Apartment>()
            .HasOne(a => a.ApplicationUser)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .IsRequired(false);

        modelBuilder.Entity<Signal>()
            .HasOne(a => a.Apartment)
            .WithMany(h => h.Signals)
            .HasForeignKey(a => a.ApartmentId);

        modelBuilder.Entity<Indication>()
            .HasOne(a => a.Signal)
            .WithMany(h => h.Indications)
            .HasForeignKey(a => a.SignalId);

        modelBuilder.Entity<Signal>()
           .Property(s => s.SignalId)
           .ValueGeneratedOnAdd();
    }

}