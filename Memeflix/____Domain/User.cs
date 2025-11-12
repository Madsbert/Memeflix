
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Memeflix.____Domain;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; }

    public User(string username, string password)
    {
        Id = ObjectId.GenerateNewId();
        Username = username;
        Password = password;
        Roles = new List<string>();
    }
}