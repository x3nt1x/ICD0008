using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_PurchasedPizza
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
        ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Description");
        ViewData["PurchaseId"] = new SelectList(_context.Purchases, "Id", "ClientGUID");
            return Page();
        }

        [BindProperty]
        public PurchasedPizza PurchasedPizza { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.PurchasedPizzas == null || PurchasedPizza == null)
            {
                return Page();
            }

            _context.PurchasedPizzas.Add(PurchasedPizza);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
