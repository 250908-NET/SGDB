using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Games.Services;
using Games.DTOs;
using Games.Models;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly ILogger<RatingController> _logger;
    private readonly IRatingService _service;
    private readonly IGameService _gameService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public RatingController(ILogger<RatingController> logger, IRatingService ratingService, IGameService gameService, IUserService userService, IMapper mapper)
    {
        _logger = logger;
        _service = ratingService;
        _gameService = gameService;
        _userService = userService;
        _mapper = mapper;
    }

    // GET /api/rating
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet(Name = "GetAllRatings")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all ratings");
        var ratings = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // GET /api/rating/game/{gameId}
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("game/{gameId}", Name = "GetRatingsByGameId")]
    public async Task<IActionResult> GetByGameIdAsync(int gameId)
    {
        _logger.LogInformation("Getting ratings for game {GameId}", gameId);
        var ratings = await _service.GetByGameIdAsync(gameId);
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // GET /api/rating/user/{userId}
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("user/{userId}", Name = "GetRatingsByUserId")]
    public async Task<IActionResult> GetByUserIdAsync(int userId)
    {
        _logger.LogInformation("Getting ratings for user {UserId}", userId);
        var ratings = await _service.GetByUserIdAsync(userId);
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // POST /api/rating
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost(Name = "CreateRating")]
    public async Task<IActionResult> CreateAsync([FromBody] RatingDto dto)
    {
        if (dto == null)
            return BadRequest("Rating data is required.");

        var user = await _userService.GetUserByIdAsync(dto.UserId);
        if (user == null)
            return NotFound("User not found.");

        var game = await _gameService.GetByIdAsync(dto.GameId);
        if (game == null)
            return NotFound("Game not found.");

        var existingRatings = await _service.GetByUserIdAsync(dto.UserId);
        if (existingRatings.Any(r => r.GameId == dto.GameId))
            return Conflict("Rating already exists for this user and game.");

        var errors = new List<string>();
        if (dto.Rate < 1 || dto.Rate > 10)
            errors.Add("Rating score must be between 1 and 10.");
        if (string.IsNullOrWhiteSpace(dto.Title))
            errors.Add("Title is required and cannot be empty.");

        if (errors.Any())
            return BadRequest(new { Errors = errors });

        _logger.LogInformation("Creating rating {@dto}", dto);
        var rating = _mapper.Map<Rating>(dto);
        await _service.CreateAsync(rating);

        return CreatedAtRoute(
        "GetRatingByCompositeKey",
        new { userId = rating.UserId, gameId = rating.GameId },
        _mapper.Map<RatingDto>(rating)
);
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPut("{userId}/{gameId}", Name = "UpdateRating")]
    public async Task<IActionResult> UpdateAsync(int userId, int gameId, [FromBody] RatingDto dto)
    {
        // if (userId != dto.UserId || gameId != dto.GameId)
        //     return BadRequest("Composite key mismatch");
        dto.UserId = userId;
        dto.GameId = gameId;    

        var userRatings = await _service.GetByUserIdAsync(userId);
        var existing = userRatings.FirstOrDefault(r => r.GameId == gameId);
        if (existing == null)
            return NotFound("Rating not found.");

        _logger.LogInformation("Updating rating for user {UserId} and game {GameId}", userId, gameId);

        _mapper.Map(dto, existing);

        await _service.UpdateAsync(userId, gameId, existing);

        return Ok(_mapper.Map<RatingDto>(existing));
    }


    // DELETE /api/rating/{userId}/{gameId}
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete("{userId}/{gameId}", Name = "DeleteRating")]
    public async Task<IActionResult> DeleteAsync(int userId, int gameId)
    {

        var userRatings = await _service.GetByUserIdAsync(userId);
        var existing = userRatings.FirstOrDefault(r => r.GameId == gameId);
        if (existing == null)
            return NotFound("Rating not found.");

        _logger.LogInformation("Deleting rating for user {UserId} and game {GameId}", userId, gameId);
        await _service.DeleteAsync(userId, gameId);
        return NoContent();
    }
    
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("{userId}/{gameId}", Name = "GetRatingByCompositeKey")]
    public async Task<IActionResult> GetByCompositeKeyAsync(int userId, int gameId)
    {
        _logger.LogInformation("Getting rating for user {UserId} and game {GameId}", userId, gameId);
        var rating = await _service.GetByCompositeKeyAsync(userId, gameId);
        if (rating is null)
            return NotFound("Rating not found.");

        return Ok(_mapper.Map<RatingDto>(rating));
    }
}
