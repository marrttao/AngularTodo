using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Interfaces.Entities;

namespace TodoApp.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);

        // We always save UTC, but SQL Server's datetime2 doesn't store the "kind" of a DateTime.
        // This converter marks values read from the database as UTC, so JSON gets a "Z" suffix.
        builder.Property(u => u.CreatedAt)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        // Two users cannot register with the same email.
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
