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
    private readonly IMapper _mapper;

    public RatingController(ILogger<RatingController> logger, IRatingService ratingService, IMapper mapper)
    {
        _logger = logger;
        _service = ratingService;
        _mapper = mapper;
    }

    // GET /api/ratings
    [HttpGet(Name = "GetAllRatings")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all ratings");
        var ratings = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // GET /api/ratings/game/{gameId}
    [HttpGet("game/{gameId}", Name = "GetRatingsByGameId")]
    public async Task<IActionResult> GetByGameIdAsync(int gameId)
    {
        _logger.LogInformation("Getting ratings for game {GameId}", gameId);
        var ratings = await _service.GetByGameIdAsync(gameId);
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // GET /api/ratings/user/{userId}
    [HttpGet("user/{userId}", Name = "GetRatingsByUserId")]
    public async Task<IActionResult> GetByUserIdAsync(int userId)
    {
        _logger.LogInformation("Getting ratings for user {UserId}", userId);
        var ratings = await _service.GetByUserIdAsync(userId);
        return Ok(_mapper.Map<List<RatingDto>>(ratings));
    }

    // GET /api/ratings/{userId}/{gameId}
    [HttpGet("{userId}/{gameId}", Name = "GetRatingByCompositeKey")]
    public async Task<IActionResult> GetByCompositeKeyAsync(int userId, int gameId)
    {
        _logger.LogInformation("Getting rating for user {UserId} and game {GameId}", userId, gameId);
        var ratings = await _service.GetByUserIdAsync(userId);
        var rating = ratings.FirstOrDefault(r => r.GameId == gameId);
        if (rating is null)
        {
            return NotFound("Rating not found.");
        }
        return Ok(_mapper.Map<RatingDto>(rating));
    }

    // POST /api/ratings
    [HttpPost(Name = "CreateRating")]
    public async Task<IActionResult> CreateAsync([FromBody] RatingDto dto)
    {

         if (dto == null)
            return BadRequest("Rating data is required.");

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
        return Created($"/api/ratings/user/{rating.UserId}/game/{rating.GameId}", _mapper.Map<RatingDto>(rating));
        
    }

    // PUT /api/ratings/{userId}/{gameId}
    [HttpPut("{userId}/{gameId}", Name = "UpdateRating")]
    public async Task<IActionResult> UpdateAsync(int userId, int gameId, [FromBody] RatingDto dto)
    {
        if (userId != dto.UserId || gameId != dto.GameId)
        {
            return BadRequest("Composite key mismatch");
        }

        var userRatings = await _service.GetByUserIdAsync(userId);
        var existing = userRatings.FirstOrDefault(r => r.GameId == gameId);
        if (existing == null)
            return NotFound("Rating not found.");

        _logger.LogInformation("Updating rating for user {UserId} and game {GameId}", userId, gameId);
        var rating = _mapper.Map<Rating>(dto);
        await _service.UpdateAsync(userId, gameId, rating);
        return Ok(_mapper.Map<RatingDto>(rating));
    }

    // DELETE /api/ratings/{userId}/{gameId}
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
}
