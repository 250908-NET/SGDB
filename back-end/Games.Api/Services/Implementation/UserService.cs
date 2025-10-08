using Games.Models;
using Games.Repositories;

namespace Games.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IGenreRepository _genreRepo;
        private readonly IGameRepository _gameRepo;

        public UserService(IUserRepository repo, IGenreRepository genreRepo, IGameRepository gameRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _genreRepo = genreRepo ?? throw new ArgumentNullException(nameof(genreRepo));
            _gameRepo = gameRepo ?? throw new ArgumentNullException(nameof(gameRepo));
        }

        public async Task<List<User>> GetAllUsersAsync() => await _repo.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("User ID must be positive.", nameof(id));

            return await _repo.GetUserByIDAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", nameof(username));

            string lowerUsername = username.ToLower();
            return await _repo.GetUserByUsernameAsync(lowerUsername);
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.username))
                throw new ArgumentException("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(user.role))
                throw new ArgumentException("User role cannot be empty.");


            // Check if each genre exists
            foreach (var userGenre in user.UserGenres)
            {
                var genre = await _genreRepo.GetByIdAsync(userGenre.GenreId);
                if (genre is null)
                {
                    throw new InvalidOperationException($"Genre with ID {userGenre.GenreId} does not exist.");
                }
            }

            // Check if each game exists
            foreach (var gameLibrary in user.GameLibrary)
            {
                var game = await _gameRepo.GetByIdAsync(gameLibrary.GameId);
                if (game is null)
                {
                    throw new InvalidOperationException($"Game with ID {gameLibrary.GameId} does not exist.");
                }
            }

            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task ChangeUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.username))
                throw new ArgumentException("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(user.role))
                throw new ArgumentException("User role cannot be empty.");

            // Check if user exists
            var existingUser = await _repo.GetUserByIDAsync(user.UserId);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {user.UserId} not found.");

            

            // Check if each genre exists
            foreach (var userGenre in user.UserGenres)
            {
                var genre = await _genreRepo.GetByIdAsync(userGenre.GenreId);
                if (genre is null)
                {
                    throw new InvalidOperationException($"Genre with ID {userGenre.GenreId} does not exist.");
                }
            }

            // Check if each game exists
            foreach (var gameLibrary in user.GameLibrary)
            {
                var game = await _gameRepo.GetByIdAsync(gameLibrary.GameId);
                if (game is null)
                {
                    throw new InvalidOperationException($"Game with ID {gameLibrary.GameId} does not exist.");
                }
            }
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
