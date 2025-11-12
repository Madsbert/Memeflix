using System.Security.Claims;
using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Memeflix._External.API;

[ApiController]
[Route("api/[controller]")]
public class APIController : ControllerBase
{
    public readonly IMovieService _movieService;
    public readonly ILoginService _loginService;

    public APIController(IMovieService movieService, ILoginService loginService)
    {
        _movieService = movieService;
        _loginService = loginService;
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

    [HttpGet("getMovieMetaData/{fileId}")]
    public async Task<IActionResult> GetMovieMetaData(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId id))
            return BadRequest(new { message = "Invalid file Id" });

        try
        {
            var metadata = await _movieService.GetMovieMetadataAsync(id);
            if (metadata == null)
                return NotFound(new { message = "File not found" });

            return Ok(metadata);
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
            return BadRequest(new { message = "Invalid file Id" });

        try
        {
            var stream = await _movieService.DownloadMovieStreamAsync(id);
            if (stream == null)
                return NotFound(new { message = "File not found" });


            return File(stream, "video/mp4", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetVideoList()
    {
        try
        {
            var files = await _movieService.GetAllMovies();
            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error retrieving video list: {ex.Message}" });
        }
    }

    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            await _loginService.RegisterAsync(request.Username, request.Password);
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error creating user: {ex.Message}" });
        }
    }

    public record CreateUserRequest(string Username, string Password);


    [HttpGet("login")]
    public async Task<IActionResult> TestAuth(string Username, string Password)
    {
        var user = new User(Username, Password);

        if (await _loginService.LoginAsync(user.Username, user.Password))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username)
            };
            var identity = new ClaimsIdentity(claims, "User");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
            // Return success json response
            return Ok(new { message = "Login successful" });
        }
        return BadRequest(new { message = "Invalid username or password" });
    }

    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok(new { message = "API is running" });
    }
}