using System.Runtime.InteropServices.JavaScript;
using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Gateway;
using MongoDB.Bson;

namespace Memeflix.___Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepo _movieRepo;

    public MovieService(IMovieRepo movieRepo)
    {
        _movieRepo = movieRepo;
    }

    public async Task<ObjectId> UploadMovieAsync(MovieUploadModel file)
    {
        var movieFile = file.Moviefile; 
        if (movieFile == null || movieFile.Length == 0)
        {
            throw new ArgumentNullException(nameof(file));
        }
        
        using var stream = movieFile.OpenReadStream();
        var metadata = new MovieMetadata(
            index: ObjectId.GenerateNewId().ToString(),
            filename: movieFile.FileName,
            uploadDate: DateTime.Now,
            description: file.Description,
            duration: 0,
            genre: file.Genre,
            chunkSize: 255 * 1024,
            metadata: new Dictionary<string, object>()
        );
        
        return await _movieRepo.UploadFileAsync(file.Title, stream, metadata);
    }

    public Task<ObjectId> UploadMovieAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> DownloadMovieAsync(ObjectId fileId)
    {
        throw new NotImplementedException();
    }
}