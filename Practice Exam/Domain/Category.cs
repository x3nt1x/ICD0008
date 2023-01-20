using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Category
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    public ICollection<Pizza>? Pizzas { get; set; }
}