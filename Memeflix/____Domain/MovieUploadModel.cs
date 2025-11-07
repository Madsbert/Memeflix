using System.ComponentModel.DataAnnotations;

namespace Memeflix.____Domain;

public class MovieUploadModel
{
    [Required]
    public IFormFile Moviefile{get;set;}
    [Required]
    public string Title{get;set;}
    [Required]
    public Genre Genre{get;set;}
    [Required]
    public string Description { get; set; }
}