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
    public Task<User> AddAsync(User user);



}