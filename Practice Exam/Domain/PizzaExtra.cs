namespace Domain;

public class PizzaExtra
{
    public int Id { get; set; }

    public int PizzaId { get; set; }
    public Pizza? Pizza { get; set; }
    
    public int ExtraId { get; set; }
    public Extra? Extra { get; set; }
    
    public int PurchasedPizzaId { get; set; }
    public PurchasedPizza? PurchasedPizza { get; set; }
}