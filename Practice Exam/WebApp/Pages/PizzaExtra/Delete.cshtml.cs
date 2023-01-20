using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_PizzaExtra
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public PizzaExtra PizzaExtra { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PizzaExtras == null)
            {
                return NotFound();
            }

            var pizzaextra = await _context.PizzaExtras.FirstOrDefaultAsync(m => m.Id == id);

            if (pizzaextra == null)
            {
                return NotFound();
            }
            else 
            {
                PizzaExtra = pizzaextra;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.PizzaExtras == null)
            {
                return NotFound();
            }
            var pizzaextra = await _context.PizzaExtras.FindAsync(id);

            if (pizzaextra != null)
            {
                PizzaExtra = pizzaextra;
                _context.PizzaExtras.Remove(PizzaExtra);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
