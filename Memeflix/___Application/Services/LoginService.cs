
using Memeflix.____Domain;
using Memeflix.__Application.Interfaces;
using Memeflix.__Gateway;

namespace Memeflix.__Application.Services;

/// <summary>
/// Service for handling user login and registration
/// </summary>
public class LoginService : ILoginService
{
    private readonly IUserRepo _userRepo;

    public LoginService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    /// <summary>
    /// Logs in a user by verifying credentials
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<bool> LoginAsync(string username, string password)
    {
        // Actually implement login logic here
        var user = new User(username, password);
        return await _userRepo.AuthenticateAsync(user);
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> RegisterAsync(string username, string password)
    {
        var user = new User(username, password);

        if (user == null || user.Username.Count() == 0)
        {
            throw new ArgumentNullException(nameof(user), "User or username cannot be null or empty");
        }
        await _userRepo.CreateUserAsync(user);
        return true;
    }
}