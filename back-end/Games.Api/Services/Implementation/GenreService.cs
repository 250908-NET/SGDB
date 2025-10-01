using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repo;

    public GenreService(IGenreRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Genre>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Genre?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Genre Genre)
    {
        await _repo.AddAsync(Genre);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Genre Genre)
    {
        await _repo.UpdateAsync(Genre);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
    }
}
