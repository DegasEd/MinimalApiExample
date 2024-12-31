using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApiSwagger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieDb>(opt => opt.UseInMemoryDatabase("MovieDbList"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movie API",
        Description = "API for managing a list of movies and their active status.",
        TermsOfService = new Uri("https://example.com/terms")
    });
});

var app = builder.Build();

app.MapGet("/movieitems", async (MovieDb db) =>
    await db.Movies.ToListAsync());

app.MapGet("/movieitems/active", async (MovieDb db) =>
    await db.Movies.Where(t => t.IsActive).ToListAsync());

app.MapGet("/movieitems/{id}", async (int id, MovieDb db) =>
    await db.Movies.FindAsync(id)
        is Movie movie
            ? Results.Ok(movie)
            : Results.NotFound());

app.MapPost("/movieitems", async (Movie movie, MovieDb db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    return Results.Created($"/movieitems/{movie.Id}", movie);
});

app.MapPut("/movieitems/{id}", async (int id, Movie inputMovie, MovieDb db) =>
{
    var movie = await db.Movies.FindAsync(id);

    if (movie is null) return Results.NotFound();

    movie.Name = inputMovie.Name;
    movie.Gender = inputMovie.Gender;
    movie.IsActive = inputMovie.IsActive;

    await db.SaveChangesAsync();

    return movie.IsActive 
        ? Results.Ok(movie) 
        : Results.NoContent();
});

app.MapDelete("/movieitems/{id}", async (int id, MovieDb db) =>
{
    if (await db.Movies.FindAsync(id) is Movie movie)
    {
        db.Movies.Remove(movie);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();