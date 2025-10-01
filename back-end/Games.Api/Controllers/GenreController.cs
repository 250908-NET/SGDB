using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly GamesDbContext _context;

    public GenresController(GamesDbContext context)
    {
        _context = context;
    }
    
    // GET: receive all platforms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
    {
        var platforms = await _context.Genres
            .Include(p => p.GameGenres)
            .ThenInclude(gp => gp.Game)
            .ToListAsync();

        var result = platforms.Select(p => new GenreDto
        {
            GenreId = p.GenreId,
            Name = p.Name,
            Games = p.GameGenres.Select(gp => gp.Game.Name).ToList()
        });

        return Ok(result);
    }

    // GET: receive platform by id
    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        var platform = await _context.Genres
            .Include(p => p.GameGenres)
            .ThenInclude(gp => gp.Game)
            .FirstOrDefaultAsync(p => p.GenreId == id);

        if (platform is null)
            return NotFound("Genre not found");

        var result = new GenreDto
        {
            GenreId = platform.GenreId,
            Name = platform.Name,
            Games = platform.GameGenres.Select(gp => gp.Game.Name).ToList()
        };

        return Ok(result);
    }

    // POST: create new platform
    [HttpPost]
    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto dto)
    {
        var platform = new Genre
        {
            Name = dto.Name
        };

        _context.Genres.Add(platform);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = platform.GenreId }, new GenreDto
        {
            GenreId = platform.GenreId,
            Name = platform.Name,
            Games = new List<string>()
        });
    }

    // PUT: update platform
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto dto)
    {
        var platform = await _context.Genres.FindAsync(id);
        if (platform is null)
        {
            return NotFound("Genre not found");
        }

        platform.Name = dto.Name;

        await _context.SaveChangesAsync();

        return Ok(new GenreDto
        {
            GenreId = platform.GenreId,
            Name = platform.Name,
            Games = await _context.GameGenres
                .Where(gp => gp.GenreId == platform.GenreId)
                .Select(gp => gp.Game.Name)
                .ToListAsync()
        });
    }

    // DELETE: delete platform
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var platform = await _context.Genres.FindAsync(id);
        if (platform is null) return NotFound("Genre not found");

        _context.Genres.Remove(platform);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}