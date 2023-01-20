using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Pizza
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    [MinLength(1)] [MaxLength(128)]
    public string Description { get; set; } = default!;
    
    public double Price { get; set; }
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public ICollection<PurchasedPizza>? PurchasedPizzas { get; set; }
    
    public ICollection<PizzaExtra>? PizzaExtras { get; set; }
}