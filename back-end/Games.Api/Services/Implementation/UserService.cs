using Games.DTOs;
using Games.Models;
using Games.Repositories;

namespace Games.Services;

public class UserService : IUserService
{
    private IUserRepository _userRepo;
    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return _userRepo.GetAllAsync();
    }
    public Task<User?> GetUserByIdAsync(int id)
    {
        return _userRepo.GetUserByIDAsync();
    }
    public Task<User> CreateUserAsync(CreateUserDto Dto)
    {
        return _userRepo.AddUserAsync(Dto);
    }
    public Task<User> UpdateUserAsync(UserDto Dto)
    {
        return _userRepo.ChangeUserAsync();
    }
    public Task<User> DeleteUserAsync(int id)
    {
        return _userRepo.RemoveUserAsync();
    }
}
