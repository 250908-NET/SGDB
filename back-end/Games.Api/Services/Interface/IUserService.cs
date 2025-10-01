using Games.Models;

namespace Games.Services;

public interface IUserService
{
    Task<User> CreateAsync(User user);
}
