using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly GamesDbContext _context;

    public CompanyRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies
            .Include(c => c.DevelopedGames)
            .Include(c => c.PublishedGames)
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies
            .Include(c => c.DevelopedGames)
            .Include(c => c.PublishedGames)
            .FirstOrDefaultAsync(c => c.CompanyId == id);
    }

    public async Task AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company != null)
        {
            _context.Companies.Remove(company);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

