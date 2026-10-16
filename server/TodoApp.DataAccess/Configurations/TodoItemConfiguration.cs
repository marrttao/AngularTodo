using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Interfaces.Entities;

namespace TodoApp.DataAccess.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);

        // One user -> many tasks. Deleting a user deletes their tasks.
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One category -> many tasks. Deleting a category must keep its tasks
        // and only clear their CategoryId.
        // We can't use DeleteBehavior.SetNull here: SQL Server rejects it because a task
        // would then have two cascade paths from Users (Users -> Tasks and Users -> Categories -> Tasks).
        // ClientSetNull = no action in the database; EF Core sets CategoryId to null
        // for tasks that are loaded into the context (see CategoryRepository.DeleteAsync).
        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Speeds up the most common query: "tasks of user X".
        builder.HasIndex(t => new { t.UserId, t.IsCompleted });
    }
}
