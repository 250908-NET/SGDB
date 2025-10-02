using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<User> CreateAsync(User user) =>
        await _repo.AddAsync(user);
}