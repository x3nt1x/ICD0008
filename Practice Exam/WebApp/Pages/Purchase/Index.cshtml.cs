using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Purchase;

public class IndexModel : PageModel
{
    private readonly DAL.AppDbContext _context;

    public IndexModel(DAL.AppDbContext context)
    {
        _context = context;
    }

    public IList<Domain.PurchasedPizza> PurchasedPizzas { get; set; } = default!;
    public IList<Domain.Extra> Extras { get; set; } = default!;

    public Domain.Purchase? Purchase { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var uid = Request.Cookies["uid"];
        if (uid == null)
            return Page();
        
        Purchase = _context.Purchases.FirstOrDefault(purchase => purchase.ClientGUID == uid && !purchase.Finalized);
        if (Purchase == null)
            return Page();

        PurchasedPizzas = await _context.PurchasedPizzas
            .Include(purchasedPizza => purchasedPizza.Pizza)
            .ThenInclude(pizza => pizza!.Category)
            .Where(purchasedPizza => purchasedPizza.PurchaseId == Purchase.Id).ToListAsync();

        Extras = await _context.Extras.Include(extra => extra.PizzaExtras).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPost(int? id, int? finalize, string? extra)
    {
        var uid = Request.Cookies["uid"];
        if (uid == null)
            return RedirectToPage("Index");

        var purchase = _context.Purchases
            .Include(purchase => purchase.PurchasedPizzas)
            .FirstOrDefault(purchase => purchase.ClientGUID == uid && !purchase.Finalized);

        if (purchase == null)
            return RedirectToPage("Index");

        if (finalize != null)
        {
            purchase.Finalized = true;

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        
        int? purchasedPizzaId = id ?? null;
        int? extraId = null;
        
        var split = extra?.Split("-");
        if (split != null)
        {
            purchasedPizzaId = Convert.ToInt32(split[0]);
            extraId = Convert.ToInt32(split[1]);
        }
        
        var purchasedPizza = _context.PurchasedPizzas.FirstOrDefault(purchasedPizza => purchasedPizza.Id == purchasedPizzaId);
        if (purchasedPizza == null)
            return RedirectToPage("Index");

        var pizza = _context.Pizzas
            .Include(pizza => pizza.PizzaExtras)
            .FirstOrDefault(pizza => pizza.Id == purchasedPizza.PizzaId);
        
        if (pizza == null)
            return RedirectToPage("Index");

        if (extraId != null)
        {
            var selectedExtra = _context.Extras.FirstOrDefault(thisExtra => thisExtra.Id == extraId);
            if (selectedExtra == null)
                return RedirectToPage("Index");
            
            pizza.PizzaExtras ??= new List<Domain.PizzaExtra>();

            var pizzaExtra = _context.PizzaExtras
                .FirstOrDefault(pizzaExtra => pizzaExtra.PurchasedPizzaId == purchasedPizzaId && pizzaExtra.ExtraId == extraId);
            
            if (pizzaExtra != null)
            {
                pizza.PizzaExtras.Remove(pizzaExtra);
                purchase.TotalPrice -= selectedExtra.Price;

                await _context.SaveChangesAsync();
            
                return RedirectToPage("Index");
            }
            
            var newPizzaExtra = new Domain.PizzaExtra
            {
                Pizza = pizza,
                Extra = selectedExtra,
                PurchasedPizza = purchasedPizza
            };
            
            pizza.PizzaExtras.Add(newPizzaExtra);
            purchase.TotalPrice += selectedExtra.Price;
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage("Index");
        }

        if (purchase.PurchasedPizzas == null)
            return RedirectToPage("Index");

        var pizzaExtras = _context.PizzaExtras
            .Where(pizzaExtra => pizzaExtra.PurchasedPizzaId == purchasedPizza.Id)
            .Sum(pizzaExtra => pizzaExtra.Extra!.Price);
        
        purchase.PurchasedPizzas.Remove(purchasedPizza);
        purchase.TotalPrice -= pizza.Price + pizzaExtras;
        
        if (!purchase.PurchasedPizzas.Any())
            _context.Purchases.Remove(purchase);
        
        await _context.SaveChangesAsync();
        
        return RedirectToPage("Index");
    }
}