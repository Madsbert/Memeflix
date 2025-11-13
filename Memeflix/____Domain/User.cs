
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Memeflix.____Domain;

/// <summary>
/// User entity representing a user in the system.
/// </summary>
public class User
{
    [BsonId]
    public ObjectId _id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Roles = new List<string>();
    }
}