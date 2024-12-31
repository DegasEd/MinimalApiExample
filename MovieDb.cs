using Microsoft.EntityFrameworkCore;

namespace MinimalApiSwagger;

public class MovieDb : DbContext
{
    public MovieDb(DbContextOptions<MovieDb> options)
    : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();

}
