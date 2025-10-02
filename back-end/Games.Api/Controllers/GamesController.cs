using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;


namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    //private readonly GamesDbContext _context;
    private readonly ILogger<GamesController> _logger;
    private readonly IGameService _service;
    private readonly IPlatformService _platformService;
    private readonly IMapper _mapper;

    public GamesController(ILogger<GamesController> logger, IGameService gameService, IPlatformService platformService, IMapper mapper)
    {
        _logger = logger;
        _service = gameService;
        _platformService = platformService;
        _mapper = mapper;
    }

    // Get all games
    [HttpGet(Name = "GetAllGames")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all games");
        var games = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<GameDto>>(games));
    }

    // Get a game by gameId
    [HttpGet("{id}", Name = "GetGameById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting game {id}", id);
        var game = await _service.GetByIdAsync(id);
        if (game is null)
            return NotFound("Game not found");
        return Ok(_mapper.Map<GameDto>(game));
    }

    // Add a game to DB
    [HttpPost(Name = "CreateGame")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateGameDto dto)
    {
        _logger.LogInformation("Creating game {@dto}", dto);
        var game = _mapper.Map<Game>(dto);
        await _service.CreateAsync(game);
        return Created($"/api/games/{game.GameId}", _mapper.Map<GameDto>(game));
    }

    // Update a game by Id
    [HttpPut("{id}", Name = "UpdateGame")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateGameDto dto)
    {
        _logger.LogInformation("Updating game {id}", id);
        var game = await _service.GetByIdAsync(id);
        if (game is null)
        {
            return NotFound("Game not found");
        }

        _mapper.Map(dto, game);
        await _service.UpdateAsync(game);

        return Ok(_mapper.Map<GameDto>(game));
    }

    // Delete a game by Id
    [HttpDelete("{id}", Name = "DeleteGame")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting game {id}", id);
        var existing = await _service.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound("Game not found");
        }
        await _service.DeleteAsync(id);
        return NoContent();
    }

    // Link game to platform
    [HttpPost("{gameId}/platforms/{platformId}")]
    public async Task<IActionResult> LinkGameToPlatform(int gameId, int platformId)
    {
        _logger.LogInformation("Linking game {gameId} to platform {platformId}", gameId, platformId);

        // Check to see if game exists
        var game = await _service.GetByIdAsync(gameId);
        if (game is null)
        {
            return NotFound($"Game with ID {gameId} not found.");
        }

        // Check to see if platform exists
        var platform = await _platformService.GetByIdAsync(platformId);
        if (platform is null)
        {
            return NotFound($"Platform with ID {platformId} not found.");
        }

        // Check if already linked
        if (game.GamePlatforms.Any(gp => gp.PlatformId == platformId))
        {
            return BadRequest("This game is already linked to that platform.");
        }
        await _service.LinkGameToPlatformAsync(gameId, platformId);
        return Ok(new { Message = "Linked successfully", GameId = gameId, PlatformId = platformId });
    }

    // Update link
    [HttpPut("{gameId}/platforms/{oldPlatformId}/{newPlatformId}")]
    public async Task<IActionResult> UpdateGamePlatform(int gameId, int oldPlatformId, int newPlatformId)
    {
        _logger.LogInformation("Update linking game {gameId} from platform {oldPlatformId} to platform {newPlatformId}", gameId, oldPlatformId, newPlatformId);

        // Check to see if game exists
        var game = await _service.GetByIdAsync(gameId);
        if (game is null)
        {
            return NotFound($"Game with ID {gameId} not found.");
        }

        // Check to see if old platform exists
        var oldPlatform = await _platformService.GetByIdAsync(oldPlatformId);
        if (oldPlatform is null)
        {
            return NotFound($"Old Platform with ID {oldPlatformId} not found.");
        }

        // Check to see if new platform exists
        var newPlatform = await _platformService.GetByIdAsync(newPlatformId);
        if (newPlatform is null)
        {
            return NotFound($"New Platform with ID {newPlatformId} not found.");
        }

        // Ensure game is currently linked to oldPlatform
        if (!game.GamePlatforms.Any(gp => gp.PlatformId == oldPlatformId))
        {
            return BadRequest("This game is not linked to the old platform.");
        }

        // Ensure game not already linked to newPlatform
        if (game.GamePlatforms.Any(gp => gp.PlatformId == newPlatformId))
        {
            return BadRequest("This game is already linked to the new platform.");
        }

        await _service.UpdateGamePlatformAsync(gameId, oldPlatformId, newPlatformId);
        return Ok(new { Message = "Link updated successfully", GameId = gameId, NewPlatformId = newPlatformId });
    }

    // Unlink
    [HttpDelete("{gameId}/platforms/{platformId}")]
    public async Task<IActionResult> DeleteGamePlatform(int gameId, int platformId)
    {
        _logger.LogInformation("Unlinking game {gameId} from platform {platformId}", gameId, platformId);
        // Check to see if game exists
        var game = await _service.GetByIdAsync(gameId);
        if (game is null)
        {
            return NotFound($"Game with ID {gameId} not found.");
        }

        // Check to see if platform exists
        var platform = await _platformService.GetByIdAsync(platformId);
        if (platform is null)
        {
            return NotFound($"Platform with ID {platformId} not found.");
        }

        // Check if link actually exists
        if (!game.GamePlatforms.Any(gp => gp.PlatformId == platformId))
        {
            return BadRequest("This game is not linked to that platform.");
        }

        await _service.UnlinkGameFromPlatformAsync(gameId, platformId);
        return NoContent();
    }

}
