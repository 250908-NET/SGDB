using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _repo;

    public GameService(IGameRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Game>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Game?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Game game)
    {
        await _repo.AddAsync(game);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        await _repo.UpdateAsync(game);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
    }
}
