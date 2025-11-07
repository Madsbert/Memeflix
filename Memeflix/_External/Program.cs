using Memeflix.___Application.Interfaces;
using Memeflix.___Application.Services;
using Memeflix.__Gateway;
using Memeflix._External;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Configuration
var mongoConn = builder.Configuration.GetConnectionString("MongoDB");
var mongoClient = new MongoClient(mongoConn);
var mongoDatabase = mongoClient.GetDatabase("MemeflixDb");
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(mongoDatabase);
builder.Services.AddSingleton<IGridFSBucket>(sp => new GridFSBucket(sp.GetRequiredService<IMongoDatabase>()));

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRepo, MovieRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();