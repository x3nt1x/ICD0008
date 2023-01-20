using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Extra
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;

    public double Price { get; set; }

    public ICollection<PizzaExtra>? PizzaExtras { get; set; }
}