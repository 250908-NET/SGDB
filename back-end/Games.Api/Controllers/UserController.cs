using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Services;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService service, ILogger<UserController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // POST /api/user
    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateAsync()
    {
        _logger.LogInformation("Creating new user");

        var newUser = new User(); // Empty user
        var createdUser = await _service.CreateAsync(newUser);

        return Created($"/api/user/{createdUser.UserID}", createdUser);
    }
}
