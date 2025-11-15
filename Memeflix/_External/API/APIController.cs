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

/// <summary>
/// API Controller for handling movie and user-related endpoints
/// </summary>
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

    /// <summary>
    /// Uploads a movie file along with its metadata
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets metadata for a specific movie file
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets AI-based movie recommendations based on title and genre
    /// </summary>
    /// <param name="title"></param>
    /// <param name="genre"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("getAIRecommendation/{title}/{genre}")]
    public async Task<IActionResult> GetAIRecommendation(string title, string genre)
    {

        var ollamaClient = new OllamaApiClient("http://localhost:11434", "gemma3:1b");
        try
        {
            var prompt = $"Provide 3 very brief movie recommendations similar to '{title}' in the genre '{genre}'.\n" +
                          "If you don't know the movie, provide general recommendations for that genre.\n" +
                          "Format each recommendation EXACTLY as follows:\n" +
                          "MOVIE_TITLE • RELEASE_YEAR • GENRE1 / GENRE2 • LEAD_ACTOR as LEAD_ROLE\n" +
                          "MOVIE_TITLE • RELEASE_YEAR • GENRE1 / GENRE2 • LEAD_ACTOR as LEAD_ROLE\n" +
                          "MOVIE_TITLE • RELEASE_YEAR • GENRE1 / GENRE2 • LEAD_ACTOR as LEAD_ROLE\n" +
                          "Use only two genre per movie. Use the dot symbol • as separator.\n" +
                          "Do not include any numbers, bullet points, additional text, or explanations.\n" +
                          "Only output the 3 lines in the specified format.";


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

    /// <summary>
    /// Downloads a movie file stream from the repository
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets a list of all video files with metadata
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Tests user authentication and issues an auth cookie on success
    /// </summary>
    /// <param name="Username"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns></returns>
    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok(new { message = "API is running" });
    }

    /// <summary>
    /// Logs out the current user by removing the auth cookie
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies"); // removes the auth cookie
        return Ok(new { message = "User logged out" });
    }
}