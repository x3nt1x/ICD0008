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
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<PizzaExtra> PizzaExtra { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.PizzaExtras != null)
            {
                PizzaExtra = await _context.PizzaExtras
                .Include(p => p.Extra)
                .Include(p => p.Pizza).ToListAsync();
            }
        }
    }
}
