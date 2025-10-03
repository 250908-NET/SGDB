using Games.Data;
using Games.DTOs;
using Games.Models;

namespace Games.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetUserByIDAsync(int id);

    Task AddUserAsync(User user);
    Task ChangeUserAsync(User user);
    Task RemoveUserAsync(int id);
    //Task AddAsync(User user);
    Task SaveChangesAsync();

}