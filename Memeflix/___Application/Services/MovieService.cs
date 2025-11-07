using Memeflix.___Application.Interfaces;
using MongoDB.Bson;

namespace Memeflix.___Application.Services;

public class MovieService : IMovieService
{
    public Task<ObjectId> UploadMovieAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> DownloadMovieAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }
}