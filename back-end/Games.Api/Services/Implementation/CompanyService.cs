using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repo;

    public CompanyService(ICompanyRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<List<Company>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Company?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Company company)
    {
        await _repo.AddAsync(company);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        await _repo.UpdateAsync(company);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
    }
}

