using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly string _databasePath = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Pizza{Path.DirectorySeparatorChar}";

    public AppDbContext CreateDbContext(string[] args)
    {
        if (!Directory.Exists(_databasePath))
            Directory.CreateDirectory(_databasePath);
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_databasePath}pizza.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}