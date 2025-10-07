using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _repo;

    public GameService(IGameRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Game>> GetGames(string? name)
    {
        if (name is null)
            return await _repo.GetAllAsync();
        else
            return await _repo.GetAllMatchingAsync(name);
    }

    public async Task<Game?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Game game)
    {
        await _repo.AddAsync(game);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        await _repo.UpdateAsync(game);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
    }

    public async Task LinkGameToPlatformAsync(int gameId, int platformId)
    {
        await _repo.LinkGameToPlatformAsync(gameId, platformId);
    }

    public async Task UpdateGamePlatformAsync(int gameId, int oldPlatformId, int newPlatformId)
    {
        await _repo.UpdateGamePlatformAsync(gameId, oldPlatformId, newPlatformId);
    }

    public async Task UnlinkGameFromPlatformAsync(int gameId, int platformId)
    {
        await _repo.UnlinkGameFromPlatformAsync(gameId, platformId);
    }

    public async Task ClearGamePlatformsAsync(int gameId)
    {
        await _repo.ClearGamePlatformsAsync(gameId);
    }

    public async Task LinkGameToGenreAsync(int gameId, int genreId)
    {
        await _repo.LinkGameToGenreAsync(gameId, genreId);
    }

    public async Task UpdateGameGenreAsync(int gameId, int oldGenreId, int newGenreId)
    {
        await _repo.UpdateGameGenreAsync(gameId, oldGenreId, newGenreId);
    }

    public async Task UnlinkGameFromGenreAsync(int gameId, int genreId)
    {
        await _repo.UnlinkGameFromGenreAsync(gameId, genreId);
    }
    public async Task ClearGameGenresAsync(int gameId)
    {
        await _repo.ClearGameGenresAsync(gameId);
    }

}