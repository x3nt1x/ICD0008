using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Song
{
    public int Id { get; set; }
    
    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    public int Length { get; set; }
    public int TimesPlayed { get; set; }
    public double Price { get; set; }
    
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}