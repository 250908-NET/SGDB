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
[Route("api/users/[controller]")]
public class UserController : ControllerBase
{
    private readonly GamesDbContext _context;
    private readonly IMapper _mapper;

    private readonly IUserService _service;

    public UserController(GamesDbContext context, IMapper mapper, IUserService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    //GET: receive all Users
    [HttpPost]
    public async Task<ActionResult<IEnumerable<UserDto>>> CreateUsers(CreateUserDto DTO)
    {
        User Users = await _service.CreateUserAsync(DTO);
        return Ok(Users);
        // return Ok(_mapper.Map<UserDto>(Users));
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        List<User> Users = await _service.GetAllUsersAsync();
        // return Ok(Users);
        return Ok(_mapper.Map<List<UserDto>>(Users));
    }

}