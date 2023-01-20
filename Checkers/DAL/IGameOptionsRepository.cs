using Domain;

namespace DAL;

public interface IGameOptionsRepository
{
    void SaveOptions(string id, GameOptions options);
    void DeleteOptions(string id);
    bool OptionsExist(string id);
    GameOptions GetOptions(string id);
    List<string?> ListOptions();
}