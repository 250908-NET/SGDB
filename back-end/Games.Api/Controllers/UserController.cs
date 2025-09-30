using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Services;
using Games.DTOs;
using Games.Models;
using Games.Data;

namespace Games.Controllers;

[ApiController]
[Route("api/users/[controller]")]
public class UserController : ControllerBase
{
    private readonly GamesDbContext _context;

    public UserController(GamesDbContext context)
    {
        _context = context;
    }

    // GET: receive all platforms
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<PlatformDto>>> GetUsers()
    // {
    //     var Users = await _context.
    //         .Include(p => p.GamePlatforms)
    //         .ThenInclude(gp => gp.Game)
    //         .ToListAsync();

    //     var result = platforms.Select(p => new PlatformDto
    //     {
    //         PlatformId = p.PlatformId,
    //         Name = p.Name,
    //         Games = p.GamePlatforms.Select(gp => gp.Game.Name).ToList()
    //     });

    //     return Ok(result);
    // }

}