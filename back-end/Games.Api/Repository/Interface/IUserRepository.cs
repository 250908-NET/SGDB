using Games.Data;
using Games.DTOs;
using Games.Models;

namespace Games.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetUserByIDAsync(int id);

    Task<User?> GetUserByUsername(string username);

    Task AddUserAsync(User user);
    Task ChangeUserAsync(User user);
    Task RemoveUserAsync(int id);
    //Task AddAsync(User user);
    Task SaveChangesAsync();

    Task LinkUserToGenreAsync(int userId, int genreId);
    //Task UpdateUserGenreAsync(int userId, int oldGenreId, int newGenreId);
    Task UnlinkUserFromGenreAsync(int userId, int genreId);

}