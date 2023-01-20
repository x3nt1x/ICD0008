using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_SongPlayed
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DjId"] = new SelectList(_context.Djs, "Id", "Name");
        ViewData["SongId"] = new SelectList(_context.Songs, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public SongPlayed SongPlayed { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.SongsPlayed == null || SongPlayed == null)
            {
                return Page();
            }

            _context.SongsPlayed.Add(SongPlayed);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
