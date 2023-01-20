using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Dj
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

      public Dj Dj { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Djs == null)
            {
                return NotFound();
            }

            var dj = await _context.Djs.FirstOrDefaultAsync(m => m.Id == id);
            if (dj == null)
            {
                return NotFound();
            }
            else 
            {
                Dj = dj;
            }
            return Page();
        }
    }
}
