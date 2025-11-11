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

    public string Title { get; set; }

    public DateTime UploadDate { get; set; }

    public string Description { get; set; }

    public long Duration { get; set; }

    public string Genre { get; set; }

    public int ChunkSize { get; set; }

    public Dictionary<string, object> Metadata { get; set; }

    public MovieMetadata(string index, string filename, string title, DateTime uploadDate, string description, long duration, string genre, int chunkSize, Dictionary<string, object> metadata)
    {
        Index = index;
        FileName = filename;
        Title = title;
        UploadDate = uploadDate;
        Description = description;
        Duration = duration;
        Genre = genre;
        ChunkSize = chunkSize;
        Metadata = metadata;
    }
}