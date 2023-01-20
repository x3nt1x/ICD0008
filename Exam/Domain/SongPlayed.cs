namespace Domain;

public class SongPlayed
{
    public int Id { get; set; }

    public int DjId { get; set; }
    public Dj? Dj { get; set; }
    
    public int SongId { get; set; }
    public Song? Song { get; set; }
    
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}