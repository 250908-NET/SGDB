using Games.Models;

namespace Games.Services;

public interface ICompanyService
{
    Task<List<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task CreateAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(int id);
}

