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
    private readonly IMapper _mapper;

    public GamesController(ILogger<GamesController> logger, IGameService gameService, IMapper mapper)
    {
        _logger = logger;
        _service = gameService;
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

    // Get a game by gameID
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

    // Update a game by ID
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

    // Delete a game by ID
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

    [HttpPost("{gameId}/platforms/{platformId}")]
    public async Task<IActionResult> LinkGameToPlatform(int gameId, int platformId)
    {
        var game = await _service.LinkGameToPlatformAsync(gameId, platformId);

        if (game == null)
            return NotFound("Game or Platform not found");

        return Ok(_mapper.Map<GameDto>(game));
    }

}
