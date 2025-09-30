using FluentAssertions;
using Games.DTOs;
using Games.Models;

namespace Games.Tests.DTOs
{
    public class PlatformDtoMappingTests
    {
        [Fact]
        public void Platform_ToDto_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var platform = new Platform
            {
                PlatformId = 1,
                Name = "PlayStation 5",
                Manufacturer = "Sony",
                ReleaseYear = 2020,
                GamePlatforms = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        Game = new Game
                        {
                            GameId = 10,
                            Title = "Spider-Man: Miles Morales",
                            Developer = "Insomniac",
                            ReleaseYear = 2020
                        }
                    },
                    new GamePlatform
                    {
                        Game = new Game
                        {
                            GameId = 11,
                            Title = "Demon's Souls",
                            Developer = "Bluepoint",
                            ReleaseYear = 2020
                        }
                    }
                }
            };

            // Act
            var dto = platform.ToDto();

            // Assert
            dto.PlatformId.Should().Be(1);
            dto.Name.Should().Be("PlayStation 5");
            dto.Manufacturer.Should().Be("Sony");
            dto.ReleaseYear.Should().Be(2020);
            dto.Games.Should().Contain(new[] { "Spider-Man: Miles Morales", "Demon's Souls" });
        }

        [Fact]
        public void Platform_ToDto_ShouldHandlePlatformWithoutGames()
        {
            // Arrange
            var platform = new Platform
            {
                PlatformId = 2,
                Name = "Nintendo Switch",
                Manufacturer = "Nintendo",
                ReleaseYear = 2017,
                GamePlatforms = new List<GamePlatform>() // no games
            };

            // Act
            var dto = platform.ToDto();

            // Assert
            dto.Games.Should().BeEmpty();
        }

        [Fact]
        public void PlatformDto_DefaultConstructor_ShouldInitializeGames()
        {
            // Act
            var dto = new PlatformDto();

            // Assert
            dto.Games.Should().NotBeNull();
            dto.Games.Should().BeEmpty();
        }
    }
}
