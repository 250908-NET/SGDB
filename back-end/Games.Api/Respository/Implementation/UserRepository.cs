using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GamesDbContext _context;

    public UserRepository(GamesDbContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}