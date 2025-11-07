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
            }
        };
        return await _gridFSBucket.UploadFromStreamAsync(filename, stream, options);
    }
    
    public async Task<Stream> OpenDownloadStreamAsync(ObjectId fileId)
    {
        var downloadStream = await _gridFSBucket.OpenDownloadStreamAsync(fileId);
        return downloadStream;
    }
    
    
    public Task<Stream> DownloadFileAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }

    public Task<MovieMetadata> GetFileMetadataAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieMetadata>> GetAllFilesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistsAsync(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetFileSizeAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }
    
    // Get file information (metadata)
    public async Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
        var fileInfo = await _gridFSBucket.Find(filter).FirstOrDefaultAsync();
        return fileInfo;
    }
    
}