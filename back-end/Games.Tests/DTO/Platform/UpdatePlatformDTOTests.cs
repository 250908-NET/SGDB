using FluentAssertions;
using Games.DTOs;

namespace Games.Tests.DTOs
{
    public class UpdatePlatformDtoTests
    {
        [Fact]
        public void UpdatePlatformDto_ShouldDefaultStringsToEmpty()
        {
            // Act
            var dto = new UpdatePlatformDto();

            // Assert
            dto.Name.Should().BeEmpty();
            dto.Manufacturer.Should().BeEmpty();
        }

        [Fact]
        public void UpdatePlatformDto_ShouldAssignValuesCorrectly()
        {
            // Arrange
            var dto = new UpdatePlatformDto
            {
                Name = "Xbox Series X",
                Manufacturer = "Microsoft",
                ReleaseYear = 2020
            };

            // Assert
            dto.Name.Should().Be("Xbox Series X");
            dto.Manufacturer.Should().Be("Microsoft");
            dto.ReleaseYear.Should().Be(2020);
        }

        [Fact]
        public void UpdatePlatformDto_ShouldAllowEmptyValues()
        {
            // Arrange
            var dto = new UpdatePlatformDto
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
        [InlineData(-1970)]
        [InlineData(0)]
        [InlineData(2099)]
        public void UpdatePlatformDto_ShouldAcceptAnyReleaseYear(int year)
        {
            // Arrange
            var dto = new UpdatePlatformDto { ReleaseYear = year };

            // Assert
            dto.ReleaseYear.Should().Be(year);
        }
    }
}
