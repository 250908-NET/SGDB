<<<<<<< HEAD:back-end/Games.Api/Respository/Implementation/UserRepository.cs
using AutoMapper;
using Games.Data;
using Games.DTOs;
=======
using Games.Data;
>>>>>>> 7f88cb9e0e887cc2399fcb579d2aa20b6bf7897e:back-end/Games.Api/Repository/Implementation/UserRepository.cs
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GamesDbContext _context;
<<<<<<< HEAD:back-end/Games.Api/Respository/Implementation/UserRepository.cs
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



=======

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
>>>>>>> 7f88cb9e0e887cc2399fcb579d2aa20b6bf7897e:back-end/Games.Api/Repository/Implementation/UserRepository.cs
}