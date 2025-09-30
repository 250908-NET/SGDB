using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _repo;

    public PlatformService(IPlatformRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Platform>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Platform?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Platform Platform)
    {
        await _repo.AddAsync(Platform);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Platform Platform)
    {
        await _repo.UpdateAsync(Platform);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
    }
}
