using Games.Models;

namespace Games.Repositories;

public interface IGameRepository
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task AddAsync(Game game);
    Task UpdateAsync(Game game);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();

    Task LinkGameToPlatformAsync(int gameId, int platformId);
    Task UpdateGamePlatformAsync(int gameId, int oldPlatformId, int newPlatformId);
    Task UnlinkGameFromPlatformAsync(int gameId, int platformId);
}