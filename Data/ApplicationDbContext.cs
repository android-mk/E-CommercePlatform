using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommercePlatform.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configure the Name column in AspNetUserTokens
    modelBuilder.Entity<IdentityUserToken<string>>(entity =>
    {
        entity.Property(e => e.Name).HasMaxLength(128); // Set max length to 128
    });

    // Configure the Product entity
    modelBuilder.Entity<Product>(entity =>
    {
        entity.Property(p => p.Price).HasColumnType("decimal(18,2)"); // Set precision and scale
    });

    // Configure the CartItem entity
    modelBuilder.Entity<CartItem>(entity =>
    {
        entity.HasKey(c => c.Id); // Primary key
        entity.HasOne(c => c.User) // Relationship with AspNetUsers
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete

        entity.HasOne(c => c.Product) // Relationship with Products
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    });

    // Configure the Order entity
    modelBuilder.Entity<Order>(entity =>
    {
        entity.HasKey(o => o.Id); // Primary key
        entity.HasOne(o => o.User) // Relationship with AspNetUsers
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    });

    // Configure the OrderItem entity
    modelBuilder.Entity<OrderItem>(entity =>
    {
        entity.HasKey(oi => oi.Id); // Primary key
        entity.HasOne(oi => oi.Order) // Relationship with Orders
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete

        entity.HasOne(oi => oi.Product) // Relationship with Products
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    });
}
    }
}