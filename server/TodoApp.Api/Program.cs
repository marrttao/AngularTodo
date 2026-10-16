using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TodoApp.Api.Middleware;
using TodoApp.DataAccess;
using TodoApp.DataAccess.Repositories;
using TodoApp.Interfaces.Options;
using TodoApp.Interfaces.Repositories;
using TodoApp.Interfaces.Services;
using TodoApp.Services;

// Program.cs is the entry point and the "composition root" of the application:
// here we register services in the DI container and configure the HTTP pipeline.
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Services (DI container)
// ---------------------------------------------------------------------------

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// "When someone asks for the interface, create this class."
// Scoped = one instance per HTTP request (same lifetime as DbContext).
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// JWT authentication
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("The 'Jwt' configuration section is missing.");

if (jwtOptions.Key.Length < 32)
{
    // HMAC-SHA256 needs a key of at least 256 bits.
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters long.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep claim names exactly as in the token ("sub", "email").
        options.MapInboundClaims = false;

        // On every request the token from the "Authorization: Bearer ..." header is checked:
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,            // created by our API
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,          // intended for our client
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,          // not expired
            ClockSkew = TimeSpan.Zero,        // no extra 5 minutes of tolerance after expiry
            ValidateIssuerSigningKey = true,  // signature made with our secret key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

builder.Services.AddAuthorization();

// CORS: the Angular app runs on another origin (localhost:4200),
// so the browser blocks its requests unless the API explicitly allows that origin.
const string FrontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();

// Swagger UI with an "Authorize" button to paste the JWT.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApp API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the token from /api/auth/login (without the 'Bearer ' prefix)."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// 2. HTTP request pipeline (middleware order matters)
// ---------------------------------------------------------------------------

// First, so it catches exceptions from everything below it.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Apply pending migrations on startup, so a fresh database is created automatically.
    // In production migrations are usually applied by a deployment step instead.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI(); // http://localhost:5000/swagger
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(FrontendCorsPolicy);
app.UseAuthentication(); // who are you? (reads and validates the JWT)
app.UseAuthorization();  // are you allowed? (checks [Authorize])
app.MapControllers();

app.Run();
