using Memeflix.____Domain;
using Memeflix.__Gateway;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Memeflix._External;

/// <summary>
/// MonogoDb Connection and REST methods
/// </summary>
public class MovieRepo : IMovieRepo
{
    private readonly IMongoDatabase _database;
    private readonly IGridFSBucket _gridFSBucket;

    public MovieRepo(IMongoDatabase database)
    {
        _database = database;
        _gridFSBucket = new GridFSBucket(database);
    }

    /// <summary>
    /// Uploads a file to GridFS storage with associated movie metadata
    /// </summary>
    /// <param name="filename">The name to assign to the uploaded file</param>
    /// <param name="stream">Stream containing the file data to be uploaded</param>
    /// <param name="metadata">Movie metadata containing descriptive information and file details</param>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Creates GridFS upload options with movie metadata converted to BSON format
    /// 2. Uploads the file stream to GridFS bucket storage
    /// 3. Returns the unique identifier assigned by GridFS to the uploaded file
    /// The metadata is stored as part of the GridFS file document for easy querying and retrieval.
    /// </remarks>
    public async Task<ObjectId> UploadFileAsync(string filename, Stream stream, MovieMetadata metadata)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "description", metadata.Description },
                { "genre", metadata.Genre },
                { "uploadDate", metadata.UploadDate },
                { "filename", metadata.FileName },
                { "title", metadata.Title },
                { "duration", metadata.Size },
            },
        };
        return await _gridFSBucket.UploadFromStreamAsync(filename, stream, options);
    }

    /// <summary>
    /// Opens a download stream for a specific movie file stored in GridFS
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<Stream> OpenDownloadStreamAsync(ObjectId fileId)
    {
        var downloadStream = await _gridFSBucket.OpenDownloadStreamAsync(fileId, new GridFSDownloadOptions
        {
            Seekable = true
        });
        return downloadStream;
    }

    /// <summary>
    /// Retrieves metadata for a specific file stored in GridFS
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<MovieMetadata> GetFileMetadataAsync(ObjectId fileId)
    {
        var fileInfo = await GetFileInfoAsync(fileId);
        if (fileInfo == null)
            return null;

        return new MovieMetadata(
            index: fileInfo.Id.ToString(),
            filename: fileInfo.Filename,
            title: fileInfo.Metadata?.GetValue("title", "No title")?.ToString() ?? "No title",
            uploadDate: fileInfo.UploadDateTime,
            description: fileInfo.Metadata?.GetValue("description", "No description")?.ToString() ?? "No description",
            size: fileInfo.Metadata?.GetValue("duration", 0)?.ToInt32() ?? 0,
            genre: fileInfo.Metadata?.GetValue("genre", "Unknown")?.ToString(),
            chunkSize: fileInfo.ChunkSizeBytes,
            metadata: fileInfo.Metadata?.ToDictionary() ?? new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Retrieves metadata for all files stored in GridFS
    /// </summary>
    /// <returns></returns>
    public async Task<List<MovieMetadata>> GetAllFilesAsync()
    {
        List<GridFSFileInfo> files = await _gridFSBucket.Find(FilterDefinition<GridFSFileInfo>.Empty)
            .ToListAsync();

        var result = files.Select(f => new MovieMetadata(
            index: f.Id.ToString(),
            filename: f.Filename,
            title: f.Metadata?.GetValue("title", "No title")?.ToString() ?? "No title",
            uploadDate: f.UploadDateTime,
            description: f.Metadata?.GetValue("description", "No description")?.ToString() ?? "No description",
            size: f.Metadata?.GetValue("duration", 0)?.ToInt32() ?? 0,
            genre: f.Metadata?.GetValue("genre", "Unknown")?.ToString(),
            chunkSize: f.ChunkSizeBytes,
            metadata: f.Metadata?.ToDictionary() ?? new Dictionary<string, object>()
        )).ToList();

        return result;
    }

    /// <summary>
    /// Get file information (metadata)
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
        var fileInfo = await _gridFSBucket.Find(filter).FirstOrDefaultAsync();
        return fileInfo;
    }
}