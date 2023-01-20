using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.GameOptions;

public class IndexModel : PageModel
{
    private readonly DAL.Database.AppDbContext _context;

    public IndexModel(DAL.Database.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    
    public IList<Domain.GameOptions> GameOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Search))
            GameOptions = await _context.Options.Where(option => option.Name!.Contains(Search)).ToListAsync();
        else
            GameOptions = await _context.Options.ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
            return RedirectToPage("./Index");

        var gameOptions = await _context.Options.FindAsync(id);

        if (gameOptions != null)
        {
            _context.Options.Remove(gameOptions);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}