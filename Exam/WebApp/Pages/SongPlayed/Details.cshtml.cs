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
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

      public SongPlayed SongPlayed { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.SongsPlayed == null)
            {
                return NotFound();
            }

            var songplayed = await _context.SongsPlayed.FirstOrDefaultAsync(m => m.Id == id);
            if (songplayed == null)
            {
                return NotFound();
            }
            else 
            {
                SongPlayed = songplayed;
            }
            return Page();
        }
    }
}
