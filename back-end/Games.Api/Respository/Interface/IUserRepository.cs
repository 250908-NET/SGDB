using Games.Data;
using Games.Models;
interface IUserRepository
{
    public Task<List<User>> GetAllAsync();

    public Task<User> GetUserByIDAsync();

    public Task<User> AddUserAsync();

    public Task<User> ChangeUserAsync();

    public Task<User> RemoveUserAsync();

}