using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_PizzaExtra
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
        ViewData["ExtraId"] = new SelectList(_context.Extras, "Id", "Name");
        ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Description");
            return Page();
        }

        [BindProperty]
        public PizzaExtra PizzaExtra { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.PizzaExtras == null || PizzaExtra == null)
            {
                return Page();
            }

            _context.PizzaExtras.Add(PizzaExtra);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
