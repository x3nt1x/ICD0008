using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Database;

public class GameOptionsRepositoryDatabase : IGameOptionsRepository
{
    private static readonly string DatabasePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}";

    private static readonly DbContextOptions<AppDbContext> DbOptions =
        new DbContextOptionsBuilder<AppDbContext>().UseSqlite($"Data Source={DatabasePath}checkers.db").Options;

    private readonly AppDbContext _dbContext;

    public GameOptionsRepositoryDatabase()
    {
        _dbContext = new AppDbContext(DbOptions);

        _dbContext.Database.Migrate();
        
        if (!_dbContext.Options.Any())
            SaveOptions("default", new GameOptions());
    }

    public void SaveOptions(string id, GameOptions option)
    {
        var existingOptions = GetExistingOptions(option);

        if (existingOptions != null)
        {
            existingOptions.Name = id;

            _dbContext.Options.Update(existingOptions);
        }
        else
        {
            option.Name = id;
            
            _dbContext.Options.Add(option);
        }

        _dbContext.SaveChanges();
    }

    public void DeleteOptions(string id)
    {
        _dbContext.Options.Remove(GetExistingOptions(id)!);

        _dbContext.SaveChanges();
    }

    public bool OptionsExist(string id)
    {
        return _dbContext.Options.Any(options => options.Name == id);
    }
    
    public GameOptions GetOptions(string id)
    {
        return GetExistingOptions(id)!;
    }

    public List<string?> ListOptions()
    {
        return _dbContext.Options.Select(options => options.Name).Where(name => name != null).ToList();
    }

    private GameOptions? GetExistingOptions(string id)
    {
        return _dbContext.Options.FirstOrDefault(options => options.Name == id);
    }

    private GameOptions? GetExistingOptions(GameOptions options)
    {
        // overriding .Equals in GameOptions didn't work here :(
        return _dbContext.Options.FirstOrDefault(gameOptions =>
            gameOptions.Width == options.Width &&
            gameOptions.Height == options.Height &&
            gameOptions.Multiplayer == options.Multiplayer &&
            gameOptions.HighlightMoves == options.HighlightMoves &&
            gameOptions.StartingPlayer == options.StartingPlayer);
    }
}