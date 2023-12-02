using BlackSwanPlat.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlackSwanPlat.Data;

public class DbBlack : IdentityDbContext<ApplicationUser>
{
    public DbSet<Product> products { get; set; }
    public DbSet<City> cities { get; set; }
    public DbSet<Brand> brands { get; set; }
    public DbSet<Order> orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<ProductCategory> productCategories { get; set; }
    public DbSet<ProductImages> productImages { get; set; }
    public DbSet<CardItem> cardItems { get; set; }
    public DbSet<dis> dis { get; set; }
    public DbSet<Inventoryval> inventoryval { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> likes { get; set; }
    public DbSet<DailySale> dailySales { get; set; }
    
    public DbBlack(DbContextOptions<DbBlack> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

   
}
