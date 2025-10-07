using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly GamesDbContext _context;

    public GenreRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetAllAsync()
    {
        return await _context.Genres
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Game)
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Game)
            .FirstOrDefaultAsync(c => c.GenreId == id);
    }

    public async Task AddAsync(Genre Genre)
    {
        await _context.Genres.AddAsync(Genre);
    }

    public async Task UpdateAsync(Genre Genre)
    {
        _context.Genres.Update(Genre);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var Genre = await _context.Genres.FindAsync(id);
        if (Genre != null)
        {
            _context.Genres.Remove(Genre);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task ClearGenreGamesAsync(int genreId)
    {
        var links = await _context.GameGenres.Where(gg => gg.GenreId == genreId).ToListAsync();
        if (links.Any())
        {
            _context.GameGenres.RemoveRange(links);
            await _context.SaveChangesAsync();
        }
    }
}