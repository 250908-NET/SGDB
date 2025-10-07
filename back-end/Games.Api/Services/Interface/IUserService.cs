using Games.DTOs;
using Games.Models;

namespace Games.Services;

public interface IUserService
{
    public Task<List<User>> GetAllUsersAsync();
    public Task<User?> GetUserByIdAsync(int id);
    public Task<User?> GetUserByusername(string username);
    public Task AddUserAsync(User user);
    public Task ChangeUserAsync(User user);
    public Task RemoveUserAsync(int id);

    public Task LinkUserToGenreAsync(int userId, int genreId);
    public Task UnlinkUserFromGenreAsync(int userId, int genreId);
}
