using Memeflix.____Domain;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

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

    Task<List<MovieMetadata>> GetAllMovies();
    

}