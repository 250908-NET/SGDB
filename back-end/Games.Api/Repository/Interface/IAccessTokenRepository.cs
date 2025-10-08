using Games.Models;

namespace Games.Repositories;

public interface IAccessTokenRepository
{
    public Task<List<AccessTokenDto>> GetAllAccessToken();
    public Task<AccessToken> GetAllAccessTokenByUserId(int id);
    public Task AddAsync(AccessToken accessToken);

    public Task UpdateAsync(AccessToken accessToken);
    public Task<bool> DeleteAsync(string token);
    public Task SaveChangesAsync();

}