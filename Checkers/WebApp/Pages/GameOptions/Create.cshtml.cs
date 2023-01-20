using DAL.Database;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.GameOptions;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Domain.GameOptions GameOptions { get; set; } = default!;

    public SelectList PlayersList { get; set; } = default!;

    public IActionResult OnGet()
    {
        var pieces = Enum.GetValues(typeof(EGamePiece)).Cast<EGamePiece>();

        PlayersList = new SelectList(pieces.Where(piece => piece is EGamePiece.Player1 or EGamePiece.Player2));

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _context.Options.Add(GameOptions);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}