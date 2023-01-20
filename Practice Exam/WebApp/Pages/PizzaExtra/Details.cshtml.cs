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
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

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
    }
}
