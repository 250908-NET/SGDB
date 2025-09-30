using FluentAssertions;
using Games.DTOs;
using Games.Models;

namespace Games.Tests.DTOs
{
    public class GameDtoMappingTests
    {
        [Fact]
        public void Game_ToDto_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var game = new Game
            {
                GameId = 1,
                Title = "Final Fantasy VI",
                Developer = "Square",
                ReleaseYear = 1994,
                GamePlatforms = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        Platform = new Platform
                        {
                            PlatformId = 1,
                            Name = "SNES",
                            Manufacturer = "Nintendo",
                            ReleaseYear = 1990
                        }
                    },
                    new GamePlatform
                    {
                        Platform = new Platform
                        {
                            PlatformId = 2,
                            Name = "PlayStation",
                            Manufacturer = "Sony",
                            ReleaseYear = 1994
                        }
                    }
                }
            };

            // Act
            var dto = game.ToDto();

            // Assert
            dto.GameId.Should().Be(1);
            dto.Title.Should().Be("Final Fantasy VI");
            dto.Developer.Should().Be("Square");
            dto.ReleaseYear.Should().Be(1994);
            dto.Platforms.Should().Contain(new[] { "SNES", "PlayStation" });
        }

        [Fact]
        public void Game_ToDto_ShouldHandleGameWithoutPlatforms()
        {
            // Arrange
            var game = new Game
            {
                GameId = 2,
                Title = "Pokémon Platinum",
                Developer = "Game Freak",
                ReleaseYear = 2008,
                GamePlatforms = new List<GamePlatform>() // no platforms
            };

            // Act
            var dto = game.ToDto();

            // Assert
            dto.Platforms.Should().BeEmpty();
        }

        [Fact]
        public void GameDto_DefaultConstructor_ShouldInitializePlatforms()
        {
            // Act
            var dto = new GameDto();

            // Assert
            dto.Platforms.Should().NotBeNull();
            dto.Platforms.Should().BeEmpty();
        }
    }
}
