using Memeflix.____Domain;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

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
    Task<Stream> OpenDownloadStreamAsync(ObjectId fileId);
    Task<MovieMetadata> GetFileMetadataAsync(ObjectId fileId);
    Task<List<MovieMetadata>> GetAllFilesAsync();
}