using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    public int OptionsId { get; set; }
    public GameOptions Options { get; set; } = default!;

    public ICollection<GameState>? GameStates { get; set; }

    // this is required for database migration
    public Game() { }
    
    public Game(GameOptions? options = null!)
    {
        Options = options ?? new GameOptions();
        GameStates ??= new List<GameState>();

        GameStates.Add(new GameState
        {
            Created = DateTime.Now,
            Turn = Options.StartingPlayer,
            GameBoard = new EGamePiece[Options.Width, Options.Height]
        });
    }

    public GameState GetLatestGameState()
    {
        return GameStates!.OrderByDescending(x => x.Created).First();
    }
    
    public string GetSerializedBoard()
    {
        var serializedBoard = "";
        var state = GetLatestGameState();

        for (var x = 0; x < Options.Width; x++)
        {
            for (var y = 0; y < Options.Height; y++)
                serializedBoard += (int)state.GameBoard[x, y];
        }

        return serializedBoard;
    }

    public void DeserializeBoard()
    {
        var piece = 0;
        var state = GetLatestGameState();
        
        state.GameBoard = new EGamePiece[Options.Width, Options.Height];

        for (var x = 0; x < Options.Width; x++)
        {
            for (var y = 0; y < Options.Height; y++)
            {
                state.GameBoard[x, y] = (EGamePiece)Convert.ToInt32(state.SerializedBoard[piece].ToString());
                piece++;
            }
        }
    }
}