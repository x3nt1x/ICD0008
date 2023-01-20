namespace MenuSystem;

public class Option
{
    public string Title { get; set; }
    public Func<string>? Method { get; set; }
    
    public Option(string title, Func<string>? method = null)
    {
        Title = title;
        Method = method;
    }
    
    public override string ToString() => Title;
}