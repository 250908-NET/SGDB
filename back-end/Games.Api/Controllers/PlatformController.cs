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
public class PlatformsController : ControllerBase
{
    private readonly ILogger<PlatformsController> _logger;
    private readonly IPlatformService _service;
    private readonly IMapper _mapper;

    public PlatformsController(ILogger<PlatformsController> logger, IPlatformService platformService, IMapper mapper)
    {
        _logger = logger;
        _service = platformService;
        _mapper = mapper;
    }

    // GET: receive all platforms
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet(Name = "GetAllPlatforms")]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatforms()
    {
        _logger.LogInformation("Getting all platforms");
        var platforms = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<PlatformDto>>(platforms));
    }

    // GET: receive platform by id
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("{id}", Name = "GetPlatformById")]
    public async Task<ActionResult<PlatformDto>> GetPlatform(int id)
    {
        _logger.LogInformation("Getting platform {id}", id);
        var platform = await _service.GetByIdAsync(id);
        if (platform is null)
            return NotFound("Platform not found");
        return Ok(_mapper.Map<PlatformDto>(platform));
    }

    // POST: create new platform
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost(Name = "CreatePlatform")]
    public async Task<ActionResult<PlatformDto>> CreatePlatform(CreatePlatformDto dto)
    {
        _logger.LogInformation("Creating platform {@dto}", dto);
        var platform = _mapper.Map<Platform>(dto);
        await _service.CreateAsync(platform);
        return Created($"/api/platforms/{platform.PlatformId}", _mapper.Map<PlatformDto>(platform));
    }

    // PUT: update platform
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPut("{id}", Name = "UpdatePlatform")]
    public async Task<IActionResult> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        _logger.LogInformation("Updating platform {id}", id);
        var platform = await _service.GetByIdAsync(id);
        if (platform is null)
        {
            return NotFound("Platform not found");
        }

        _mapper.Map(dto, platform);
        await _service.UpdateAsync(platform);

        return Ok(_mapper.Map<PlatformDto>(platform));
    }

    // DELETE: delete platform
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete("{id}", Name = "DeletePlatform")]
    public async Task<IActionResult> DeletePlatform(int id)
    {
        _logger.LogInformation("Deleting platform {id}", id);
        var existing = await _service.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound("Platform not found");
        }
        await _service.DeleteAsync(id);
        return NoContent();
    }
}