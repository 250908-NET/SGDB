using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly GamesDbContext _context;

    public PlatformsController(GamesDbContext context)
    {
        _context = context;
    }
    
    // GET: receive all platforms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatforms()
    {
        var platforms = await _context.Platforms
            .Include(p => p.GamePlatforms)
            .ThenInclude(gp => gp.Game)
            .ToListAsync();

        var result = platforms.Select(p => new PlatformDto
        {
            PlatformId = p.PlatformId,
            Name = p.Name,
            Games = p.GamePlatforms.Select(gp => gp.Game.Name).ToList()
        });

        return Ok(result);
    }

    // GET: receive platform by id
    [HttpGet("{id}")]
    public async Task<ActionResult<PlatformDto>> GetPlatform(int id)
    {
        var platform = await _context.Platforms
            .Include(p => p.GamePlatforms)
            .ThenInclude(gp => gp.Game)
            .FirstOrDefaultAsync(p => p.PlatformId == id);

        if (platform is null)
            return NotFound("Platform not found");

        var result = new PlatformDto
        {
            PlatformId = platform.PlatformId,
            Name = platform.Name,
            Games = platform.GamePlatforms.Select(gp => gp.Game.Name).ToList()
        };

        return Ok(result);
    }

    // POST: create new platform
    [HttpPost]
    public async Task<ActionResult<PlatformDto>> CreatePlatform(CreatePlatformDto dto)
    {
        var platform = new Platform
        {
            Name = dto.Name
        };

        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlatform), new { id = platform.PlatformId }, new PlatformDto
        {
            PlatformId = platform.PlatformId,
            Name = platform.Name,
            Games = new List<string>()
        });
    }

    // PUT: update platform
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        var platform = await _context.Platforms.FindAsync(id);
        if (platform is null)
        {
            return NotFound("Platform not found");
        }

        platform.Name = dto.Name;

        await _context.SaveChangesAsync();

        return Ok(new PlatformDto
        {
            PlatformId = platform.PlatformId,
            Name = platform.Name,
            Games = await _context.GamePlatforms
                .Where(gp => gp.PlatformId == platform.PlatformId)
                .Select(gp => gp.Game.Name)
                .ToListAsync()
        });
    }

    // DELETE: delete platform
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform(int id)
    {
        var platform = await _context.Platforms.FindAsync(id);
        if (platform is null) return NotFound("Platform not found");

        _context.Platforms.Remove(platform);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}