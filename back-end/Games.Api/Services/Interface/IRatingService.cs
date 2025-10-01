using Games.Models;

namespace Games.Services;

public interface IRatingService
{
    Task<List<Rating>> GetAllAsync();
    Task<List<Rating>> GetByGameIdAsync(int gameId);
    Task<List<Rating>> GetByUserIdAsync(int userId);
    Task<Rating> AddAsync(Rating rating);
    Task UpdateAsync(int userId, int gameId, Rating rating);
    Task DeleteAsync(int userId, int gameId);
}