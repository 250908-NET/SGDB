using Games.Models;

namespace Games.Repositories;

public interface ICompanyRepository
{
    Task<List<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}

