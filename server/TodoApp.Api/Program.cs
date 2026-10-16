using Microsoft.EntityFrameworkCore;
using TodoApp.DataAccess;
using TodoApp.DataAccess.Repositories;
using TodoApp.Interfaces.Options;
using TodoApp.Interfaces.Repositories;
using TodoApp.Interfaces.Services;
using TodoApp.Services;

// Program.cs is the entry point and the "composition root" of the application:
// here we register services in the DI container and configure the HTTP pipeline.
var builder = WebApplication.CreateBuilder(args);

// ---- Services (DI container) ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

// "When someone asks for the interface, create this class."
// Scoped = one instance per HTTP request (same lifetime as DbContext).
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();

var app = builder.Build();

// ---- HTTP request pipeline (middleware order matters) ----
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
