using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly DAL.AppDbContext _context;

    public IndexModel(DAL.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public IList<Domain.Pizza> Pizzas { get; set; } = default!;

    public async void OnGet()
    {
        // tracking
        if (Request.Cookies["uid"] == null)
        {
            var option = new CookieOptions();  
            Response.Cookies.Append("uid", Guid.NewGuid().ToString(), option);
        }
        
        // search
        if (!string.IsNullOrEmpty(Search))
        {
            Search = Search.ToLower();
            
            Pizzas = await _context.Pizzas.Include(pizza => pizza.Category)
                .Where(pizza =>
                    pizza.Name.ToLower().Contains(Search)
                    || pizza.Description.ToLower().Contains(Search)
                    || pizza.Category!.Name.ToLower().Contains(Search)).ToListAsync();
        }
        else
        {
            Pizzas = await _context.Pizzas.Include(pizza => pizza.Category).ToListAsync();
        }
    }

    public async Task<IActionResult> OnPost(int? id)
    {
        if (id == null)
            return Page();
        
        var pizza = _context.Pizzas.FirstOrDefault(pizza => pizza.Id == id);
        if (pizza == null)
            return Page();

        var uid = Request.Cookies["uid"];
        if (uid == null)
            return Page();

        var purchase = _context.Purchases
            .Include(purchase => purchase.PurchasedPizzas)
            .FirstOrDefault(purchase => purchase.ClientGUID == uid && !purchase.Finalized);
        
        if (purchase == null)
        {
            purchase = new Domain.Purchase
            {
                ClientGUID = uid,
                PurchasedPizzas = new List<Domain.PurchasedPizza>()
            };

            _context.Add(purchase);
        }

        var purchasedPizza = new Domain.PurchasedPizza
        {
            Pizza = pizza,
            Purchase = purchase,
        };
        
        purchase.PurchasedPizzas!.Add(purchasedPizza);
        purchase.TotalPrice += pizza.Price;
        
        // not required???
        //_context.Update(purchase);
        await _context.SaveChangesAsync();
        
        return RedirectToPage("Index");
    }
}