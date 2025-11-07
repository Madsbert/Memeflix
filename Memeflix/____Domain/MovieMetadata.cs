using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Memeflix.____Domain;

public class MovieMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    private string _index;
    
    private string _filename;
    private DateTime _uploadDate;
    private string _description;
    private long _duration;
    private Genre _genre;
    private int _chunkSize;
    private Dictionary<string,object> _metadata;
    public string Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public string FileName
    {
        get { return _filename; }
        set { _filename = value; }
    }

    public DateTime UploadDate
    {
        get { return _uploadDate; }
        set { _uploadDate = value; }
    }

    
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public long Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    public Genre Genre
    {
        get { return _genre; }
        set { _genre = value; }
    }

    public int ChunkSize
    {
        get { return _chunkSize; }
        set { _chunkSize = value; }
    }

    public Dictionary<string, object> Metadata
    {
        get { return _metadata; }
        set { _metadata = value; }
    }

    public MovieMetadata(string index, string filename, DateTime uploadDate, string description, long duration, Genre genre, int chunkSize, Dictionary<string, object> metadata)
    {
        _index = index;
        _filename = filename;
        _uploadDate = uploadDate;
        _description = description;
        _duration = duration;
        _genre = genre;
        _chunkSize = chunkSize;
        _metadata = metadata;
    }
}