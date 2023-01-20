using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;

namespace WebApp.Pages.Games;

public class Play : PageModel
{
    public int PlayerId { get; set; } = 2;
    public Game Game { get; set; } = default!;
    public CheckersBrain Brain { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int IsPredicted { get; set; }

    public IActionResult OnGet(string? id, int? playerId, int? pieceX, int? pieceY, int? moveX, int? moveY)
    {
        if (id == null || !Util.GameRepo("Database").GameExist(id))
            return RedirectToPage("/Index", new { error = "Missing or invalid Game ID!" });

        if (playerId == null || (playerId != 2 && playerId != 3))
            return RedirectToPage("/Index", new { error = "Missing or invalid Player ID!" });

        PlayerId = playerId.Value;
        Game = Util.GameRepo("Database").GetGame(id);
        Brain = new CheckersBrain(Game, false);

        if (IsPredicted == 3)
        {
            Brain.Piece = null;
            Brain.State.Turn = Brain.State.Turn == EGamePiece.Player1 ? EGamePiece.Player2 : EGamePiece.Player1;

            Util.GameRepo("Database").SaveGame(Game.Name, Game);

            return Page();
        }

        if (Brain.State.Turn == EGamePiece.Player2 && !Brain.Options.Multiplayer)
        {
            Brain.AiMove(pieceX, pieceY);

            Util.GameRepo("Database").SaveGame(Game.Name, Game);

            return Page();
        }
        
        if (pieceX == null || pieceY == null)
            return Page();

        if (Brain.SelectPiece((int)pieceX, (int)pieceY, IsPredicted != 0) != null)
            return Page();

        if (moveX == null || moveY == null)
            return Page();

        var type = Brain.Piece?.Type;

        var result = Brain.Move((int)moveX, (int)moveY, true);

        if (Brain.State.Winner != null)
        {
            Util.GameRepo("Database").SaveGame(Game.Name, Game);

            return Page();
        }
        
        if (result != null)
        {
            if (result != "-")
                return Page();

            if (Brain.Piece?.Type == type)
            {
                IsPredicted = 1;

                Util.GameRepo("Database").SaveGame(Game.Name, Game);

                if (Brain.SelectPiece((int)moveX, (int)moveY, true) == null)
                    return RedirectToPage(new { id, playerId, IsPredicted, pieceX = moveX, pieceY = moveY });
            }

            Brain.Piece = null;
            Brain.State.Turn = Brain.State.Turn == EGamePiece.Player1 ? EGamePiece.Player2 : EGamePiece.Player1;
        }

        Util.GameRepo("Database").SaveGame(Game.Name, Game);

        return Page();
    }
}