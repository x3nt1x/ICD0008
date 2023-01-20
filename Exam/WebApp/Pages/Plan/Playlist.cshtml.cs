using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Plan;

public class Playlist : PageModel
{
    private readonly DAL.AppDbContext _context;

    public Playlist(DAL.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SelectedDj { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Error { get; set; }

    public IList<Domain.Song> Songs { get; set; } = default!;
    public IList<Domain.Dj> Djs { get; set; } = default!;

    public async void OnGet(int? dj)
    {
        if (!string.IsNullOrEmpty(Search))
        {
            Search = Search.ToLower();

            Songs = await _context.Songs
                .Include(song => song.Author)
                .Include(song => song.Category)
                .Where(song =>
                    song.Name.ToLower().Contains(Search)
                    || song.Price.ToString().Contains(Search)
                    || song.Author!.Name.ToLower().Contains(Search)
                    || song.Category!.Name.ToLower().Contains(Search)).ToListAsync();
        }
        else
        {
            Songs = await _context.Songs
                .Include(song => song.Author)
                .Include(song => song.Category)
                .ToListAsync();
        }

        Djs = await _context.Djs.Where(dj1 => dj1.Performs).ToListAsync();
    }

    public async Task<IActionResult> OnPost(int? songId, int? dj, string? rmv)
    {
        if (dj != null)
            SelectedDj = (int)dj;

        if (songId == null)
            return RedirectToPage(new { SelectedDj });
        
        var currentDj = _context.Djs.Include(dj1 => dj1.SongsPlayed).FirstOrDefault(dj1 => dj1.Id == dj);
        if (currentDj == null)
            return RedirectToPage( new { Error = "Missing or invalid DJ ID!" });
        
        var song = _context.Songs
            .Include(song => song.Author)
            .FirstOrDefault(song => song.Id == songId);
        
        if (song == null)
            return RedirectToPage(new { SelectedDj });

        if (rmv != null)
        {
            var songPlayed = _context.SongsPlayed.FirstOrDefault(s => s.SongId == song.Id && s.DjId == currentDj.Id);
            if (songPlayed == null)
                return RedirectToPage(new { SelectedDj });
            
            song.TimesPlayed--;
            song.Author!.Price -= song.Price;
        
            _context.SongsPlayed.Remove(songPlayed);

            await _context.SaveChangesAsync();

            return RedirectToPage(new { SelectedDj });
        }
        
        currentDj.SongsPlayed ??= new List<Domain.SongPlayed>();

        var newSongPlayed = new Domain.SongPlayed
        {
            Dj = currentDj,
            Song = song,
            Author = song.Author
        };

        song.TimesPlayed++;
        song.Author!.Price += song.Price;
        
        currentDj.SongsPlayed.Add(newSongPlayed);

        await _context.SaveChangesAsync();

        return RedirectToPage(new { SelectedDj });
    }
}