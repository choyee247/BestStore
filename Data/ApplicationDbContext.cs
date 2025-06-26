using Example2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Example2.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BrandandCatView> BrandandCatViews { get; set; }
    public DbSet<DashboardViewModel> DashboardViewModels { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Brand>().HasData(
            new Brand { Id = 1, Name = "Dell" },
            new Brand { Id = 2, Name = "HP" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Computers"},
            new Category { Id = 2, Name = "Phones"}
            //new Category { Id = 3, Name = "Accessories", BrandId = 1 }
        );
        modelBuilder.Entity<BrandandCatView>()
        .HasKey(bc => new { bc.BrandId, bc.CategoryId });

        modelBuilder.Entity<BrandandCatView>()
            .HasOne(bc => bc.Brand)
            .WithMany(b => b.BrandandCatViews)
            .HasForeignKey(bc => bc.BrandId);

        //modelBuilder.Entity<BrandandCatView>()
        //    .HasOne(bc => bc.Category)
        //    .WithMany(c => c.BrandandCatViews)
        //    .HasForeignKey(bc => bc.CategoryId);
        modelBuilder.Entity<Review>()
        .HasOne(r => r.Product)
        .WithMany(p => p.Reviews)
        .HasForeignKey(r => r.ProductId); // ✅ Use proper FK

        //modelBuilder.Entity<Review>()
        //.HasOne(r => r.Product)
        //.WithMany(p => p.Reviews)
        //.HasForeignKey(r => r.Id);

        //modelBuilder.Entity<Product>()
        //    .HasMany(p => p.Reviews)
        //    .WithOne(r => r.Product)
        //    .HasForeignKey(r => r.ProductId);

        modelBuilder.Entity<Order>()
        .Property(o => o.Status)
        .HasConversion<string>();
    }

}
