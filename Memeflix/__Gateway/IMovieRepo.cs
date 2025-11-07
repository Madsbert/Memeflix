using Memeflix.____Domain;
using MongoDB.Bson;

namespace Memeflix.__Gateway;

public interface IMovieRepo
{
    Task<ObjectId> UploadFileAsync(string filename, Stream stream, MovieMetadata metadata);
    Task<Stream> DownloadFileAsync(ObjectId fileId);
    Task<MovieMetadata> GetFileMetadataAsync(ObjectId fileId);
    Task<IEnumerable<MovieMetadata>> GetAllFilesAsync();
    Task<bool> DeleteFileAsync(ObjectId fileId);
    Task<bool> FileExistsAsync(string filename);
    Task<long> GetFileSizeAsync(ObjectId fileId);
    
    
}