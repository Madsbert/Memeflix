using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Gateway;
using Microsoft.AspNetCore.Mvc;

namespace Memeflix._External.API;

[ApiController]
[Route("api/[controller]")]
public class APIController :ControllerBase
{
    
    public readonly IMovieService _movieService;
    
    public APIController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost("uploadMovie")]
    public async Task<IActionResult> UploadMovieAsync([FromForm] MovieUploadModel file)
    {
        if (file == null || file.Moviefile.Length == 0)
        {
           return BadRequest("No file provided");
        }
        
        var fileId = await _movieService.UploadMovieAsync(file.Moviefile);
        return Ok(fileId);
    }
    
}