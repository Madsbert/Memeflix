using System.Security.Claims;
using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OllamaSharp;

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
    [Authorize]
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

    [Authorize]
    [HttpGet("getAIRecommendation/{title}/{genre}")]
    public async Task<IActionResult> GetAIRecommendation(string title, string genre)
    {

        var ollamaClient = new OllamaApiClient("http://localhost:11434", "gemma3:1b");
        try
        {
            var prompt = $"Provide 3 very brief movie recommendations for another movie like the movie '{title}' in the genre '{genre}'." +
            $"\nIf you don't know the movie, just provide general recommendations for that genre.\n Make the recommendations concise and to the point." +
            $"\nFormat the recommendations as a numbered list." + "Do not include any additional commentary or questions. Just the list." +
            "\nExample:\\n1. Movie One (RELEASE_YEAR), (2 GENRES) - (DIRECTOR)\\n2. Movie Two (RELEASE_YEAR), (2 GENRES) - (DIRECTOR)\\n3. Movie Three (RELEASE_YEAR), (2 GENRES) - DIRECTOR\n";


            var chat = new Chat(ollamaClient);
            string response = "";
            await foreach (var answer in chat.SendAsync(prompt, null, null, null))
            {
                response += answer;
            }

            return Ok(new { recommendation = response.Trim() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }
    }

    [Authorize]
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
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }
    }

    [Authorize]
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

            await HttpContext.SignInAsync("Cookies", principal);
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

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies"); // removes the auth cookie
        return Ok(new { message = "User logged out" });
    }
}