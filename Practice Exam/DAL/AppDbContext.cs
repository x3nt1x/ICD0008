using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    private readonly string _databasePath = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Pizza{Path.DirectorySeparatorChar}";
    
    public DbSet<Pizza> Pizzas { get; set; } = default!;
    public DbSet<Extra> Extras { get; set; } = default!;
    public DbSet<Purchase> Purchases { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<PizzaExtra> PizzaExtras { get; set; } = default!;
    public DbSet<PurchasedPizza> PurchasedPizzas { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={_databasePath}pizza.db");
    }
}