using Games.Models;

namespace Games.Repositories;

public interface IRatingRepository
{
    // GET /api/ratings
    public Task<List<Rating>> GetAllAsync();

    // GET /api/ratings/game/{gameId}
    public Task<List<Rating>> GetByGameIdAsync(int gameId);

    // GET /api/ratings/user/{userId}
    public Task<List<Rating>> GetByUserIdAsync(int userId);

    // POST /api/ratings
    public Task<Rating> AddAsync(Rating rating);

    // PUT /api/ratings/{userId}/{gameId}
    public Task UpdateAsync(int userId, int gameId, Rating rating);

    // DELETE /api/ratings/{userId}/{gameId}
    public Task DeleteAsync(int userId, int gameId);

    Task<Rating?> GetByCompositeKeyAsync(int userId, int gameId);
}