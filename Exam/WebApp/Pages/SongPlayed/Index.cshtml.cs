using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_SongPlayed
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<SongPlayed> SongPlayed { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.SongsPlayed != null)
            {
                SongPlayed = await _context.SongsPlayed
                .Include(s => s.Dj)
                .Include(s => s.Song).ToListAsync();
            }
        }
    }
}
