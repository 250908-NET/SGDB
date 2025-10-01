using Games.Models;

namespace Games.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
}