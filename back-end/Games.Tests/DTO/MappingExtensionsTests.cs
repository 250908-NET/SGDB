using FluentAssertions;
using Games.DTOs;
using Games.Models;
using Games;
using System.Collections.Generic;

namespace Games.Tests.DTOs
{
    public class MappingExtensionsTests
    {
        [Fact]
        public void ToDto_ShouldMapGameToGameDto()
        {
            // Arrange
            var game = new Game
            {
                GameId = 1,
                Title = "Chrono Trigger",
                Developer = "Square",
                ReleaseYear = 1995,
                GamePlatforms = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        Platform = new Platform { Name = "SNES" }
                    }
                }
            };

            // Act
            var dto = game.ToDto();

            // Assert
            dto.Should().NotBeNull();
            dto.GameId.Should().Be(1);
            dto.Title.Should().Be("Chrono Trigger");
            dto.Developer.Should().Be("Square");
            dto.ReleaseYear.Should().Be(1995);
            dto.Platforms.Should().ContainSingle(p => p == "SNES");
        }

        [Fact]
        public void ToDto_WithGameWithoutPlatforms_ShouldReturnEmptyPlatformList()
        {
            var game = new Game
            {
                GameId = 2,
                Title = "Pokémon Platinum",
                Developer = "Game Freak",
                ReleaseYear = 2008,
                GamePlatforms = new List<GamePlatform>()
            };

            var dto = game.ToDto();

            dto.Platforms.Should().BeEmpty();
        }

        [Fact]
        public void ToDto_ShouldMapPlatformToPlatformDto()
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
                        Game = new Game { Title = "Spider-Man: Miles Morales" }
                    }
                }
            };

            // Act
            var dto = platform.ToDto();

            // Assert
            dto.Should().NotBeNull();
            dto.PlatformId.Should().Be(1);
            dto.Name.Should().Be("PlayStation 5");
            dto.Manufacturer.Should().Be("Sony");
            dto.ReleaseYear.Should().Be(2020);
            dto.Games.Should().ContainSingle(g => g == "Spider-Man: Miles Morales");
        }

        [Fact]
        public void ToDto_WithPlatformWithoutGames_ShouldReturnEmptyGameList()
        {
            var platform = new Platform
            {
                PlatformId = 2,
                Name = "Xbox Series X",
                Manufacturer = "Microsoft",
                ReleaseYear = 2020,
                GamePlatforms = new List<GamePlatform>()
            };

            var dto = platform.ToDto();

            dto.Games.Should().BeEmpty();
        }
    }
}
