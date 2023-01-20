using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.GameOptions;

public class EditModel : PageModel
{
    private readonly DAL.Database.AppDbContext _context;

    public EditModel(DAL.Database.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Domain.GameOptions GameOptions { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
            return NotFound();

        var gameOptions = await _context.Options.FirstOrDefaultAsync(options => options.Id == id);
        if (gameOptions == null)
            return NotFound();

        GameOptions = gameOptions;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _context.Attach(GameOptions).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GameOptionsExists(GameOptions.Id))
                return NotFound();

            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool GameOptionsExists(int id)
    {
        return (_context.Options?.Any(options => options.Id == id)).GetValueOrDefault();
    }
}