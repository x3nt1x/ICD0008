using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;

namespace WebApp.Pages.Games;

public class StartGame : PageModel
{
    public string? Id { get; set; }

    public IActionResult OnGet(string? id)
    {
        if (id == null || !Util.GameRepo("Database").GameExist(id))
            return RedirectToPage("/Index", new { error = "Missing or invalid Game ID!" });

        var game = Util.GameRepo("Database").GetGame(id);

        if (!game.Options.Multiplayer)
            return RedirectToPage("./Play", new { id = game.Name, playerId = 2 });

        Id = game.Name;

        return Page();
    }
}