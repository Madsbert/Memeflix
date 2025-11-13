
using System.Text.Json;
using Memeflix.____Domain;
using Memeflix.__Gateway;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Memeflix._External;

/// <summary>
/// Repository implementation for user-related operations
/// </summary>
public class UserRepo : IUserRepo
{
    private readonly IMongoDatabase _database;

    public UserRepo(IMongoDatabase database)
    {
        _database = database;
    }

    /// <summary>
    /// Creates a new user in the database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ObjectId> CreateUserAsync(User user)
    {
        var collection = _database.GetCollection<BsonDocument>("Users");
        var document = new BsonDocument()
        {
            { "Username", user.Username },
            { "Password", user.Password },
            { "Roles", new BsonArray(user.Roles) }
        };
        await collection.InsertOneAsync(document);
        return user._id;
    }

    /// <summary>
    /// Authenticates a user by verifying username and password
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<bool> AuthenticateAsync(User user)
    {
        var collection = _database.GetCollection<User>("Users");
        var filter = Builders<User>.Filter.Eq(u => u.Username, user.Username) &
                     Builders<User>.Filter.Eq(u => u.Password, user.Password);
        var foundUser = await collection.Find(filter).FirstOrDefaultAsync();
        return foundUser != null;
    }

}