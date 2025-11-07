using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Gateway;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        try
        {
            var fileId = await _movieService.UploadMovieAsync(file);
            return Ok(new { FileId = fileId.ToString() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("downloadMovie/{fileId}")]
    public async Task<IActionResult> DownloadMovieAsync(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId id))
            return BadRequest("Invalid file Id");

        try
        {
            var stream = await _movieService.DownloadMovieStreamAsync(id);
            if (stream == null)
                return NotFound("File not found");

           
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}