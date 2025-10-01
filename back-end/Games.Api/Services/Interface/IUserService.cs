using Games.DTOs;
using Games.Models;

namespace Games.Services;

public interface IUserService
{
    public Task<List<User>> GetAllUsersAsync();
    public Task<User?> GetUserByIdAsync(int id);
    public Task<User> CreateUserAsync(UserDto Dto);
    public Task<User> UpdateUserAsync(UserDto Dto);
    public Task<User> DeleteUserAsync(int id);
}
