using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<Winner> Winners => Set<Winner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Phone).IsRequired().HasMaxLength(20);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasMany(u => u.Auctions)
                  .WithOne(a => a.User)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Bids)
                  .WithOne(b => b.User)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Winners)
                  .WithOne(w => w.User)
                  .HasForeignKey(w => w.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(2000);
            entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Images).HasMaxLength(1000);

            entity.HasOne(p => p.Auction)
                  .WithOne(a => a.Product)
                  .HasForeignKey<Auction>(a => a.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Auction configuration
        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.StartPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(a => a.CurrentPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(a => a.Status).IsRequired().HasConversion<int>();

            entity.HasOne(a => a.Winner)
                  .WithOne(w => w.Auction)
                  .HasForeignKey<Winner>(w => w.AuctionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(a => a.Bids)
                  .WithOne(b => b.Auction)
                  .HasForeignKey(b => b.AuctionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Bid configuration
        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.HasIndex(b => new { b.AuctionId, b.UserId });
        });

        // Winner configuration
        modelBuilder.Entity<Winner>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.FinalPrice).IsRequired().HasColumnType("decimal(18,2)");
        });
    }
}

