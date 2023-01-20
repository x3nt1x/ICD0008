using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly DAL.AppDbContext _context;

    public IndexModel(DAL.AppDbContext context)
    {
        _context = context;
    }

    public IList<Domain.Dj> Djs { get; set; } = default!;
    public IList<Domain.Author> Authors { get; set; } = default!;

    public double TotalCost { get; set; }
    public string ApproximateDuration { get; set; } = default!;

    public async void OnGet()
    {
        Djs = await _context.Djs
            .Include(dj => dj.SongsPlayed)!
            .ThenInclude(songPlayed => songPlayed.Song)
            .ThenInclude(song => song!.Author)
            .Include(song => song.SongsPlayed)!
            .ThenInclude(songPlayed => songPlayed.Song)
            .ThenInclude(song => song!.Category)
            .Where(dj => dj.Performs)
            .ToListAsync();

        Authors = await _context.Authors
            .Include(author => author.SongsPlayed!)
            .ThenInclude(songPlayed => songPlayed.Song)
            .Where(author => author.Price > 0)
            .ToListAsync();

        TotalCost = Authors.Sum(a => a.Price) + Djs.Sum(s => s.Price);
        ApproximateDuration = TimeSpan.FromSeconds(_context.SongsPlayed.Sum(a => a.Song!.Length)).ToString(@"hh\:mm\:ss");
    }
}