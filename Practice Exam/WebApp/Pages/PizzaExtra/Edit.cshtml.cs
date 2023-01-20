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

namespace WebApp.Pages_PizzaExtra
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
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

            var pizzaextra =  await _context.PizzaExtras.FirstOrDefaultAsync(m => m.Id == id);
            if (pizzaextra == null)
            {
                return NotFound();
            }
            PizzaExtra = pizzaextra;
           ViewData["ExtraId"] = new SelectList(_context.Extras, "Id", "Name");
           ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Description");
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

            _context.Attach(PizzaExtra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaExtraExists(PizzaExtra.Id))
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

        private bool PizzaExtraExists(int id)
        {
          return (_context.PizzaExtras?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
