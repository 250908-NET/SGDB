using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class PlatformRepository : IPlatformRepository
{
    private readonly GamesDbContext _context;

    public PlatformRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Platform>> GetAllAsync()
    {
        return await _context.Platforms.ToListAsync();
    }

    public async Task<Platform?> GetByIdAsync(int id)
    {
        return await _context.Platforms.FirstOrDefaultAsync(c => c.PlatformId == id);
    }

    public async Task AddAsync(Platform Platform)
    {
        await _context.Platforms.AddAsync(Platform);
    }

    public async Task UpdateAsync(Platform Platform)
    {
        _context.Platforms.Update(Platform);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var Platform = await _context.Platforms.FindAsync(id);
        if (Platform != null)
        {
            _context.Platforms.Remove(Platform);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}