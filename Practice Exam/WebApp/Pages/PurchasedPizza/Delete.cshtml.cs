using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_PurchasedPizza
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public PurchasedPizza PurchasedPizza { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PurchasedPizzas == null)
            {
                return NotFound();
            }

            var purchasedpizza = await _context.PurchasedPizzas.FirstOrDefaultAsync(m => m.Id == id);

            if (purchasedpizza == null)
            {
                return NotFound();
            }
            else 
            {
                PurchasedPizza = purchasedpizza;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.PurchasedPizzas == null)
            {
                return NotFound();
            }
            var purchasedpizza = await _context.PurchasedPizzas.FindAsync(id);

            if (purchasedpizza != null)
            {
                PurchasedPizza = purchasedpizza;
                _context.PurchasedPizzas.Remove(PurchasedPizza);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
