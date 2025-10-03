using Games.DTOs;
using Games.Models;

namespace Games.Services;

public interface IUserService
{
    public Task<List<User>> GetAllUsersAsync();
    public Task<User?> GetUserByIdAsync(int id);
    public Task AddUserAsync(User user);
    public Task ChangeUserAsync(User user);
    public Task RemoveUserAsync(int id);
}
