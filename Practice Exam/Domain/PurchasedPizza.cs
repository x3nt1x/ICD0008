namespace Domain;

public class PurchasedPizza
{
    public int Id { get; set; }

    public int PizzaId { get; set; }
    public Pizza? Pizza { get; set; }
    
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    
    public ICollection<PizzaExtra>? PizzaExtras { get; set; }
}