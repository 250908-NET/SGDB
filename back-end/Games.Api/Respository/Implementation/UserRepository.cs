using AutoMapper;
using Games.Data;
using Games.DTOs;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GamesDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(GamesDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.User.ToListAsync();
    }

    public Task<User> GetUserByIDAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> AddUserAsync(CreateUserDto DTO)
    {
        User user = _mapper.Map<User>(DTO);
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        return _mapper.Map<User>(user);

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