using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Purchase
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(64)]
    public string ClientGUID { get; set; } = default!;

    public double TotalPrice { get; set; }

    public bool Finalized { get; set; }

    public ICollection<PurchasedPizza>? PurchasedPizzas { get; set; }
}