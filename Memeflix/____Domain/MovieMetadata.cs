using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Memeflix.____Domain;
/// <summary>
/// a domain class for det moviemeta data 
/// </summary>
public class MovieMetadata
{
    [field: BsonId]
    [field: BsonRepresentation(BsonType.ObjectId)]
    public string Index { get; set; }
    
    public string FileName { get; set; }

    public DateTime UploadDate { get; set; }


    public string Description { get; set; }

    public long Duration { get; set; }

    public Genre Genre { get; set; }

    public int ChunkSize { get; set; }

    public Dictionary<string, object> Metadata { get; set; }

    public MovieMetadata(string index, string filename, DateTime uploadDate, string description, long duration, Genre genre, int chunkSize, Dictionary<string, object> metadata)
    {
        Index = index;
        FileName = filename;
        UploadDate = uploadDate;
        Description = description;
        Duration = duration;
        Genre = genre;
        ChunkSize = chunkSize;
        Metadata = metadata;
    }
}