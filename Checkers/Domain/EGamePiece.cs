namespace Domain;

public enum EGamePiece
{
    Unavailable,    // white square
    Empty,          // empty black square
    Player1,        // ⦿ - player 1 piece (white)
    Player2,        // ⦾ - player 2 piece (black)
    King1,          // ♛ - player 1 king (white)
    King2,          // ♕ - player 2 king (black)
    Playable,       // square to which selected piece can move
    Takeable,       // piece which current player can take
    TakeableKing    // king which current player can take
}