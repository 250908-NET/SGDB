using AutoMapper;
using Games.Controllers;
using Games.Data;
using Games.DTOs;
using Games.Models;
using Games.Repositories;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

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

var app = builder.Build();

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
