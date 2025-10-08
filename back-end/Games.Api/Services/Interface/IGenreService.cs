using Games.Models;

namespace Games.Services;

public interface IGenreService
{
    public Task<List<Genre>> GetAllAsync();
    public Task<Genre?> GetByIdAsync(int id);
    public Task CreateAsync(Genre Genre);
    public Task UpdateAsync(Genre Genre);
    public Task DeleteAsync(int id);
    public Task ClearGenreGamesAsync(int genreId);
}
