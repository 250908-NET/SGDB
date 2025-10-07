using System.Security.Principal;
using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class AccessTokenRepository : IAccessTokenRepository
{
    private readonly GamesDbContext _context;

    public AccessTokenRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccessTokenDto>> GetAllAccessToken()
    {
        return await _context.AccessToken
        .Include(at => at.User)
        .Select(at => new AccessTokenDto
        {
            Id = at.Id,
            Token = at.Token,
            UserId = at.UserId,
            Username = at.User.username,
            ExpiresAt = at.ExpiresAt,
            CreatedAt = at.CreatedAt,
            IsExpired = at.IsExpired
        }).ToListAsync();
    }

    public async Task<AccessToken> GetAllAccessTokenByUserId(int id)
    {
        AccessToken? token = await _context.AccessToken.FirstOrDefaultAsync(at => at.UserId == id);
        return token;
    }



    public async Task AddAsync(AccessToken accessToken)
    {
        await _context.AccessToken.AddAsync(accessToken);
    }

    public async Task UpdateAsync(AccessToken accessToken)
    {
        _context.AccessToken.Update(accessToken);
        await _context.SaveChangesAsync();

    }

    public async Task<bool> DeleteAsync(string token)
    {
        AccessToken? foundtoken = await _context.AccessToken.FirstOrDefaultAsync(at => at.Token == token);

        if (foundtoken != null)
        {
            _context.AccessToken.Remove(foundtoken);
            return true;
        }
        return false;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

