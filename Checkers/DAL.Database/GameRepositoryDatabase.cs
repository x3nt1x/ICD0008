using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Database;

public class GameRepositoryDatabase : IGameRepository
{
     private static readonly string DatabasePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}";
    
    private static readonly DbContextOptions<AppDbContext> DbOptions =
        new DbContextOptionsBuilder<AppDbContext>().UseSqlite($"Data Source={DatabasePath}checkers.db").Options;

    private readonly AppDbContext _dbContext;

    public GameRepositoryDatabase()
    {
        _dbContext = new AppDbContext(DbOptions);
        
        _dbContext.Database.Migrate();
    }

    public void SaveGame(string id, Game game)
    {
        var state = game.GetLatestGameState();
        var existingGame = GetExistingGame(id);
        var thisGame = existingGame;

        if (existingGame == null)
        {
            thisGame = new Game
            {
                Name = id,
                Options = GetOptions(game.Options),
                GameStates = new List<GameState>()
            };
        }

        thisGame!.GameStates!.Add(new GameState
        {
            Created = DateTime.Now,
            Player1Score = state.Player1Score,
            Player2Score = state.Player2Score,
            Turn = state.Turn,
            Winner = state.Winner,
            SerializedBoard = game.GetSerializedBoard()
        });

        if (existingGame != null)
            _dbContext.Games.Update(thisGame);
        else
            _dbContext.Games.Add(thisGame);

        _dbContext.SaveChanges();
    }

    public void DeleteGame(string id)
    {
        _dbContext.Games.Remove(GetExistingGame(id)!);
        
        _dbContext.SaveChanges();
    }
    
    public bool GameExist(string id)
    {
        return _dbContext.Games.Any(game => game.Name == id);
    }

    public Game GetGame(string id)
    {
        var game = GetExistingGame(id)!;

        game.DeserializeBoard();

        return game;
    }

    public List<string?> ListGames()
    {
        return _dbContext.Games.Select(game => game.Name).ToList()!;
    }

    private Game? GetExistingGame(string id)
    {
        return _dbContext.Games
            .Include(game => game.Options)
            .Include(game => game.GameStates)
            .FirstOrDefault(game => game.Name == id);
    }

    private GameOptions GetOptions(GameOptions options)
    {
        /*
         check if database already has GameOptions that match with current game's options,
         if so - use it, if not - create new
        */
        
        // overriding .Equals in GameOptions didn't work :(
        var existingOptions = _dbContext.Options.FirstOrDefault(gameOptions =>
            gameOptions.Width == options.Width &&
            gameOptions.Height == options.Height &&
            gameOptions.Multiplayer == options.Multiplayer &&
            gameOptions.HighlightMoves == options.HighlightMoves &&
            gameOptions.StartingPlayer == options.StartingPlayer);

        if (existingOptions != null)
            return existingOptions;
        
        var newOptions = new GameOptions
        {
            Name = options.Name,
            Width = options.Width,
            Height = options.Height,
            Multiplayer = options.Multiplayer,
            HighlightMoves = options.HighlightMoves,
            StartingPlayer = options.StartingPlayer
        };

        return newOptions;
    }
}