
using System.Text.Json;
using Memeflix.____Domain;
using Memeflix.__Gateway;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Memeflix._External;

public class UserRepo : IUserRepo
{
    private readonly IMongoDatabase _database;

    public UserRepo(IMongoDatabase database)
    {
        _database = database;
    }

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

    public async Task<bool> AuthenticateAsync(User user)
    {
        var collection = _database.GetCollection<User>("Users");
        var filter = Builders<User>.Filter.Eq(u => u.Username, user.Username) &
                     Builders<User>.Filter.Eq(u => u.Password, user.Password);
        var foundUser = await collection.Find(filter).FirstOrDefaultAsync();
        return foundUser != null;
    }
}