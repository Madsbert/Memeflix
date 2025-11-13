
using Memeflix.____Domain;
using MongoDB.Bson;

namespace Memeflix.__Gateway;

public interface IUserRepo
{
    // Define user-related data access methods here
    public Task<ObjectId> CreateUserAsync(User user);
    public Task<bool> AuthenticateAsync(User user);

}