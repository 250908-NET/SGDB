using Games.Models;

namespace Games.Services;

public interface IPlatformService
{
    public Task<List<Platform>> GetAllAsync();
    public Task<Platform?> GetByIdAsync(int id);
    public Task CreateAsync(Platform Platform);
    public Task UpdateAsync(Platform Platform);
    public Task DeleteAsync(int id);
}
