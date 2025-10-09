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

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserService service)
    {
        //_context = context;
        _mapper = mapper;
        _service = service;
        _logger = logger;
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
        // Validate if username exists
        if (user is null)
            return NotFound($"User with '{id}' not found");
            
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
    [Microsoft.AspNetCore.Authorization.Authorize]
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
    [Microsoft.AspNetCore.Authorization.Authorize]
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


    // LINKING LOGIC
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost("{userId}/genres/{genreId}")]
    public async Task<IActionResult> LinkUserToGenre(int userId, int genreId)
    {
        await _service.LinkUserToGenreAsync(userId, genreId);
        return Ok(new { Message = "Linked successfully", UserId = userId, GenreId = genreId });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete("{userId}/genres/{genreId}")]
    public async Task<IActionResult> UnlinkUserFromGenre(int userId, int genreId)
    {
        await _service.UnlinkUserFromGenreAsync(userId, genreId);
        return NoContent();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost("{userId}/games/{gameId}")]
    public async Task<IActionResult> LinkUserToGame(int userId, int gameId)
    {
        await _service.LinkUserToGameAsync(userId, gameId);
        return Ok(new { Message = "Linked successfully", UserId = userId, GameId = gameId });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete("{userId}/games/{gameId}")]
    public async Task<IActionResult> UnlinkUserFromGame(int userId, int gameId)
    {
        await _service.UnlinkUserFromGameAsync(userId, gameId);
        return NoContent();
    }

}


