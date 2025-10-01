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

    private readonly UserService _service;

    public UserController(GamesDbContext context, IMapper mapper, UserService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    //GET: receive all Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        // var Users = await _context.User.ToListAsync();
        // _context.User.get
        // var UserDTOs = _mapper.Map<IEnumerable<UserDto>>(Users);

        var Users = await _service.GetAllUsersAsync();
        return Ok(_mapper.Map<List<User>>(Users));
    }

}