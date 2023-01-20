namespace Domain;

public class GameState
{
    public int Id { get; set; }

    public DateTime Created { get; set; }
    
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    
    public EGamePiece Turn { get; set; }
    public EGamePiece? Winner { get; set; }
    
    public EGamePiece[,] GameBoard = null!;
    public string SerializedBoard { get; set; } = null!;
    
    public int GameId { get; set; }
    public Game? Game { get; set; }
}