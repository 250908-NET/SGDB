using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly GamesDbContext _context;

    public RatingRepository(GamesDbContext context)
    {
        _context = context;
    }

    // GET /api/ratings
    public async Task<List<Rating>> GetAllAsync()
    {
        return await _context.Ratings.ToListAsync();
    }

    // GET /api/ratings/game/{gameId}
    public async Task<List<Rating>> GetByGameIdAsync(int gameId)
    {
        return await _context.Ratings
            .Where(r => r.GameId == gameId)
            .ToListAsync();
    }

    // GET /api/ratings/user/{userId}
    public async Task<List<Rating>> GetByUserIdAsync(int userId)
    {
        return await _context.Ratings
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    // POST /api/ratings
    public async Task<Rating> AddAsync(Rating rating)
    {
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    // PUT /api/ratings/{userId}/{gameId}
    public async Task UpdateAsync(int userId, int gameId, Rating updatedRating)
    {
        var existing = await _context.Ratings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

        if (existing == null)
            throw new KeyNotFoundException("Rating not found.");

        _context.Ratings.Attach(updatedRating);
        _context.Entry(updatedRating).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // DELETE /api/ratings/{userId}/{gameId}
    public async Task DeleteAsync(int userId, int gameId)
    {
        var rating = await _context.Ratings.FindAsync(userId, gameId);

        if (rating == null)
        {

            throw new KeyNotFoundException("Rating not found.");
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Rating?> GetByCompositeKeyAsync(int userId, int gameId)
    {
        return await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);
    }

}