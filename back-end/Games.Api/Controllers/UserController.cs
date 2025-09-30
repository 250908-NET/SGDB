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

    public UserController(GamesDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }

    //GET: receive all Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var Users = await _context.User.ToListAsync();

        var UserDTOs = _mapper.Map<IEnumerable<UserDto>>(Users);

        return Ok(UserDTOs);
    }

}