<<<<<<< HEAD:back-end/Games.Api/Respository/Interface/IUserRepository.cs
using Games.Data;
using Games.DTOs;
using Games.Models;
public interface IUserRepository
{
    public Task<List<User>> GetAllAsync();

    public Task<User> GetUserByIDAsync();

    public Task<User> AddUserAsync(CreateUserDto dto);

    public Task<User> ChangeUserAsync();

    public Task<User> RemoveUserAsync();

=======
using Games.Models;

namespace Games.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
>>>>>>> 7f88cb9e0e887cc2399fcb579d2aa20b6bf7897e:back-end/Games.Api/Repository/Interface/IUserRepository.cs
}