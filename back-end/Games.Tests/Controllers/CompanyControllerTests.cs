using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Games.Controllers;
using Games.Services;
using Games.DTOs;
using Games.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games.Tests.Controllers
{
    public class CompanyControllerTests
    {
        private readonly Mock<ICompanyService> _mockService;
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CompanyController>> _mockLogger;
        private readonly CompanyController _controller;

        public CompanyControllerTests()
        {
            _mockService = new Mock<ICompanyService>();
            _mockGameService = new Mock<IGameService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CompanyController>>();

            _controller = new CompanyController(_mockLogger.Object, _mockService.Object, _mockGameService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOk_WithListOfCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new() { CompanyId = 1, Name = "Valve" },
                new() { CompanyId = 2, Name = "Ubisoft" }
            };

            var companyDtos = new List<CompanyDto>
            {
                new() { CompanyId = 1, Name = "Valve" },
                new() { CompanyId = 2, Name = "Ubisoft" }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(companies);
            _mockMapper.Setup(m => m.Map<List<CompanyDto>>(companies)).Returns(companyDtos);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<List<CompanyDto>>(okResult.Value);
            Assert.Equal(2, returned.Count);
            Assert.Equal("Valve", returned[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOk_WhenCompanyExists()
        {
            // Arrange
            var company = new Company { CompanyId = 1, Name = "Square Enix" };
            var dto = new CompanyDto { CompanyId = 1, Name = "Square Enix" };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<CompanyDto>(company)).Returns(dto);

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<CompanyDto>(okResult.Value);
            Assert.Equal("Square Enix", returned.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Company?)null);

            var result = await _controller.GetByIdAsync(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Company not found", notFound.Value);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreated_WithMappedCompanyDto()
        {
            // Arrange
            var createDto = new CreateCompanyDto { Name = "Capcom" };
            var company = new Company { CompanyId = 1, Name = "Capcom" };
            var companyDto = new CompanyDto { CompanyId = 1, Name = "Capcom" };

            _mockMapper.Setup(m => m.Map<Company>(createDto)).Returns(company);
            _mockMapper.Setup(m => m.Map<CompanyDto>(company)).Returns(companyDto);

            _mockService.Setup(s => s.CreateAsync(company)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateAsync(createDto);

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            var returned = Assert.IsType<CompanyDto>(created.Value);
            Assert.Equal("Capcom", returned.Name);
            Assert.Equal(1, returned.CompanyId);
        }

        [Fact]
        public async Task UpdateCompany_ShouldReturnOk_WhenCompanyExists()
        {
            // Arrange
            var existing = new Company { CompanyId = 1, Name = "EA" };
            var updateDto = new UpdateCompanyDto { Name = "Electronic Arts" };
            var updatedDto = new CompanyDto { CompanyId = 1, Name = "Electronic Arts" };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockMapper.Setup(m => m.Map(updateDto, existing));
            _mockMapper.Setup(m => m.Map<CompanyDto>(existing)).Returns(updatedDto);

            _mockService.Setup(s => s.UpdateAsync(existing)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCompany(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<CompanyDto>(okResult.Value);
            Assert.Equal("Electronic Arts", returned.Name);
        }

        [Fact]
        public async Task UpdateCompany_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var dto = new UpdateCompanyDto { Name = "FakeCo" };
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Company?)null);

            // Act
            var result = await _controller.UpdateCompany(99, dto);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Company not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNoContent_WhenCompanyExists()
        {
            // Arrange
            var company = new Company { CompanyId = 1, Name = "Rockstar" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(company);
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            _mockService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((Company?)null);

            var result = await _controller.DeleteAsync(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Company not found", notFound.Value);
        }
    }
}
