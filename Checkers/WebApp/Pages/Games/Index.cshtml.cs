using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Games;

public class IndexModel : PageModel
{
    private readonly DAL.Database.AppDbContext _context;

    public IndexModel(DAL.Database.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    
    public IList<Game> Game { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Search))
            Game = await _context.Games.Where(option => option.Name.Contains(Search)).ToListAsync();
        else
            Game = await _context.Games.Include(game => game.Options).ToListAsync();
    }
    
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
            return RedirectToPage("./Index");

        var game = await _context.Games.FindAsync(id);

        if (game != null)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}