using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Games.Data;
using Games.Models;
using Games.Repositories;
using Xunit;

namespace Games.Tests.Repositories
{
    public class CompanyRepositoryTests : IDisposable
    {
        private readonly GamesDbContext _context;
        private readonly CompanyRepository _repository;

        public CompanyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GamesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new GamesDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new CompanyRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Company_To_Database()
        {
            var company = new Company { Name = "Test Studio" };

            await _repository.AddAsync(company);
            await _repository.SaveChangesAsync();

            var result = await _context.Companies.FirstOrDefaultAsync(c => c.Name == "Test Studio");

            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Studio");
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Companies()
        {
            var companies = new List<Company>
            {
                new Company { Name = "Studio A" },
                new Company { Name = "Studio B" }
            };

            await _context.Companies.AddRangeAsync(companies);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "Studio A");
            result.Should().Contain(c => c.Name == "Studio B");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Company_With_Games()
        {
            var company = new Company { Name = "Square Enix" };
            var game1 = new Game
            {
                Name = "FFVII",
                ReleaseDate = new DateTime(1997, 1, 31),
                Developer = company,
                Publisher = company
            };

            await _context.Companies.AddAsync(company);
            await _context.Games.AddAsync(game1);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(company.CompanyId);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Square Enix");
            result.DevelopedGames.Should().ContainSingle(g => g.Name == "FFVII");
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Company()
        {
            var company = new Company { Name = "Old Name" };
            await _repository.AddAsync(company);
            await _repository.SaveChangesAsync();

            company.Name = "New Name";
            await _repository.UpdateAsync(company);
            await _repository.SaveChangesAsync();

            var updated = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == company.CompanyId);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Company()
        {
            var company = new Company { Name = "Delete Me" };
            await _repository.AddAsync(company);
            await _repository.SaveChangesAsync();

            await _repository.DeleteAsync(company.CompanyId);
            await _repository.SaveChangesAsync();

            var deleted = await _context.Companies.FindAsync(company.CompanyId);

            deleted.Should().BeNull();
        }
    }
}
