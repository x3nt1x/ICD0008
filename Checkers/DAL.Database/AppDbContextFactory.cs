using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly string _databasePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}";

    public AppDbContext CreateDbContext(string[] args)
    {
        if (!Directory.Exists(_databasePath))
            Directory.CreateDirectory(_databasePath);
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_databasePath}checkers.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}