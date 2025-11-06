namespace Memeflix.____Domain;

public class Movie
{
    private string _index;
    private string _title;
    private string _date;
    private string _description;
    private int _duration;
    private Genre _genre;

    public string Index { get; set; }
    public string Title { get; set; }
    public string Date { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public Genre Genre { get; set; }
    
    
    public Movie(string index, string title, string date, string description, int duration, Genre genre)
    {
        _index = index;
        _title = title;
        _date = date;
        _description = description;
        _duration = duration;
        _genre = genre;
    }
    
    
}