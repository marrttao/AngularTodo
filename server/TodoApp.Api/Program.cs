using Microsoft.EntityFrameworkCore;
using TodoApp.DataAccess;

// Program.cs is the entry point and the "composition root" of the application:
// here we register services in the DI container and configure the HTTP pipeline.
var builder = WebApplication.CreateBuilder(args);

// ---- Services (DI container) ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

// ---- HTTP request pipeline (middleware order matters) ----
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
