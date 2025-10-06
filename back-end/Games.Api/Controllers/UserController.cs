using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;
using AutoMapper;
using System.Collections.Generic;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    //private readonly GamesDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;
    private readonly IGenreService _genreService;
    private readonly IGameService _gameService;

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserService service, IGenreService genreService, IGameService gameService)
    {
        //_context = context;
        _mapper = mapper;
        _service = service;
        _logger = logger;
        _genreService = genreService;
        _gameService = gameService;
    }

    // Get all users
    [HttpGet(Name = "GetAllUsers")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all users");
        var users = await _service.GetAllUsersAsync();
        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    // Get a user by userId    
    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        _logger.LogInformation("Getting user by id {id}", id);
        var user = await _service.GetUserByIdAsync(id);
        // Validate if username 
        if (user is null)
            return NotFound($"User with '{id}' not found");
        // return Ok(Users);
        return Ok(_mapper.Map<UserDto>(user));
    }

    // Get a user by username
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {   
        _logger.LogInformation("Getting user by username {username}", username);
        var user = await _service.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound($"User '{username}' not found.");

        return Ok(_mapper.Map<UserDto>(user));
    }

    // Add a user to DB
    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("Creating user {@dto}", dto);
        var user = _mapper.Map<User>(dto);
        await _service.AddUserAsync(user);
        return Created($"/api/users/{user.UserId}", _mapper.Map<UserDto>(user));
    }

    // Update a user by Id
    [HttpPut("{id}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateUserDto dto)
    {
        _logger.LogInformation("Updating user {id}", id);
        var user = await _service.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound("User not found");
        }

        _mapper.Map(dto, user);
        await _service.ChangeUserAsync(user);

        return Ok(_mapper.Map<UserDto>(user));
    }

    // Delete a user by Id
    [HttpDelete("{id}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting user {id}", id);
        var existing = await _service.GetUserByIdAsync(id);
        if (existing is null)
        {
            return NotFound("User not found");
        }
        await _service.RemoveUserAsync(id);
        return NoContent();
    }

    // Link user to genre
    [HttpPost("{userId}/genres/{genreId}")]
    public async Task<IActionResult> LinkUserToGenre(int userId, int genreId)
    {
        _logger.LogInformation("Linking user {userId} to genre {genreId}", userId, genreId);

        // Check to see if user exists
        var user = await _service.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        // Check to see if genre exists
        var genre = await _genreService.GetByIdAsync(genreId);
        if (genre is null)
        {
            return NotFound($"Genre with ID {genreId} not found.");
        }

        // Check if already linked
        if (user.UserGenres.Any(gp => gp.GenreId == genreId))
        {
            return BadRequest("This user is already linked to that genre.");
        }
        await _service.LinkUserToGenreAsync(userId, genreId);
        return Ok(new { Message = "Linked successfully", UserId = userId, GenreId = genreId });
    }

    [HttpDelete("{userId}/genres/{genreId}")]
    public async Task<IActionResult> DeleteUserGenre(int userId, int genreId)
    {
        _logger.LogInformation("Unlinking user {userId} from genre {genreId}", userId, genreId);
        // Check to see if user exists
        var user = await _service.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        // Check to see if genre exists
        var genre = await _genreService.GetByIdAsync(genreId);
        if (genre is null)
        {
            return NotFound($"Genre with ID {genreId} not found.");
        }

        // Check if link actually exists
        if (!user.UserGenres.Any(gp => gp.GenreId == genreId))
        {
            return BadRequest("This user is not linked to that genre.");
        }

        await _service.UnlinkUserFromGenreAsync(userId, genreId);
        return NoContent();
    }

    // Link user to game
    [HttpPost("{userId}/games/{gameId}")]
    public async Task<IActionResult> LinkUserToGame(int userId, int gameId)
    {
        _logger.LogInformation("Linking user {userId} to game {gameId}", userId, gameId);

        // Check to see if user exists
        var user = await _service.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        // Check to see if game exists
        var game = await _gameService.GetByIdAsync(gameId);
        if (game is null)
        {
            return NotFound($"Game with ID {gameId} not found.");
        }

        // Check if already linked
        if (user.GameLibrary.Any(ug => ug.GameId == gameId))
        {
            return BadRequest("This user is already linked to that game.");
        }

        await _service.LinkUserToGameAsync(userId, gameId);
        return Ok(new { Message = "Linked successfully", UserId = userId, GameId = gameId });
    }

    // Unlink user from game
    [HttpDelete("{userId}/games/{gameId}")]
    public async Task<IActionResult> DeleteUserGame(int userId, int gameId)
    {
        _logger.LogInformation("Unlinking user {userId} from game {gameId}", userId, gameId);

        // Check to see if user exists
        var user = await _service.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        // Check to see if game exists
        var game = await _gameService.GetByIdAsync(gameId);
        if (game is null)
        {
            return NotFound($"Game with ID {gameId} not found.");
        }

        // Check if link actually exists
        if (!user.GameLibrary.Any(ug => ug.GameId == gameId))
        {
            return BadRequest("This user is not linked to that game.");
        }

        await _service.UnlinkUserFromGameAsync(userId, gameId);
        return NoContent();
    }

}


