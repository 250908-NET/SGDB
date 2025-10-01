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

    public async Task<List<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByIDAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> AddUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> ChangeUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> RemoveUserAsync()
    {
        throw new NotImplementedException();
    }



}