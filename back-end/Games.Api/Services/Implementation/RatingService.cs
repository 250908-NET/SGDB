using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _repo;

    public RatingService(IRatingRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Rating>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<List<Rating>> GetByGameIdAsync(int gameId) =>
        await _repo.GetByGameIdAsync(gameId);

    public async Task<List<Rating>> GetByUserIdAsync(int userId) =>
        await _repo.GetByUserIdAsync(userId);

    public async Task<Rating> CreateAsync(Rating rating) =>
        await _repo.AddAsync(rating);

    public async Task UpdateAsync(int userId, int gameId, Rating rating)
    {
        await _repo.UpdateAsync(userId, gameId, rating);
    }

    public async Task DeleteAsync(int userId, int gameId) =>
        await _repo.DeleteAsync(userId, gameId);

    public async Task<Rating?> GetByCompositeKeyAsync(int userId, int gameId)
    {
        return await _repo.GetByCompositeKeyAsync(userId, gameId);
    }

}