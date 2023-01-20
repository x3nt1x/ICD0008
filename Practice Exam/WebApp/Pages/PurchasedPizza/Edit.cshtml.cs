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

namespace WebApp.Pages_PurchasedPizza
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
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

            var purchasedpizza =  await _context.PurchasedPizzas.FirstOrDefaultAsync(m => m.Id == id);
            if (purchasedpizza == null)
            {
                return NotFound();
            }
            PurchasedPizza = purchasedpizza;
           ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Description");
           ViewData["PurchaseId"] = new SelectList(_context.Purchases, "Id", "ClientGUID");
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

            _context.Attach(PurchasedPizza).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchasedPizzaExists(PurchasedPizza.Id))
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

        private bool PurchasedPizzaExists(int id)
        {
          return (_context.PurchasedPizzas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
