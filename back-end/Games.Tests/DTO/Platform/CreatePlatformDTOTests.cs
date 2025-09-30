using FluentAssertions;
using Games.DTOs;

namespace Games.Tests.DTOs
{
    public class CreatePlatformDtoTests
    {
        [Fact]
        public void CreatePlatformDto_ShouldDefaultStringsToEmpty()
        {
            // Act
            var dto = new CreatePlatformDto();

            // Assert
            dto.Name.Should().BeEmpty();
            dto.Manufacturer.Should().BeEmpty();
        }

        [Fact]
        public void CreatePlatformDto_ShouldAssignValuesCorrectly()
        {
            // Arrange
            var dto = new CreatePlatformDto
            {
                Name = "PlayStation 5",
                Manufacturer = "Sony",
                ReleaseYear = 2020
            };

            // Assert
            dto.Name.Should().Be("PlayStation 5");
            dto.Manufacturer.Should().Be("Sony");
            dto.ReleaseYear.Should().Be(2020);
        }

        [Fact]
        public void CreatePlatformDto_ShouldAllowEmptyValues()
        {
            // Arrange
            var dto = new CreatePlatformDto
            {
                Name = "",
                Manufacturer = "",
                ReleaseYear = 0
            };

            // Assert
            dto.Name.Should().BeEmpty();
            dto.Manufacturer.Should().BeEmpty();
            dto.ReleaseYear.Should().Be(0);
        }

        [Theory]
        [InlineData(-1980)]
        [InlineData(0)]
        [InlineData(2100)]
        public void CreatePlatformDto_ShouldAcceptAnyReleaseYear(int year)
        {
            // Arrange
            var dto = new CreatePlatformDto { ReleaseYear = year };

            // Assert
            dto.ReleaseYear.Should().Be(year);
        }
    }
}
