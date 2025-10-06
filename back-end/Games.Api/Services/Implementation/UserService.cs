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

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            string lowerUsername = username.ToLower();
            return await _repo.GetUserByUsernameAsync(lowerUsername);
        }

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

        public async Task LinkUserToGameAsync(int userId, int gameId)
        {
            await _repo.LinkUserToGameAsync(userId, gameId);
        }

        public async Task UnlinkUserFromGameAsync(int userId, int gameId)
        {
            await _repo.UnlinkUserFromGameAsync(userId, gameId);
        }
    }
}
