using Microsoft.EntityFrameworkCore;
using SDTicaret.Core;
using SDTicaret.Core.Entities;

namespace SDTicaret.Infrastructure.Data;

public class SDTicaretDbContext : DbContext
{
    public SDTicaretDbContext(DbContextOptions<SDTicaretDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<StockNotification> StockNotifications { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Entity konfigürasyonları burada olacak
    }
} 
