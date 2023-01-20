using Domain;

namespace DAL.FileSystem;

public class GameRepositoryFileSystem : IGameRepository
{
    private const string FileExtension = ".json";

    private readonly string _savesPath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Saves{Path.DirectorySeparatorChar}";
    
    public GameRepositoryFileSystem()
    {
        if (!Directory.Exists(_savesPath))
            Directory.CreateDirectory(_savesPath);
    }

    public void SaveGame(string id, Game game)
    {
        game.Name = id;
        game.GetLatestGameState().SerializedBoard = game.GetSerializedBoard();
        
        var fileContent = System.Text.Json.JsonSerializer.Serialize(game);
        File.WriteAllText(GetFileName(id), fileContent);
    }

    public void DeleteGame(string id)
    {
        File.Delete(GetFileName(id));
    }
    
    public bool GameExist(string id)
    {
        return File.Exists($"{_savesPath}{id}{FileExtension}");
    }

    public Game GetGame(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var game = System.Text.Json.JsonSerializer.Deserialize<Game>(fileContent);
        
        if (game == null)
            throw new NullReferenceException($"Could not deserialize: {fileContent}");

        game.DeserializeBoard();
        
        return game;
    }
    
    public List<string?> ListGames()
    {
        return Directory.GetFiles(_savesPath, $"*{FileExtension}").Select(Path.GetFileNameWithoutExtension).ToList();
    }

    private string GetFileName(string id)
    {
        return $"{_savesPath}{Path.DirectorySeparatorChar}{id}{FileExtension}";
    }
}