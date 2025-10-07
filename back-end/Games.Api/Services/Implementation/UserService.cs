using Games.Models;
using Games.Repositories;

namespace Games.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<List<User>> GetAllUsersAsync() => await _repo.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await _repo.GetUserByIDAsync(id);

        public async Task<User?> GetUserByusername(string username) => await _repo.GetUserByUsername(username);
        public async Task AddUserAsync(User user)
        {
            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task ChangeUserAsync(User user)
        {
            await _repo.ChangeUserAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(int id)
        {
            await _repo.RemoveUserAsync(id);
            await _repo.SaveChangesAsync();
        }

        public async Task LinkUserToGenreAsync(int userId, int genreId)
        {
            await _repo.LinkUserToGenreAsync(userId, genreId);
        }

        public async Task UnlinkUserFromGenreAsync(int userId, int genreId)
        {
            await _repo.UnlinkUserFromGenreAsync(userId, genreId);
        }
    }
}
