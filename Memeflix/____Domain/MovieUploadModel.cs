using System.ComponentModel.DataAnnotations;

namespace Memeflix.____Domain;
/// <summary>
/// a domain class for the movieupload - the informatino needed to import a movie to the database
/// </summary>
public class MovieUploadModel
{
    [Required]
    public IFormFile Moviefile { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Genre { get; set; }
    [Required]
    public string Description { get; set; }
}