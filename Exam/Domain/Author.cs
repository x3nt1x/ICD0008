using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Author
{
    public int Id { get; set; }

    [MinLength(1)] [MaxLength(32)]
    public string Name { get; set; } = default!;
    
    public double Price { get; set; }
    
    public ICollection<Song>? Songs { get; set; }
    public ICollection<SongPlayed>? SongsPlayed { get; set; }
}