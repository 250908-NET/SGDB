using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Games.Data;
using Games.Models;
using Games.DTOs;

namespace Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly GamesDbContext _context;
    private readonly ILogger<GamesController> _logger;


    public CompanyController(GamesDbContext context)
    {
        //_logger = logger;
        _context = context;
        //_service = service;
    }

    // GET: /Company
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
    {
        var companies = await _context.Companies
            .Include(c => c.DevelopedGames)
            .Include(c => c.PublishedGames)
            .ToListAsync();

        var result = companies.Select(c => new CompanyDto
        {
            CompanyId = c.CompanyId,
            Name = c.Name,
            DevelopedGames = c.DevelopedGames.Select(g => g.Name).ToList(),
            PublishedGames = c.PublishedGames.Select(g => g.Name).ToList()
        });

        return Ok(result);
    }

    // GET: /Company/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id)
    {
        var company = await _context.Companies
            .Include(c => c.DevelopedGames)
            .Include(c => c.PublishedGames)
            .FirstOrDefaultAsync(c => c.CompanyId == id);

        if (company is null) return NotFound("Company not found");

        var result = new CompanyDto
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            DevelopedGames = company.DevelopedGames.Select(g => g.Name).ToList(),
            PublishedGames = company.PublishedGames.Select(g => g.Name).ToList()
        };

        return Ok(result);
    }

    // POST: /Company
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto dto)
    {
        var company = new Company
        {
            Name = dto.Name,
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCompany), new { id = company.CompanyId }, new CompanyDto
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            DevelopedGames = new List<string>(),
            PublishedGames = new List<string>()
        });
    }

    // PUT: /Company/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return NotFound("Company not found");

        company.Name = dto.Name;

        await _context.SaveChangesAsync();

        return Ok(new CompanyDto
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            DevelopedGames = company.DevelopedGames.Select(g => g.Name).ToList(),
            PublishedGames = company.PublishedGames.Select(g => g.Name).ToList()
        });
    }

    // DELETE: /Company/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return NotFound("Company not found");

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
