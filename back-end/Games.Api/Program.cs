using Games.Controllers;
using Games.Data;
using Games.DTOs;
using Games.Models;
using Games.Repositories;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Connection string
string CS = File.ReadAllText("../ConnectionString.txt");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core DbContext
builder.Services.AddDbContext<GamesDbContext>(options => options.UseSqlServer(CS));

// Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

// Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();

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
