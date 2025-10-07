using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class GameRepository : IGameRepository
{
    private readonly GamesDbContext _context;

    public GameRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Game>> GetAllAsync()
    {
        return await _context.Games
            .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
            .Include(g => g.GameGenres).ThenInclude(gp => gp.Genre)
            .Include(g => g.Developer)
            .Include(g => g.Publisher)
            .ToListAsync();
    }

    public async Task<List<Game>> GetAllMatchingAsync(string? name)
    {
        var query = _context.Games.AsQueryable();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Query parameter cannot be empty or whitespace.", nameof(name));

        query = query.Where(g => EF.Functions.Like(g.Name, $"%{name}%"));

        return await query.ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
            .Include(g => g.GameGenres).ThenInclude(gp => gp.Genre)
            .Include(g => g.Developer)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.GameId == id);
    }

    public async Task AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
    }

    public async Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game != null)
        {
            _context.Games.Remove(game);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task LinkGameToPlatformAsync(int gameId, int platformId)
    {
        var existingLink = await _context.GamePlatforms.FindAsync(gameId, platformId);

        // Create new link if doesn't exist
        if (existingLink == null)
        {
            _context.GamePlatforms.Add(new GamePlatform { GameId = gameId, PlatformId = platformId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateGamePlatformAsync(int gameId, int oldPlatformId, int newPlatformId)
    {
        var updateLink = await _context.GamePlatforms.FindAsync(gameId, oldPlatformId);
        if (updateLink != null)
        {
            _context.GamePlatforms.Remove(updateLink);
            _context.GamePlatforms.Add(new GamePlatform { GameId = gameId, PlatformId = newPlatformId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnlinkGameFromPlatformAsync(int gameId, int platformId)
    {
        var link = await _context.GamePlatforms.FindAsync(gameId, platformId);
        if (link != null)
        {
            _context.GamePlatforms.Remove(link);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearGamePlatformsAsync(int gameId)
    {
        var links = await _context.GamePlatforms.Where(gp => gp.GameId == gameId).ToListAsync();
        _context.GamePlatforms.RemoveRange(links);
        await _context.SaveChangesAsync();
    }

    public async Task LinkGameToGenreAsync(int gameId, int genreId)
    {
        var existingLink = await _context.GameGenres.FindAsync(gameId, genreId);

        // Create new link if doesn't exist
        if (existingLink == null)
        {
            _context.GameGenres.Add(new GameGenre { GameId = gameId, GenreId = genreId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateGameGenreAsync(int gameId, int oldGenreId, int newGenreId)
    {
        var updateLink = await _context.GameGenres.FindAsync(gameId, oldGenreId);
        if (updateLink != null)
        {
            _context.GameGenres.Remove(updateLink);
            _context.GameGenres.Add(new GameGenre { GameId = gameId, GenreId = newGenreId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnlinkGameFromGenreAsync(int gameId, int genreId)
    {
        var link = await _context.GameGenres.FindAsync(gameId, genreId);
        if (link != null)
        {
            _context.GameGenres.Remove(link);
            await _context.SaveChangesAsync();
        }
    }
    }

    public async Task ClearGameGenresAsync(int gameId)
    {
        var links = await _context.GameGenres.Where(gg => gg.GameId == gameId).ToListAsync();
        _context.GameGenres.RemoveRange(links);
        await _context.SaveChangesAsync();
    }

    


}
