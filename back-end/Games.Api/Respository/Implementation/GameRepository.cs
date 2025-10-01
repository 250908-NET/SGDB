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
            .Include(g => g.Developer)
            .Include(g => g.Publisher)
            .ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
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

    public async Task<Game?> LinkGameToPlatformAsync(int gameId, int platformId)
    {
        var game = await _context.Games
            .Include(g => g.GamePlatforms)
            .ThenInclude(gp => gp.Platform)
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game == null)
            throw new ArgumentException($"Game with ID {gameId} not found");

        var platform = await _context.Platforms.FindAsync(platformId);
        if (platform == null)
            throw new ArgumentException($"Platform with ID {platformId} not found");

        var existingLink = await _context.GamePlatforms.FindAsync(gameId, platformId);

        // Create new link if doesn't exist
        if (existingLink == null)
        {
            _context.GamePlatforms.Add(new GamePlatform { GameId = gameId, PlatformId = platformId });
            await _context.SaveChangesAsync();
        }

        game = await _context.Games
            .Include(g => g.GamePlatforms)
            .ThenInclude(gp => gp.Platform)
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        return game;
    }

}