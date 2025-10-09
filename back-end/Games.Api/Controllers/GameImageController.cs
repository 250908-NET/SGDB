using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Games.Services;

namespace Games.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameImageController : ControllerBase
    {
        private readonly GameImageService _imageService;

        public GameImageController(GameImageService imageService)
        {
            _imageService = imageService;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("{gameName}")]
        public async Task<IActionResult> Get(string gameName)
        {
            var imageUrl = await _imageService.GetGameImageUrlAsync(gameName);

            if (imageUrl == null)
                return NotFound(new { message = "Game image not found." });

            return Ok(new { game = gameName, imageUrl });
        }
    }
}
