using System.Threading.Tasks;

namespace Games.Services
{
    public interface IGameImageService
    {
        Task<string?> GetGameImageUrlAsync(string gameName);
    }
}
