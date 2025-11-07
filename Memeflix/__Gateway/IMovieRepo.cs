using Memeflix.____Domain;
using MongoDB.Bson;

namespace Memeflix.__Gateway;
/// <summary>
/// interface for the movie repo
/// </summary>
public interface IMovieRepo
{
    /// <summary>
    /// Uploads a file to GridFS storage with associated movie metadata
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    Task<ObjectId> UploadFileAsync(string filename, Stream stream, MovieMetadata metadata);
    Task<Stream> DownloadFileAsync(ObjectId fileId);
    Task<Stream> OpenDownloadStreamAsync(ObjectId fileId);
    Task<MovieMetadata> GetFileMetadataAsync(ObjectId fileId);
    Task<IEnumerable<MovieMetadata>> GetAllFilesAsync();
    Task<bool> DeleteFileAsync(ObjectId fileId);
    Task<bool> FileExistsAsync(string filename);
    Task<long> GetFileSizeAsync(ObjectId fileId);
    
    
}