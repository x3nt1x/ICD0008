using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_SongPlayed
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SongPlayed SongPlayed { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.SongsPlayed == null)
            {
                return NotFound();
            }

            var songplayed =  await _context.SongsPlayed.FirstOrDefaultAsync(m => m.Id == id);
            if (songplayed == null)
            {
                return NotFound();
            }
            SongPlayed = songplayed;
           ViewData["DjId"] = new SelectList(_context.Djs, "Id", "Name");
           ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(SongPlayed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongPlayedExists(SongPlayed.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SongPlayedExists(int id)
        {
          return (_context.SongsPlayed?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
