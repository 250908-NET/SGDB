using System.Text;
using AutoMapper;
using Games.Controllers;
using Games.Data;
using Games.DTOs;
using Games.Models;
using Games.Repositories;
using Games.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy => policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod());
    });
}


// EF Core DbContext
if (!builder.Environment.IsEnvironment("Testing"))
{
    // Connection string
    string CS = File.ReadAllText("../ConnectionString.txt");

    builder.Services.AddDbContext<GamesDbContext>(options => options.UseSqlServer(CS));
}

// Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();

// Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITokenServices, TokenService>();

//External API Request for Game Images
builder.Services.AddHttpClient<IGameImageService, GameImageService>(client =>
{
    client.BaseAddress = new Uri("https://api.rawg.io/api/");
});


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
// Auth
string keyString = File.ReadAllText("../key.txt");
// byte[] key = Encoding.ASCII.GetBytes(keyString);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Server",//builder.Configuration["Jwt:Issuer"],
        ValidAudience = "User",//builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine("message recived");
            if (context.Request.Cookies.ContainsKey("access_token"))
            {
                Console.WriteLine("Token from cookie: " + context.Request.Cookies["access_token"]);
                context.Token = context.Request.Cookies["access_token"];
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(); // Add authorization services


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (builder.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

// Log each HTTP request
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (d, ctx) =>
    {
        d.Set("RequestPath", ctx.Request.Path);
        d.Set("RequestHost", ctx.Request.Host.Value);
        d.Set("UserAgent", ctx.Request.Headers.UserAgent.ToString());
    };
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }
