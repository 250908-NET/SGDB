using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;
using AutoMapper;
using System.Collections.Generic;
using Games.Repositories;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    // private readonly GamesDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;
    private readonly ITokenServices _tokenservice;
    private readonly IAccessTokenRepository _accessTokenRepo;

    public AuthenticationController(ILogger<UserController> logger, IMapper mapper, IUserService service, ITokenServices tokenservice, IAccessTokenRepository accessTokenRepository)
    {
        //_context = context;
        _mapper = mapper;
        _service = service;
        _logger = logger;
        _tokenservice = tokenservice;
        _accessTokenRepo = accessTokenRepository;


    }

    // Get all users
    [HttpPost("CreateAccount")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("Creating Account {@dto}", dto);
        var user = _mapper.Map<User>(dto);
        await _service.AddUserAsync(user);
        User? USER = await _service.GetUserByusername(dto.username);
        string accessTokenString = _tokenservice.GenerateAccessToken();
        Response.Cookies.Append("access_token", accessTokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        AccessToken TokenObject = new AccessToken();
        TokenObject.Token = accessTokenString;
        TokenObject.UserId = USER.UserId;
        TokenObject.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
        TokenObject.CreatedAt = DateTime.UtcNow;


        //presisted to database 
        await _accessTokenRepo.AddAsync(TokenObject);
        await _accessTokenRepo.SaveChangesAsync();


        // return Created($"/api/users/{user.UserId}", _mapper.Map<UserDto>(user));
        return Ok(new { message = "Account Created" });
    }

    // Get a user by userId
    [HttpPost("LoginAccount")]
    public async Task<ActionResult<UserDto>> LoginAccount([FromBody] CreateUserDto dto)
    {
        var user = await _service.GetUserByusername(dto.username);
        if (user == null)
        {
            return NotFound(new { message = "Invalid username or password" });
        }
        string accessTokenString = _tokenservice.GenerateAccessToken();
        Response.Cookies.Append("access_token", accessTokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });
        AccessToken usersAccessTokenOld = await _accessTokenRepo.GetAllAccessTokenByUserId(user.UserId);
        // Persit to dataBase

        if (usersAccessTokenOld == null)
        {
            AccessToken TokenObject = new AccessToken();
            TokenObject.Token = accessTokenString;
            TokenObject.UserId = user.UserId;
            TokenObject.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
            TokenObject.CreatedAt = DateTime.UtcNow;
            await _accessTokenRepo.AddAsync(TokenObject);
            await _accessTokenRepo.SaveChangesAsync();
            return Ok(new { message = "logged in" });
        }
        usersAccessTokenOld.Token = accessTokenString;
        usersAccessTokenOld.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
        usersAccessTokenOld.CreatedAt = DateTime.UtcNow;

        await _accessTokenRepo.UpdateAsync(usersAccessTokenOld);
        await _accessTokenRepo.SaveChangesAsync();
        return Ok(new { message = "logged in" });
    }

    // Add a user to DB
    [HttpDelete("Logout")]
    public async Task<IActionResult> LogoutAccount()
    {
        if (Request.Cookies.TryGetValue("access_token", out var tokenString))
        {
            bool deleted = await _accessTokenRepo.DeleteAsync(tokenString);
            await _accessTokenRepo.SaveChangesAsync();

            if (deleted == true)
            {
                Response.Cookies.Delete("access_token");
                return Ok(new { message = "LoggedOut " });
            }
            // return Ok(new { message = tokenString });
            return Unauthorized();
        }
        return Unauthorized();
    }

    // [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("TestAuthorization")]
    public async Task<IActionResult> TestAuth()
    {
        var users = await _accessTokenRepo.GetAllAccessToken();

        return Ok(new { message = "All good", data = users });
    }



}


