namespace Memeflix.__Application.Interfaces;

public interface ILoginService
{
    // Define login-related business logic methods here
    public Task<bool> LoginAsync(string username, string password);
    public Task<bool> RegisterAsync(string username, string password);
}