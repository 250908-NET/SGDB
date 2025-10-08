using Games.Models;

namespace Games.Repositories;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task AddAsync(Genre Genre);
    Task UpdateAsync(Genre Genre);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
    Task ClearGenreGamesAsync(int id);
}