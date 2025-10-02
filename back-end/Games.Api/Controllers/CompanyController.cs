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
    private readonly IMapper _mapper;


    public CompanyController(ILogger<CompanyController> logger, ICompanyService service, IMapper mapper)
    {
        _logger = logger;
        _service = service;
        _mapper = mapper;
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
        if (company is null) return NotFound("Company not found");
        return Ok(_mapper.Map<CompanyDto>(company));
    }

    // Create company
    [HttpPost(Name = "CreateCompany")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCompanyDto dto)
    {
        _logger.LogInformation("Creating company {@dto}", dto);
        var company = _mapper.Map<Company>(dto);
        await _service.CreateAsync(company);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = company.CompanyId }, _mapper.Map<CompanyDto>(company));
    }

    // Update company
    [HttpPut("{id}", Name = "UpdateCompany")]
    public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        _logger.LogInformation("Updating company {id}", id);
        var company = await _service.GetByIdAsync(id);
        if (company is null)
        {
            return NotFound("Company not found");
        }

        _mapper.Map(dto, company);
        await _service.UpdateAsync(company);

        return Ok(_mapper.Map<CompanyDto>(company));
    }

    /// DELETE: delete company
    [HttpDelete("{id}", Name = "DeleteCompany")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting company {id}", id);
        var company = await _service.GetByIdAsync(id);
        if (company is null) return NotFound("Company not found");

        await _service.DeleteAsync(id);
        return NoContent();
    }
}
