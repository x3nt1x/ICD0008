using DAL.Database;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.GameStates;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }
    
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public IList<GameState> GameState { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Search))
        {
            GameState = await _context.GameStates
                .Include(gameState => gameState.Game)
                .Where(gameState => gameState.Game!.Name.Contains(Search)).ToListAsync();
        }
        else
        {
            GameState = await _context.GameStates
                .Include(gameState => gameState.Game).ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
            return RedirectToPage("./Index");

        var gameState = await _context.GameStates.FindAsync(id);

        if (gameState != null)
        {
            _context.GameStates.Remove(gameState);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}