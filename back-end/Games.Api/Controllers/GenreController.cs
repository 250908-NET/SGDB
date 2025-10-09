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
public class GenresController : ControllerBase
{
    private readonly ILogger<GenresController> _logger;
    private readonly IGenreService _service;
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;

    public GenresController(ILogger<GenresController> logger, IGenreService genreService, IGameService gameService, IMapper mapper)
    {
        _logger = logger;
        _service = genreService;
        _gameService = gameService;
        _mapper = mapper;
    }

    // GET: receive all genres
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet(Name = "GetAllGenres")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all genres");
        var genres = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<GenreDto>>(genres));
    }

    // GET: receive genre by id
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("{id}", Name = "GetGenreById")]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        _logger.LogInformation("Getting genre {id}", id);
        var genre = await _service.GetByIdAsync(id);
        if (genre is null)
        {
            return NotFound("Genre not found");
        }
        return Ok(_mapper.Map<GenreDto>(genre));
    }

    // POST: create new genre
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost(Name = "CreateGenre")]
    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto dto)
    {
        _logger.LogInformation("Creating genre {@dto}", dto);
        var genre = _mapper.Map<Genre>(dto);
        await _service.CreateAsync(genre);

        foreach (var gameId in dto.Games.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found.");
            }
            await _gameService.LinkGameToGenreAsync(gameId, genre.GenreId);
        }
       
        return Created($"/api/genres/{genre.GenreId}", _mapper.Map<GenreDto>(genre));
    }

    // PUT: update genre
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPut("{id}", Name = "UpdateGenre")]
    public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto dto)
    {
        _logger.LogInformation("Updating genre {id}", id);
        var genre = await _service.GetByIdAsync(id);
        if (genre is null)
        {
            return NotFound("Genre not found");
        }

        _mapper.Map(dto, genre);
        await _service.UpdateAsync(genre);
        await _service.ClearGenreGamesAsync(id);

        foreach (var gameId in dto.Games.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found.");
            }
            await _gameService.LinkGameToGenreAsync(gameId, id);
        }

        return Ok(_mapper.Map<GenreDto>(genre));
    }

    // DELETE: delete genre
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete("{id}", Name = "DeleteGenre")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        _logger.LogInformation("Deleting genre {id}", id);
        var existing = await _service.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound("Genre not found");
        }
        await _service.ClearGenreGamesAsync(id);
        await _service.DeleteAsync(id);
        return NoContent();
    }
}