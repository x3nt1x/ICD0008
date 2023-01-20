using Domain;

namespace GameBrain;

public class SelectedPiece
{
    public int X { get; set; }
    public int Y { get; set; }
    public EGamePiece Type { get; set; }
    
    public SelectedPiece(int x, int y, EGamePiece type)
    {
        X = x;
        Y = y;
        Type = type;
    }
}