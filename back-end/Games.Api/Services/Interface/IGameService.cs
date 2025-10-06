using Games.Models;

namespace Games.Services;

public interface IGameService
{
    public Task<List<Game>> GetGames(string? name);
    public Task<Game?> GetByIdAsync(int id);
    public Task CreateAsync(Game game);
    public Task UpdateAsync(Game game);
    public Task DeleteAsync(int id);

    public Task LinkGameToPlatformAsync(int gameId, int platformId);
    public Task UpdateGamePlatformAsync(int gameId, int oldPlatformId, int newPlatformId);
    public Task UnlinkGameFromPlatformAsync(int gameId, int platformId);

    public Task LinkGameToGenreAsync(int gameId, int genreId);
    public Task UpdateGameGenreAsync(int gameId, int oldGenreId, int newGenreId);
    public Task UnlinkGameFromGenreAsync(int gameId, int genreId);

}
