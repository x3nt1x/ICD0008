using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    private readonly string _databasePath = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Exam{Path.DirectorySeparatorChar}";
    
    public DbSet<Dj> Djs { get; set; } = default!;
    public DbSet<Song> Songs { get; set; } = default!;
    public DbSet<Author> Authors { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<SongPlayed> SongsPlayed { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={_databasePath}exam.db");
    }
}