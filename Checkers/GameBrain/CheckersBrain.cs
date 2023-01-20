using Domain;

namespace GameBrain;

public class CheckersBrain
{
    public SelectedPiece? Piece;
    public readonly GameState State;
    public readonly GameOptions Options;
    private readonly int _piecesPerPlayer;
    private List<(int x, int y, EGamePiece piece)>? _validMoves;

    public CheckersBrain(Game game, bool newGame = true)
    {
        State = game.GetLatestGameState();
        Options = game.Options;
        
        var initialEmptyRows = Options.Width % 2 == 0 ? 2 : 3;    // 2 empty rows for even boards and 3 for uneven boards
        var initialFilledRows = Options.Width - initialEmptyRows; // total rows which have game pieces on them initially
        var rowsPerPlayer = initialFilledRows / 2; // are there enough rows for opponent to leave empty squares between players
        _piecesPerPlayer = Convert.ToInt32(Math.Floor(Options.Height * rowsPerPlayer / 2.0));
        
        if (!newGame)
            return;

        if (Options.Width < 4 || Options.Height < 4)
            throw new ArgumentException("Board size too small");

        var emptyRows = 0;            // how many empty rows are created
        var leaveSquareEmpty = false; // don't put piece on this square
        var opponentPieces = true;    // start placing pieces from left to right, top to bottom, which means placing opponent's pieces first

        for (var x = 0; x < Options.Width; x++)
        {
            for (var y = 0; y < Options.Height; y++)
            {
                if (x % 2 == 0 ? y % 2 != 0 : y % 2 == 0)
                {
                    if (leaveSquareEmpty)
                    {
                        State.GameBoard[x, y] = EGamePiece.Empty;
                        continue;
                    }

                    if (!opponentPieces)
                        State.GameBoard[x, y] = EGamePiece.Player1;
                    else
                        State.GameBoard[x, y] = EGamePiece.Player2;
                }
                else
                {
                    State.GameBoard[x, y] = EGamePiece.Unavailable;
                }
            }

            if (x + 1 < rowsPerPlayer)
                continue;

            if (emptyRows != initialEmptyRows)
            {
                leaveSquareEmpty = true;
                emptyRows++;
            }
            else
            {
                leaveSquareEmpty = false;
                opponentPieces = false;
            }
        }
    }

    public bool IsPlayerPiece(int x, int y)
    {
        switch (State.Turn)
        {
            case EGamePiece.Player1 when State.GameBoard[x, y] is EGamePiece.Player1 or EGamePiece.King1:
            case EGamePiece.Player2 when State.GameBoard[x, y] is EGamePiece.Player2 or EGamePiece.King2:
                return true;
        }

        return false;
    }
    
    private (int x, int y, List<EGamePiece> pieces) GetOpponent(int oldX = 0, int oldY = 0, int newX = 0, int newY = 0)
    {
        // get opponent's piece location
        var x = newX < oldX ? newX + 1 : newX - 1;
        var y = newY < oldY ? newY + 1 : newY - 1;

        List<EGamePiece> pieces;

        if (State.Turn == EGamePiece.Player1)
            pieces = new List<EGamePiece> { EGamePiece.Player2, EGamePiece.King2 };
        else
            pieces = new List<EGamePiece> { EGamePiece.Player1, EGamePiece.King1 };

        return (x, y, pieces);
    }

    public string? SelectPiece(int x, int y, bool isPredicted = false, bool isAi = false)
    {
        // check if piece exists
        if (x < 0 || y < 0 || x >= Options.Width || y >= Options.Height)
            return "Can't select the piece as it does not exist!";
        
        // check if piece belongs to player
        if (!IsPlayerPiece(x, y))
            return $"Can't select the piece as it does not belong to {State.Turn}!";

        // select piece
        Piece = new SelectedPiece(x, y, State.GameBoard[x, y]);

        // place to store valid moves
        _validMoves = new List<(int x, int y, EGamePiece piece)>();
        
        // don't return any string, when everything worked out fine
        if (FindMoves(x, y, isPredicted, false, isAi))
            return null;
        
        Piece = null;
        return "Can't select the piece as it does not have any available moves!";
    }

    private bool FindMoves(int pieceX, int pieceY, bool isPredicted = false, bool quickFind = false, bool isAi = false)
    {
        for (var x = pieceX - 1; x <= pieceX + 1; x += 2)
        {
            // x is out of GameBoard bounds
            if (x < 0 || x >= Options.Width)
                continue;

            // TODO: make this optional
            // regular pieces can only move forwards when such option is enabled
            if (true)
            {
                if (x >= pieceX && Piece!.Type == EGamePiece.Player1)
                    continue;

                if (x <= pieceX && Piece!.Type == EGamePiece.Player2)
                    continue;
            }
            
            for (var y = pieceY - 1; y <= pieceY + 1; y += 2)
            {
                // y is out of GameBoard bounds
                if (y < 0 || y >= Options.Height)
                    continue;

                var piece = State.GameBoard[x, y];
                
                // get all available moves near the selected piece, don't do this for predicted pieces
                if (!isPredicted && piece == EGamePiece.Empty)
                {
                    // quickly find if any moves exist
                    if (quickFind)
                        return true;
                    
                    // store the square
                    _validMoves?.Add((x, y, piece));
                    
                    // mark square as playable
                    State.GameBoard[x, y] = EGamePiece.Playable;
                    continue;
                }

                if (!GetOpponent().pieces.Contains(piece))
                    continue;

                // predict where selected piece will be after user chooses to make this move
                var predictedX = x < pieceX ? x - 1 : x + 1;
                var predictedY = y < pieceY ? y - 1 : y + 1;

                // predicted x is out of GameBoard bounds
                if (predictedX < 0 || predictedX >= Options.Width)
                    continue;

                // predicted y is out of GameBoard bounds
                if (predictedY < 0 || predictedY >= Options.Height)
                    continue;

                // already predicted move from this position, just mark piece as takeable 
                if (State.GameBoard[predictedX, predictedY] == EGamePiece.Playable)
                {
                    _validMoves?.Add((x, y, piece));
                    
                    State.GameBoard[x, y] = piece is EGamePiece.King1 or EGamePiece.King2 ? EGamePiece.TakeableKing : EGamePiece.Takeable;
                }

                // can't make this move anyway, don't predict
                if (State.GameBoard[predictedX, predictedY] != EGamePiece.Empty)
                    continue;
                
                // quickly find if any moves exist
                if (quickFind)
                    return true;
                
                // store predicted square
                _validMoves?.Add((predictedX, predictedY, State.GameBoard[predictedX, predictedY]));
                
                // mark predicted square as playable
                State.GameBoard[predictedX, predictedY] = EGamePiece.Playable;
                
                // store piece
                _validMoves?.Add((x, y, piece));
                
                // mark piece as takeable
                State.GameBoard[x, y] = piece is EGamePiece.King1 or EGamePiece.King2 ? EGamePiece.TakeableKing : EGamePiece.Takeable;

                // find only takeable moves from predicted square, don't do this for AI
                if (!isAi)
                    FindMoves(predictedX, predictedY, true);
            }
        }

        // can the piece make any moves in it's current position
        return _validMoves != null && _validMoves.Any();
    }

    private void MovePiece(int x, int y)
    {
        if (Piece == null)
            return;

        var opponent = GetOpponent(Piece.X, Piece.Y, x, y);

        // did the move result in taking enemy's piece
        if (opponent.x != Piece.X && opponent.y != Piece.Y)
        {
            State.GameBoard[opponent.x, opponent.y] = EGamePiece.Empty;

            if (State.Turn == EGamePiece.Player1)
                State.Player1Score++;
            else
                State.Player2Score++;
        }
        
        // if piece reached opponent's 1st row, turn it into king
        if (x == 0 && State.Turn == EGamePiece.Player1)
            Piece.Type = EGamePiece.King1;
        else if (x == Options.Width - 1 && State.Turn == EGamePiece.Player2)
            Piece.Type = EGamePiece.King2;

        // select piece's new location
        Piece.X = x;
        Piece.Y = y;
    }

    public bool IsValidMove(int x, int y)
    {
        return CheckMoves(new List<(int x, int y)> { (x, y) });
    }
    
    private bool CheckMoves(List<(int x, int y)> moves)
    {
        if (Piece == null)
            return false;

        var index = 0;
        var pieceX = Piece.X;
        var pieceY = Piece.Y;

        foreach (var (x, y) in moves)
        {
            // check if move is inside playable area
            if (x < 0 || y < 0 || x >= Options.Width || y >= Options.Height)
                return false;
            
            // can't move to unplayable square
            if (State.GameBoard[x, y] != EGamePiece.Playable)
                return false;

            // TODO: make this optional
            // regular pieces can only move forwards when such option is enabled
            if (true)
            {
                if (x >= Piece.X && Piece.Type == EGamePiece.Player1)
                    return false;

                if (x <= Piece.X && Piece.Type == EGamePiece.Player2)
                    return false;
            }

            // move is too far from current position
            if (Math.Abs(pieceX - x) > 2 || Math.Abs(pieceY - y) > 2)
                return false;

            var opponent = GetOpponent(pieceX, pieceY, x, y);

            /*
             1st move doesn't always require opponent piece in between moves,
             but when move is further than 1 square, then it does.
             every next move after that require opponent piece in between moves
            */
            if (index > 0 || Math.Abs(Piece.X - x) > 1 || Math.Abs(Piece.Y - y) > 1)
            {
                if (State.GameBoard[opponent.x, opponent.y] is not EGamePiece.Takeable and not EGamePiece.TakeableKing)
                    return false;
            }

            // can't move back to same square where we just came from
            if (index > 1 && moves[index - 2].x == x && moves[index - 2].y == y)
                return false;

            index++;
            pieceX = x;
            pieceY = y;
        }

        return true;
    }
    
    public bool EndMove()
    {
        var morePossibleMoves = false;
        
        // restore unaffected squares & pieces
        foreach (var validMove in _validMoves!)
        {
            if (State.GameBoard[validMove.x, validMove.y] == EGamePiece.Empty)
            {
                morePossibleMoves = true;
                continue;
            }

            State.GameBoard[validMove.x, validMove.y] = validMove.piece;
        }

        return morePossibleMoves;
    }

    public string? Move(int x, int y, bool keepMoving = false)
    {
        return Move(new List<(int x, int y)> { (x, y) }, keepMoving);
    }

    public string? Move(List<(int x, int y)> moves, bool keepMoving = false)
    {
        if (Piece == null)
            return "No piece selected!";

        // check if inputted moves are valid before actually doing them
        if (!CheckMoves(moves))
            return "Can't make this move!";

        // free selected piece's current position
        State.GameBoard[Piece.X, Piece.Y] = EGamePiece.Empty;

        // actually do the moves
        foreach (var move in moves)
            MovePiece(move.x, move.y);

        // probably more possible moves?
        var morePossibleMoves = EndMove();

        // place piece on new location
        State.GameBoard[Piece.X, Piece.Y] = Piece.Type;

        // did move result in winning the game
        if (State.Player1Score >= _piecesPerPlayer || State.Player2Score >= _piecesPerPlayer)
        {
            Piece = null;
            State.Winner = State.Turn;
            
            return null;
        }
        
        // keep moving with same piece
        if (keepMoving && morePossibleMoves)
            return "-";
        
        // move is completely done
        Piece = null;
        State.Turn = State.Turn == EGamePiece.Player1 ? EGamePiece.Player2 : EGamePiece.Player1;
        
        return null;
    }

    public void AiMove(int? x = null, int? y = null)
    {
        var random = new Random();

        if (x != null && y != null)
        {
            // try selecting existing piece if it has more available moves
            if (SelectPiece((int)x, (int)y, true, true) != null)
            {
                // if it can't select the piece, AI's turn is over
                State.Turn = State.Turn == EGamePiece.Player1 ? EGamePiece.Player2 : EGamePiece.Player1;
                
                return;
            }
        }
        else
        {
            // get all AI pieces which have available moves
            var pieces = GetPlayerPieces();

            // randomly select one of the pieces
            var piece = pieces[random.Next(pieces.Count)];

            SelectPiece(piece.x, piece.y, false, true);
        }
        
        // get all possible moves AI can make with its current selected piece
        var moves = _validMoves!.Where(move => IsValidMove(move.x, move.y)).ToList();

        // randomly make one of the moves
        var move = moves[random.Next(moves.Count)];

        Move(move.x, move.y, true);
    }

    private List<(int x, int y)> GetPlayerPieces()
    {
        // clear all previously stored moves
        _validMoves?.Clear();
        
        // get all current player pieces which can make a move
        var pieces = new List<(int x, int y)>();
        
        for (var x = 0; x < Options.Width; x++)
        {
            for (var y = 0; y < Options.Height; y++)
            {
                if (!IsPlayerPiece(x, y))
                    continue;

                Piece = new SelectedPiece(x, y, State.GameBoard[x, y]);

                if (FindMoves(x, y, false, true))
                    pieces.Add((x, y));
            }
        }

        return pieces;
    }

    public EGamePiece[,] GetBoard()
    {
        var copy = new EGamePiece[Options.Width, Options.Height];
        Array.Copy(State.GameBoard, copy, State.GameBoard.Length);
        
        return copy;
    }
}