using FluentAssertions;
using Games.DTOs;

namespace Games.Tests.DTOs
{
    public class UpdateGameDtoTests
    {
        [Fact]
        public void UpdateGameDto_ShouldDefaultStringsToEmpty()
        {
            // Act
            var dto = new UpdateGameDto();

            // Assert
            dto.Title.Should().BeEmpty();
            dto.Developer.Should().BeEmpty();
        }

        [Fact]
        public void UpdateGameDto_ShouldAssignValuesCorrectly()
        {
            // Arrange
            var dto = new UpdateGameDto
            {
                Title = "Pokémon Platinum",
                Developer = "Game Freak",
                ReleaseYear = 2008
            };

            // Assert
            dto.Title.Should().Be("Pokémon Platinum");
            dto.Developer.Should().Be("Game Freak");
            dto.ReleaseYear.Should().Be(2008);
        }

        [Fact]
        public void UpdateGameDto_ShouldAllowEmptyValues()
        {
            // Arrange
            var dto = new UpdateGameDto
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
        [InlineData(-1990)]
        [InlineData(0)]
        [InlineData(2050)]
        public void UpdateGameDto_ShouldAcceptAnyReleaseYear(int year)
        {
            // Arrange
            var dto = new UpdateGameDto { ReleaseYear = year };

            // Assert
            dto.ReleaseYear.Should().Be(year);
        }
    }
}
