using Domain;

namespace DAL.FileSystem;

public class GameOptionsRepositoryFileSystem : IGameOptionsRepository
{
    private const string FileExtension = ".json";
    
    private readonly string _optionsPath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}" +
        $"Checkers{Path.DirectorySeparatorChar}Options{Path.DirectorySeparatorChar}";

    public GameOptionsRepositoryFileSystem()
    {
        if (!Directory.Exists(_optionsPath) || !File.Exists($"{_optionsPath}default{FileExtension}"))
            SaveOptions("default", new GameOptions());
    }

    public void SaveOptions(string id, GameOptions options)
    {
        Directory.CreateDirectory(_optionsPath);

        options.Name = id;

        var fileContent = System.Text.Json.JsonSerializer.Serialize(options);
        File.WriteAllText(GetFileName(id), fileContent);
    }

    public void DeleteOptions(string id)
    {
        File.Delete(GetFileName(id));
    }
    
    public bool OptionsExist(string id)
    {
        return File.Exists($"{_optionsPath}{id}{FileExtension}");
    }

    public GameOptions GetOptions(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var options = System.Text.Json.JsonSerializer.Deserialize<GameOptions>(fileContent);
        
        if (options == null)
            throw new NullReferenceException($"Could not deserialize: {fileContent}");

        return options;
    }
    
    public List<string?> ListOptions()
    {
        return Directory.GetFiles(_optionsPath, $"*{FileExtension}").Select(Path.GetFileNameWithoutExtension).ToList();
    }

    private string GetFileName(string id)
    {
        return $"{_optionsPath}{Path.DirectorySeparatorChar}{id}{FileExtension}";
    }
}