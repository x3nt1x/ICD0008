using System.ComponentModel.DataAnnotations;

namespace Domain;

public class GameOptions
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string? Name { get; set; }

    public int Width { get; set; } = 8;
    public int Height { get; set; } = 8;
    public bool Multiplayer { get; set; } = true;
    public bool HighlightMoves { get; set; } = true;
    public EGamePiece StartingPlayer { get; set; } = EGamePiece.Player1;
    
    public ICollection<Game>? Games { get; set; }
}