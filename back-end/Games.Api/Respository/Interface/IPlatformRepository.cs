using Games.Models;

namespace Games.Repositories;

public interface IPlatformRepository
{
    Task<List<Platform>> GetAllAsync();
    Task<Platform?> GetByIdAsync(int id);
    Task AddAsync(Platform Platform);
    Task UpdateAsync(Platform Platform);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}