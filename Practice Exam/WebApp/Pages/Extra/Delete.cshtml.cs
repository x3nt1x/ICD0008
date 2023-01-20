using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Extra
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Extra Extra { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras.FirstOrDefaultAsync(m => m.Id == id);

            if (extra == null)
            {
                return NotFound();
            }
            else 
            {
                Extra = extra;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }
            var extra = await _context.Extras.FindAsync(id);

            if (extra != null)
            {
                Extra = extra;
                _context.Extras.Remove(Extra);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
