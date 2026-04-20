using Microsoft.EntityFrameworkCore;
using Technical_Challenge.Domain;

namespace Technical_Challenge.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}