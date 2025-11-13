namespace Memeflix.____Domain;

/// <summary>
/// Request model for creating a new user
/// </summary>
/// <param name="Username"></param>
/// <param name="Password"></param>
public record CreateUserRequest(string Username, string Password);