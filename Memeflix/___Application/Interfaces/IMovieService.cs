using Memeflix.____Domain;
using MongoDB.Bson;

namespace Memeflix.___Application.Interfaces;

public interface IMovieService
{
    Task<ObjectId> UploadMovieAsync(MovieUploadModel file);
    Task<byte[]> DownloadMovieAsync(ObjectId fileId);
    
}