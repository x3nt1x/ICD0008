using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Dj
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    public double Price { get; set; }
    public bool Performs { get; set; }
    
    public ICollection<SongPlayed>? SongsPlayed { get; set; }
}