using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Games.Controllers;
using Games.Services;
using Games.Models;
using Games.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly Mock<IGameService> _mockGameService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockGenreService = new Mock<IGenreService>();
            _mockGameService = new Mock<IGameService>();

            _controller = new UserController(
                _mockLogger.Object,
                _mockMapper.Object,
                _mockUserService.Object,
                _mockGenreService.Object,
                _mockGameService.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, username = "TestUser", role = "Player" }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { UserId = 1, username = "TestUser", role = "Player" }
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("TestUser", returnValue[0].username);
            Assert.Equal("Player", returnValue[0].role);
        }

        [Fact]
        public async Task GetUser_ReturnsOk_WhenUserExists()
        {
            var user = new User { UserId = 1, username = "Player1", role = "Admin" };
            var userDto = new UserDto { UserId = 1, username = "Player1", role = "Admin" };

            _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            var result = await _controller.GetUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("Player1", returnValue.username);
            Assert.Equal("Admin", returnValue.role);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.GetUser(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreated_WhenUserIsAdded()
        {
            var createDto = new CreateUserDto { username = "NewUser", role = "Player" };
            var user = new User { UserId = 1, username = "NewUser", role = "Player" };
            var userDto = new UserDto { UserId = 1, username = "NewUser", role = "Player" };

            _mockMapper.Setup(m => m.Map<User>(createDto)).Returns(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            var result = await _controller.CreateAsync(createDto);

            var createdResult = Assert.IsType<CreatedResult>(result);
            var returnValue = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal("NewUser", returnValue.username);
            Assert.Equal("Player", returnValue.role);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenUserExists()
        {
            var user = new User { UserId = 1, username = "ToDelete" };
            _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _controller.DeleteAsync(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.DeleteAsync(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
