namespace Memeflix.____Domain;

public class MovieUploadModel
{
    public IFormFile Moviefile{get;set;}
    public string Title{get;set;}
    public Genre Genre{get;set;}
    
    public string Description{get;set;}

    public MovieUploadModel(IFormFile moviefile, string title, Genre genre,  string description)
    {
        Moviefile = moviefile;
        Title = title;
        Genre = genre;
        Description = description;
        
    }
}