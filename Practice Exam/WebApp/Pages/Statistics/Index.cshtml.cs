using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Statistics;

public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;

    public Index(DAL.AppDbContext context)
    {
        _context = context;
    }

    public double TotalProfit { get; set; }
    
    public int PendingOrders { get; set; }
    public int FinalizedOrders { get; set; }

    public string TopBuyer { get; set; } = default!;
    
    public void OnGet()
    {
        TotalProfit = _context.Purchases.Where(purchase => purchase.Finalized).Sum(purchase => purchase.TotalPrice);
        
        PendingOrders = _context.Purchases.Count(purchase => !purchase.Finalized);
        FinalizedOrders = _context.Purchases.Count(purchase => purchase.Finalized);

        if (FinalizedOrders > 0)
        {
            TopBuyer = _context.Purchases.AsEnumerable()
                .Where(p => p.Finalized)
                .GroupBy(p => p.ClientGUID).MaxBy(g => g.Count())!
                .Key;
        }
        else
        {
            TopBuyer = "-";
        }
    }
}