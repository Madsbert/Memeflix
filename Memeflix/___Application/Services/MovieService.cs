using System.Runtime.InteropServices.JavaScript;
using Memeflix.____Domain;
using Memeflix.___Application.Interfaces;
using Memeflix.__Gateway;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace Memeflix.___Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepo _movieRepo;

    public MovieService(IMovieRepo movieRepo)
    {
        _movieRepo = movieRepo;
    }

    /// <summary>
    /// Uploads a movie file to the repository with associated metadata
    /// </summary>
    /// <param name="file">Movie upload model containing the movie file and metadata</param>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates the input movie file
    /// 2. Creates movie metadata 
    /// 3. Opens a read stream for the movie file
    /// 4. Delegates the actual file upload to the movie repository
    /// </remarks>
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
        // Delegate the actual file upload to the repository layer
        // Returns the ObjectId assigned to the uploaded movie
        return await _movieRepo.UploadFileAsync(movieFile.FileName, stream, metadata);
    }

    public async Task<Stream> DownloadMovieStreamAsync(ObjectId fileId)
    {
        if (fileId == ObjectId.Empty)
            throw new ArgumentException("Invalid fileId", nameof(fileId));

        return await _movieRepo.OpenDownloadStreamAsync(fileId);
    }

    public async Task<List<MovieMetadata>> GetAllMovies()
    {
        return await _movieRepo.GetAllFilesAsync();
    }
}