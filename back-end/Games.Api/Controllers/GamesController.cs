using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly GamesDbContext _context;

    public GamesController(GamesDbContext context)
    {
        _context = context;
    }

    // GET: receive all games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
    {
        var games = await _context.Games
            .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
            .Include(g => g.Developer)
            .Include(g => g.Publisher)
            .ToListAsync();

        var result = games.Select(g => new GameDto
        {
            GameId = g.GameId,
            Name = g.Name,
            Developer = g.Developer?.Name ?? "Unknown",
            Publisher = g.Publisher?.Name ?? "Unknown",
            ReleaseDate = g.ReleaseDate,
            Platforms = g.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
        });

        return Ok(result);
    }

    // GET: receive game by id
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(int id)
    {
        var game = await _context.Games
            .Include(g => g.GamePlatforms)
            .ThenInclude(gp => gp.Platform)
            .FirstOrDefaultAsync(g => g.GameId == id);

        if (game is null)
            return NotFound("Game not found");

        var result = new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            ReleaseDate = game.ReleaseDate,
            Developer = game.Developer?.Name ?? "Unknown",
            Publisher = game.Publisher?.Name ?? "Unknown",
            Platforms = game.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
        };

        return Ok(result);
    }

    // POST: add game
    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame(CreateGameDto dto)
    {
        var game = new Game
        {
            Name = dto.Name,
            ReleaseDate = dto.ReleaseDate,
            DeveloperId = dto.DeveloperId,
            PublisherId = dto.PublisherId
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            ReleaseDate = game.ReleaseDate,
            Developer = (await _context.Companies.FindAsync(dto.DeveloperId))?.Name ?? "Unknown",
            Publisher = (await _context.Companies.FindAsync(dto.PublisherId))?.Name ?? "Unknown",
            Platforms = new List<string>()
        });
    }

    // PUT: update game
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, UpdateGameDto dto)
    {
        var game = await _context.Games.FindAsync(id);
        if (game is null)
        {
            return NotFound("Game not found");
        }

        game.Name = dto.Name;
        game.ReleaseDate = dto.ReleaseDate;
        game.DeveloperId = dto.DeveloperId;
        game.PublisherId = dto.PublisherId;

        await _context.SaveChangesAsync();

        return Ok(new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            ReleaseDate = game.ReleaseDate,
            Developer = (await _context.Companies.FindAsync(dto.DeveloperId))?.Name ?? "Unknown",
            Publisher = (await _context.Companies.FindAsync(dto.PublisherId))?.Name ?? "Unknown",
            Platforms = await _context.GamePlatforms
                .Where(gp => gp.GameId == game.GameId)
                .Select(gp => gp.Platform.Name)
                .ToListAsync()
        });
    }

    // DELETE: delete game
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game is null) return NotFound("Game not found");

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    // POST: Link game to a platform
    [HttpPost("{gameId}/platforms/{platformId}")]
    public async Task<IActionResult> LinkGameToPlatform(int gameId, int platformId)
    {
        var game = await _context.Games.FindAsync(gameId);
        if (game is null)
        {
            return NotFound("Game not found");
        }

        var platform = await _context.Platforms.FindAsync(platformId);
        if (platform is null)
        {
            return NotFound("Platform not found");
        }


        var existingLink = await _context.GamePlatforms.FindAsync(gameId, platformId);

        if (existingLink == null)
        {
            _context.GamePlatforms.Add(new GamePlatform { GameId = gameId, PlatformId = platformId });
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            Message = "Platform linked successfully",
            Game = game.Name,
            Platform = platform.Name
        });
    }
    
}