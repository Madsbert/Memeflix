using Memeflix.____Domain;
using MongoDB.Bson;

namespace Memeflix.___Application.Interfaces;
/// <summary>
/// interface class for imovieservice
/// </summary>
public interface IMovieService
{
    
    /// <summary>
    /// Uploads a movie file to the repository with associated metadata
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<ObjectId> UploadMovieAsync(MovieUploadModel file);
    Task<Stream> DownloadMovieStreamAsync(ObjectId fileId);
    
}