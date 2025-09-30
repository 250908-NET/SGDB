using FluentAssertions;
using Games.DTOs;

namespace Games.Tests.DTOs
{
    public class CreateGameDtoTests
    {
        [Fact]
        public void CreateGameDto_ShouldDefaultStringsToEmpty()
        {
            // Act
            var dto = new CreateGameDto();

            // Assert
            dto.Title.Should().BeEmpty();
            dto.Developer.Should().BeEmpty();
        }

        [Fact]
        public void CreateGameDto_ShouldAssignValuesCorrectly()
        {
            // Arrange
            var dto = new CreateGameDto
            {
                Title = "Final Fantasy VI",
                Developer = "Square",
                ReleaseYear = 1994
            };

            // Assert
            dto.Title.Should().Be("Final Fantasy VI");
            dto.Developer.Should().Be("Square");
            dto.ReleaseYear.Should().Be(1994);
        }

        [Fact]
        public void CreateGameDto_ShouldAllowEmptyValues()
        {
            // Arrange
            var dto = new CreateGameDto
            {
                Title = "",
                Developer = "",
                ReleaseYear = 0
            };

            // Assert
            dto.Title.Should().BeEmpty();
            dto.Developer.Should().BeEmpty();
            dto.ReleaseYear.Should().Be(0);
        }

        [Theory]
        [InlineData(-1985)]
        [InlineData(0)]
        [InlineData(3000)]
        public void CreateGameDto_ShouldAcceptAnyReleaseYear(int year)
        {
            // Arrange
            var dto = new CreateGameDto { ReleaseYear = year };

            // Assert
            dto.ReleaseYear.Should().Be(year);
        }
    }
}
