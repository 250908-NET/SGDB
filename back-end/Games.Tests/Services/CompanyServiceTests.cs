using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Games.Models;
using Games.Repositories;
using Games.Services;

namespace Games.Tests.Services
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> _mockRepo;
        private readonly CompanyService _service;

        public CompanyServiceTests()
        {
            _mockRepo = new Mock<ICompanyRepository>();
            _service = new CompanyService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Name = "Nintendo" },
                new Company { CompanyId = 2, Name = "Sony" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(companies);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Nintendo");
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCompany_WhenExists()
        {
            // Arrange
            var company = new Company { CompanyId = 1, Name = "Valve" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Valve", result!.Name);
            Assert.Equal(1, result.CompanyId);
            _mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Company?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            // Arrange
            var company = new Company { CompanyId = 1, Name = "Capcom" };

            // Act
            await _service.CreateAsync(company);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(company), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            // Arrange
            var company = new Company { CompanyId = 1, Name = "Ubisoft" };

            // Act
            await _service.UpdateAsync(company);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(company), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAndSave()
        {
            // Arrange
            int id = 5;

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CompanyService(null!));
        }
    }
}
