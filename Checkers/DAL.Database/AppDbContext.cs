using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Database;

public class AppDbContext : DbContext
{
    private readonly string _databasePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}";
    
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameOptions> Options { get; set; } = default!;
    public DbSet<GameState> GameStates { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={_databasePath}checkers.db");
    }
}