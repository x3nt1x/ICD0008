using Domain;

namespace DAL;

public interface IGameRepository
{
    void SaveGame(string id, Game game);
    void DeleteGame(string id);
    bool GameExist(string id);
    Game GetGame(string id);
    List<string?> ListGames();
}