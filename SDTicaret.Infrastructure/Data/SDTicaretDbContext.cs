using Microsoft.EntityFrameworkCore;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using System.Security.Cryptography;
using System.Text;

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
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
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
        
        // Seed Data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Örnek kullanıcılar
        var adminUser = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@sdticaret.com",
            PasswordHash = HashPassword("admin123"),
            FirstName = "Admin",
            LastName = "User",
            Role = "Admin",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        
        var employeeUser = new User
        {
            Id = 2,
            Username = "employee",
            Email = "employee@sdticaret.com",
            PasswordHash = HashPassword("employee123"),
            FirstName = "Çalışan",
            LastName = "Kullanıcı",
            Role = "Employee",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        
        var customerUser = new User
        {
            Id = 3,
            Username = "customer",
            Email = "customer@sdticaret.com",
            PasswordHash = HashPassword("customer123"),
            FirstName = "Müşteri",
            LastName = "Kullanıcı",
            Role = "Customer",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        
        modelBuilder.Entity<User>().HasData(adminUser, employeeUser, customerUser);

        // Örnek kategoriler
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Elektronik", Description = "Elektronik ürünler", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, Name = "Giyim", Description = "Giyim ürünleri", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, Name = "Ev & Yaşam", Description = "Ev ve yaşam ürünleri", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 4, Name = "Spor", Description = "Spor ürünleri", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 5, Name = "Kitap", Description = "Kitap ve yayınlar", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Category>().HasData(categories);

        // Örnek tedarikçiler
        var suppliers = new List<Supplier>
        {
            new Supplier { Id = 1, Name = "TechCorp", ContactPerson = "Ahmet Yılmaz", Email = "info@techcorp.com", Phone = "0212 555 0101", Address = "İstanbul, Türkiye", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Supplier { Id = 2, Name = "FashionLine", ContactPerson = "Ayşe Demir", Email = "contact@fashionline.com", Phone = "0216 555 0202", Address = "İstanbul, Türkiye", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Supplier { Id = 3, Name = "HomeStyle", ContactPerson = "Mehmet Kaya", Email = "sales@homestyle.com", Phone = "0232 555 0303", Address = "İzmir, Türkiye", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Supplier { Id = 4, Name = "SportMax", ContactPerson = "Fatma Özkan", Email = "info@sportmax.com", Phone = "0312 555 0404", Address = "Ankara, Türkiye", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Supplier { Id = 5, Name = "BookWorld", ContactPerson = "Ali Çelik", Email = "orders@bookworld.com", Phone = "0224 555 0505", Address = "Bursa, Türkiye", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Supplier>().HasData(suppliers);

        // Örnek şubeler
        var branches = new List<Branch>
        {
            new Branch { Id = 1, Name = "Merkez Şube", Address = "Kadıköy, İstanbul", Phone = "0216 555 1001", Manager = "Hasan Yıldız", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Branch { Id = 2, Name = "Ankara Şube", Address = "Çankaya, Ankara", Phone = "0312 555 1002", Manager = "Zeynep Arslan", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Branch { Id = 3, Name = "İzmir Şube", Address = "Konak, İzmir", Phone = "0232 555 1003", Manager = "Murat Özkan", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Branch { Id = 4, Name = "Bursa Şube", Address = "Nilüfer, Bursa", Phone = "0224 555 1004", Manager = "Elif Demir", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Branch>().HasData(branches);

        // Örnek çalışanlar
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "Hasan", LastName = "Yıldız", Email = "hasan.yildiz@sdticaret.com", Phone = "0532 555 2001", Position = "Şube Müdürü", BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 2, FirstName = "Zeynep", LastName = "Arslan", Email = "zeynep.arslan@sdticaret.com", Phone = "0533 555 2002", Position = "Şube Müdürü", BranchId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 3, FirstName = "Murat", LastName = "Özkan", Email = "murat.ozkan@sdticaret.com", Phone = "0534 555 2003", Position = "Şube Müdürü", BranchId = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 4, FirstName = "Elif", LastName = "Demir", Email = "elif.demir@sdticaret.com", Phone = "0535 555 2004", Position = "Şube Müdürü", BranchId = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 5, FirstName = "Can", LastName = "Kaya", Email = "can.kaya@sdticaret.com", Phone = "0536 555 2005", Position = "Satış Temsilcisi", BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 6, FirstName = "Selin", LastName = "Çelik", Email = "selin.celik@sdticaret.com", Phone = "0537 555 2006", Position = "Satış Temsilcisi", BranchId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Employee>().HasData(employees);

        // Örnek müşteriler
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet.yilmaz@email.com", Phone = "0538 555 3001", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 2, FirstName = "Ayşe", LastName = "Demir", Email = "ayse.demir@email.com", Phone = "0539 555 3002", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 3, FirstName = "Mehmet", LastName = "Kaya", Email = "mehmet.kaya@email.com", Phone = "0540 555 3003", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 4, FirstName = "Fatma", LastName = "Özkan", Email = "fatma.ozkan@email.com", Phone = "0541 555 3004", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 5, FirstName = "Ali", LastName = "Çelik", Email = "ali.celik@email.com", Phone = "0542 555 3005", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 6, FirstName = "Zeynep", LastName = "Arslan", Email = "zeynep.arslan@email.com", Phone = "0543 555 3006", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 7, FirstName = "Murat", LastName = "Yıldız", Email = "murat.yildiz@email.com", Phone = "0544 555 3007", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 8, FirstName = "Elif", LastName = "Kaya", Email = "elif.kaya@email.com", Phone = "0545 555 3008", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Customer>().HasData(customers);

        // Örnek ürünler
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 15 Pro", Description = "Apple iPhone 15 Pro 128GB", Price = 45000, Stock = 50, CategoryId = 1, SupplierId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "Samsung Galaxy S24", Description = "Samsung Galaxy S24 256GB", Price = 35000, Stock = 45, CategoryId = 1, SupplierId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, Name = "MacBook Air M2", Description = "Apple MacBook Air M2 13 inch", Price = 55000, Stock = 25, CategoryId = 1, SupplierId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, Name = "Kadın Bluz", Description = "Yazlık kadın bluz", Price = 150, Stock = 100, CategoryId = 2, SupplierId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 5, Name = "Erkek Gömlek", Description = "Klasik erkek gömlek", Price = 200, Stock = 80, CategoryId = 2, SupplierId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 6, Name = "Kadın Elbise", Description = "Şık kadın elbise", Price = 350, Stock = 60, CategoryId = 2, SupplierId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 7, Name = "Kahve Makinesi", Description = "Otomatik kahve makinesi", Price = 1200, Stock = 30, CategoryId = 3, SupplierId = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 8, Name = "Blender Seti", Description = "Profesyonel blender seti", Price = 800, Stock = 40, CategoryId = 3, SupplierId = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 9, Name = "Koşu Ayakkabısı", Description = "Nike koşu ayakkabısı", Price = 1200, Stock = 35, CategoryId = 4, SupplierId = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 10, Name = "Fitness Topu", Description = "Pilates fitness topu", Price = 150, Stock = 70, CategoryId = 4, SupplierId = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 11, Name = "Harry Potter Seti", Description = "Harry Potter 7 kitap seti", Price = 450, Stock = 25, CategoryId = 5, SupplierId = 5, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 12, Name = "Bilim Kurgu Romanı", Description = "Popüler bilim kurgu romanı", Price = 75, Stock = 50, CategoryId = 5, SupplierId = 5, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Product>().HasData(products);

        // Örnek stoklar
        var stocks = new List<Stock>
        {
            new Stock { Id = 1, ProductId = 1, Quantity = 50, MinimumStock = 10, MaximumStock = 100, BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 2, ProductId = 2, Quantity = 45, MinimumStock = 10, MaximumStock = 100, BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 3, ProductId = 3, Quantity = 25, MinimumStock = 5, MaximumStock = 50, BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 4, ProductId = 4, Quantity = 100, MinimumStock = 20, MaximumStock = 200, BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 5, ProductId = 5, Quantity = 80, MinimumStock = 15, MaximumStock = 150, BranchId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 6, ProductId = 1, Quantity = 30, MinimumStock = 10, MaximumStock = 80, BranchId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 7, ProductId = 2, Quantity = 25, MinimumStock = 10, MaximumStock = 80, BranchId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Stock { Id = 8, ProductId = 4, Quantity = 60, MinimumStock = 20, MaximumStock = 150, BranchId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Stock>().HasData(stocks);

        // Örnek siparişler
        var orders = new List<Order>
        {
            new Order { Id = 1, CustomerId = 1, AssignedEmployeeId = 5, OrderDate = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc), TotalAmount = 45000, OrderStatus = "Delivered", CreatedAt = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc) },
            new Order { Id = 2, CustomerId = 2, AssignedEmployeeId = 6, OrderDate = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc), TotalAmount = 350, OrderStatus = "Processing", CreatedAt = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc) },
            new Order { Id = 3, CustomerId = 3, AssignedEmployeeId = 5, OrderDate = new DateTime(2024, 1, 29, 16, 45, 0, DateTimeKind.Utc), TotalAmount = 1200, OrderStatus = "Shipped", CreatedAt = new DateTime(2024, 1, 29, 16, 45, 0, DateTimeKind.Utc) },
            new Order { Id = 4, CustomerId = 4, AssignedEmployeeId = 6, OrderDate = new DateTime(2024, 1, 30, 9, 15, 0, DateTimeKind.Utc), TotalAmount = 800, OrderStatus = "Pending", CreatedAt = new DateTime(2024, 1, 30, 9, 15, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Order>().HasData(orders);

        // Örnek sipariş kalemleri
        var orderItems = new List<OrderItem>
        {
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 45000, CreatedAt = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc) },
            new OrderItem { Id = 2, OrderId = 2, ProductId = 4, Quantity = 1, UnitPrice = 150, CreatedAt = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc) },
            new OrderItem { Id = 3, OrderId = 2, ProductId = 5, Quantity = 1, UnitPrice = 200, CreatedAt = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc) },
            new OrderItem { Id = 4, OrderId = 3, ProductId = 9, Quantity = 1, UnitPrice = 1200, CreatedAt = new DateTime(2024, 1, 29, 16, 45, 0, DateTimeKind.Utc) },
            new OrderItem { Id = 5, OrderId = 4, ProductId = 8, Quantity = 1, UnitPrice = 800, CreatedAt = new DateTime(2024, 1, 30, 9, 15, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<OrderItem>().HasData(orderItems);

        // Örnek ödemeler
        var payments = new List<Payment>
        {
            new Payment { Id = 1, OrderId = 1, Amount = 45000, PaymentDate = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc), PaymentType = "CreditCard", PaymentMethod = "Kredi Kartı", Status = "Completed", CreatedAt = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc) },
            new Payment { Id = 2, OrderId = 2, Amount = 350, PaymentDate = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc), PaymentType = "Cash", PaymentMethod = "Nakit", Status = "Completed", CreatedAt = new DateTime(2024, 1, 27, 14, 30, 0, DateTimeKind.Utc) },
            new Payment { Id = 3, OrderId = 3, Amount = 1200, PaymentDate = new DateTime(2024, 1, 29, 16, 45, 0, DateTimeKind.Utc), PaymentType = "BankTransfer", PaymentMethod = "Havale", Status = "Completed", CreatedAt = new DateTime(2024, 1, 29, 16, 45, 0, DateTimeKind.Utc) },
            new Payment { Id = 4, OrderId = 4, Amount = 800, PaymentDate = new DateTime(2024, 1, 30, 9, 15, 0, DateTimeKind.Utc), PaymentType = "CreditCard", PaymentMethod = "Kredi Kartı", Status = "Pending", CreatedAt = new DateTime(2024, 1, 30, 9, 15, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Payment>().HasData(payments);

        // Örnek kampanyalar
        var campaigns = new List<Campaign>
        {
            new Campaign { Id = 1, Name = "Yaz İndirimi", Description = "Tüm yaz ürünlerinde %20 indirim", StartDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 2, 19, 23, 59, 59, DateTimeKind.Utc), DiscountRate = 20, IsActive = true, CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Campaign { Id = 2, Name = "Elektronik Fırsatları", Description = "Elektronik ürünlerde %15 indirim", StartDate = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 2, 14, 23, 59, 59, DateTimeKind.Utc), DiscountRate = 15, IsActive = true, CreatedAt = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Campaign { Id = 3, Name = "Kitap Haftası", Description = "Tüm kitaplarda %25 indirim", StartDate = new DateTime(2024, 1, 28, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 2, 7, 23, 59, 59, DateTimeKind.Utc), DiscountRate = 25, IsActive = true, CreatedAt = new DateTime(2024, 1, 28, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Campaign>().HasData(campaigns);

        // Örnek sözleşmeler
        var contracts = new List<Contract>
        {
            new Contract { Id = 1, Title = "TechCorp Tedarik Sözleşmesi", CustomerId = 1, SupplierId = 1, ContractType = "Tedarik", StartDate = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 7, 1, 23, 59, 59, DateTimeKind.Utc), TotalValue = 500000, Status = "Active", CreatedAt = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Contract { Id = 2, Title = "FashionLine Tedarik Sözleşmesi", CustomerId = 2, SupplierId = 2, ContractType = "Tedarik", StartDate = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 9, 1, 23, 59, 59, DateTimeKind.Utc), TotalValue = 300000, Status = "Active", CreatedAt = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Contract { Id = 3, Title = "HomeStyle Tedarik Sözleşmesi", CustomerId = 3, SupplierId = 3, ContractType = "Tedarik", StartDate = new DateTime(2023, 11, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2024, 11, 1, 23, 59, 59, DateTimeKind.Utc), TotalValue = 200000, Status = "Active", CreatedAt = new DateTime(2023, 11, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Contract>().HasData(contracts);

        // Örnek şikayetler
        var complaints = new List<Complaint>
        {
            new Complaint { Id = 1, CustomerId = 1, OrderId = 1, Subject = "Ürün hasarlı geldi", Description = "Sipariş ettiğim ürün hasarlı bir şekilde geldi", Status = "Open", CreatedDate = new DateTime(2024, 1, 28, 12, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 28, 12, 0, 0, DateTimeKind.Utc) },
            new Complaint { Id = 2, CustomerId = 2, OrderId = 2, Subject = "Kargo gecikmesi", Description = "Siparişim belirtilen tarihte gelmedi", Status = "Resolved", CreatedDate = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc), ResolvedDate = new DateTime(2024, 1, 29, 16, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 25, 10, 0, 0, DateTimeKind.Utc) },
            new Complaint { Id = 3, CustomerId = 3, OrderId = 3, Subject = "Yanlış ürün", Description = "Farklı bir ürün gönderildi", Status = "InProgress", CreatedDate = new DateTime(2024, 1, 29, 14, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 29, 14, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Complaint>().HasData(complaints);

        // Örnek anketler
        var surveys = new List<Survey>
        {
            new Survey { Id = 1, Title = "Müşteri Memnuniyeti", Description = "Hizmet kalitemizi değerlendirin", CustomerId = 1, OrderId = 1, Rating = 5, Comment = "Çok memnun kaldım", SurveyDate = new DateTime(2024, 1, 29, 15, 0, 0, DateTimeKind.Utc), CreatedDate = new DateTime(2024, 1, 29, 15, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 29, 15, 0, 0, DateTimeKind.Utc) },
            new Survey { Id = 2, Title = "Ürün Kalitesi", Description = "Ürün kalitemizi değerlendirin", CustomerId = 2, OrderId = 2, Rating = 4, Comment = "Kaliteli ürün", SurveyDate = new DateTime(2024, 1, 28, 14, 0, 0, DateTimeKind.Utc), CreatedDate = new DateTime(2024, 1, 28, 14, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 28, 14, 0, 0, DateTimeKind.Utc) },
            new Survey { Id = 3, Title = "Kargo Hizmeti", Description = "Kargo hizmetimizi değerlendirin", CustomerId = 3, OrderId = 3, Rating = 3, Comment = "Orta seviyede", SurveyDate = new DateTime(2024, 1, 27, 13, 0, 0, DateTimeKind.Utc), CreatedDate = new DateTime(2024, 1, 27, 13, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 27, 13, 0, 0, DateTimeKind.Utc) }
        };
        
        modelBuilder.Entity<Survey>().HasData(surveys);
    }
    
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
} 
