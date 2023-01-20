using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Plan;

public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;

    public Index(DAL.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    
    public IList<Domain.Dj> Djs { get; set; } = default!;

    public async void OnGet()
    {
        if (!string.IsNullOrEmpty(Search))
        {
            Djs = await _context.Djs.Where(dj =>
                dj.Name.ToLower().Contains(Search.ToLower())
                || dj.Price.ToString().Contains(Search)).ToListAsync();
        }
        else
        {
            Djs = await _context.Djs.ToListAsync();
        }
    }

    public async Task<IActionResult> OnPost(int? id)
    {
        if (id == null)
            return RedirectToPage("Index");
        
        var dj = _context.Djs.FirstOrDefault(dj => dj.Id == id);
        if (dj == null)
            return RedirectToPage("Index");

        if (dj.Performs)
        {
            var songPlayed = _context.SongsPlayed
                .Include(o => o.Dj)
                .Include(o => o.Author)
                .Include(o => o.Song)
                .Where(o => o.Dj!.Id == id).ToList();

            foreach (var played in songPlayed)
            {
                played.Song!.TimesPlayed--;
                played.Author!.Price -= played.Song.Price;
            }
            
            await _context.SongsPlayed
                .Where(purchase => purchase.DjId == dj.Id)
                .ExecuteDeleteAsync();
        }

        dj.Performs = !dj.Performs;
        
        await _context.SaveChangesAsync();
        
        return RedirectToPage("Index");
    }
}