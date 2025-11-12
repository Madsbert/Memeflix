using Memeflix.___Application.Interfaces;
using Memeflix.___Application.Services;
using Memeflix.__Gateway;
using Memeflix._External;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http.Features;
using Memeflix.__Application.Interfaces;
using Memeflix.__Application.Services;

var builder = WebApplication.CreateBuilder(args);


const String AuthScheme = "token";

builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme, Options =>
    {
        Options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        Options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("user", pb =>
    {
        pb.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthScheme)
        .AddRequirements()
        .RequireClaim("user_type", "standard");
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure maximum request body size for file uploads
// Example: 524_288_000 = 500 MB
long maxUploadBytes = 4_194_304_000; // 4 GB

// Kestrel server limit (applies when using Kestrel directly)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxUploadBytes;
});

// Multipart form options (applies to IFormFile multipart uploads)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxUploadBytes; // total body size
});

// MongoDB Configuration
var mongoConn = builder.Configuration.GetConnectionString("MongoDB");
var mongoClient = new MongoClient(mongoConn);
var mongoDatabase = mongoClient.GetDatabase("MemeflixDb");
// Register MongoDB services with dependency injection container
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(mongoDatabase);

// Register GridFS bucket for file storage operations
builder.Services.AddSingleton<IGridFSBucket>(sp => new GridFSBucket(sp.GetRequiredService<IMongoDatabase>()));

// Register application-specific services with scoped lifetime
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRepo, MovieRepo>();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();

var app = builder.Build();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files (HTML, CSS, JS)
app.UseStaticFiles();

app.UseDefaultFiles();

// Fallback to serve index.html for root URL
app.MapFallbackToFile("index.html");
app.MapControllers();

app.Run();