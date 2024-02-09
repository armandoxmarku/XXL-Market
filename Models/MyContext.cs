#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace XXL_Market.Models;

public class MyContext : DbContext 
{     
    public MyContext(DbContextOptions options) : base(options) { }   
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Orderi> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
}