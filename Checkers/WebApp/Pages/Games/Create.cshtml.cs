using DAL;
using DAL.Database;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities;

namespace WebApp.Pages.Games;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IGameRepository _repo;

    public CreateModel(AppDbContext context, IGameRepository repo)
    {
        _context = context;
        _repo = repo;
    }

    [BindProperty]
    public string GameName { get; set; } = default!;

    [BindProperty]
    public string OptionsName { get; set; } = default!;

    public SelectList Options { get; set; } = default!;

    public IActionResult OnGet()
    {
        Options = new SelectList(Util.OptionsRepo("Database").ListOptions());

        return Page();
    }

    public Task<IActionResult> OnPostAsync()
    {
        var options = Util.OptionsRepo("Database").GetOptions(OptionsName);

        var game = new Game(options)
        {
            Name = GameName
        };

        new CheckersBrain(game);

        _repo.SaveGame(game.Name, game);

        return Task.FromResult<IActionResult>(RedirectToPage("./StartGame", new { id = game.Name }));
    }
}