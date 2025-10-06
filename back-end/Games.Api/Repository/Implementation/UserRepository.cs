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
        return await _context.Users
            .Include(u => u.GameLibrary)
            .Include(u => u.Ratings)
            .Include(u => u.UserGenres)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIDAsync(int id)
    {
        return await _context.Users
            .Include(u => u.GameLibrary)
            .Include(u => u.Ratings)
            .Include(u => u.UserGenres)
            .FirstOrDefaultAsync(g => g.UserId == id);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);

    }

    public async Task ChangeUserAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task RemoveUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task LinkUserToGenreAsync(int userId, int genreId)
    {
        var existingLink = await _context.UserGenres.FindAsync(userId, genreId);

        if (existingLink == null)
        {
            _context.UserGenres.Add(new UserGenre { UserId = userId, GenreId = genreId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnlinkUserFromGenreAsync(int userId, int genreId)
    {
        var link = await _context.UserGenres.FindAsync(userId, genreId);
        if (link != null)
        {
            _context.UserGenres.Remove(link);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LinkUserToGameAsync(int userId, int gameId)
    {
        var existingLink = await _context.UserGames.FindAsync(userId, gameId);

        if (existingLink == null)
        {
            _context.UserGames.Add(new UserGame { UserId = userId, GameId = gameId });
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnlinkUserFromGameAsync(int userId, int gameId)
    {
        var link = await _context.UserGames.FindAsync(userId, gameId);
        if (link != null)
        {
            _context.UserGames.Remove(link);
            await _context.SaveChangesAsync();
        }
    }



}