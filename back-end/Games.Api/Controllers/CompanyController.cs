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
public class CompanyController : ControllerBase
{
    //private readonly GamesDbContext _context;
    private readonly ILogger<CompanyController> _logger;
    private readonly ICompanyService _service;
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;


    public CompanyController(ILogger<CompanyController> logger, ICompanyService service, IGameService gameService, IMapper mapper)
    {
        _logger = logger;
        _service = service;
        _mapper = mapper;
        _gameService = gameService;
    }

    // Get all companies
    [HttpGet(Name = "GetAllCompanies")]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all companies");
        var companies = await _service.GetAllAsync();
        return Ok(_mapper.Map<List<CompanyDto>>(companies));
    }

    // Get a company by companyId
    [HttpGet("{id}", Name = "GetCompanyById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting company {id}", id);
        var company = await _service.GetByIdAsync(id);
        if (company is null)
        {
            _logger.LogWarning("Company with ID {id} not found.", id);
            return NotFound("Company not found");
        }

        return Ok(_mapper.Map<CompanyDto>(company));
    }

    // Create company
    [HttpPost(Name = "CreateCompany")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCompanyDto dto)
    {
        _logger.LogInformation("Creating company {@dto}", dto);
        var company = _mapper.Map<Company>(dto);
        await _service.CreateAsync(company);

        foreach (var gameId in dto.DevelopedGames.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game is null)
            {
                _logger.LogWarning("Developed game ID {id} not found.", gameId);
                return NotFound($"Developed game ID {gameId} not found.");
            }

            game.DeveloperId = company.CompanyId;
            await _gameService.UpdateAsync(game);
        }

        foreach (var gameId in dto.PublishedGames.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game is null)
            {
                _logger.LogWarning("Published game ID {id} not found.", gameId);
                return NotFound($"Published game ID {gameId} not found.");
            }

            game.PublisherId = company.CompanyId;
            await _gameService.UpdateAsync(game);
        }

        
        return Created($"/api/company/{company.CompanyId}", _mapper.Map<CompanyDto>(company));
    }

    // Update company
    [HttpPut("{id}", Name = "UpdateCompany")]
    public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        _logger.LogInformation("Updating company {id}", id);
        var updatedCompany = await _service.GetByIdAsync(id);

        // Check to see if company exists
        if (updatedCompany is null)
        {
            return NotFound("Company not found");
        }

        // Check and update referenced developed games
        
        foreach (var gameId in dto.DevelopedGames.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game is null)
            {
                _logger.LogWarning("Developed game ID {id} not found.", gameId);
                return NotFound($"Developed game ID {gameId} not found.");
            }

            game.DeveloperId = id;
            await _gameService.UpdateAsync(game);
        }

        // Check and update referenced published games
        foreach (var gameId in dto.PublishedGames.Distinct())
        {
            var game = await _gameService.GetByIdAsync(gameId);
            if (game is null)
            {
                _logger.LogWarning("Published game ID {id} not found.", gameId);
                return NotFound($"Published game ID {gameId} not found.");
            }

            game.PublisherId = id;
            await _gameService.UpdateAsync(game);
        }


        return Ok(_mapper.Map<CompanyDto>(updatedCompany));
    }

    /// DELETE: delete company
    [HttpDelete("{id}", Name = "DeleteCompany")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting company {id}", id);
        var company = await _service.GetByIdAsync(id);

        if (company is null)
        {
            _logger.LogWarning("Company with ID {id} not found.", id);
            return NotFound("Company not found");
        }

        // Not efficient but check every game to see if it has a foreign reference to the to be deleted company
        bool hasDevelopedGames = company.DevelopedGames.Any();
        bool hasPublishedGames = company.PublishedGames.Any();

        if (hasDevelopedGames || hasPublishedGames)
        {
            _logger.LogWarning("Cannot delete company {id}", id);
            return BadRequest("Cannot delete company — it’s still referenced by existing games.");
        }
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
