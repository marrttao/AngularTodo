using Microsoft.EntityFrameworkCore;
using TodoApp.Interfaces.Entities;

namespace TodoApp.DataAccess;

/// <summary>
/// EF Core database context: a session with the database.
/// Each DbSet represents a table.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TodoItem> Tasks => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up all IEntityTypeConfiguration<T> classes from the Configurations folder.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
