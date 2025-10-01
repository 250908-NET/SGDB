using Games.Models;

namespace Games.DTOs;

public static class MappingExtensions
{

    // GAME
    public static GameDto ToDto(this Game game)
    {
        return new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            PublisherId = game.PublisherId,
            DeveloperId = game.DeveloperId,
            //Platforms = game.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
            Platforms = game.GamePlatforms != null
                    ? game.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
                    : new List<string>()
        };
    }

    // PLATFORM TBD
    public static PlatformDto ToDto(this Platform platform)
    {
        return new PlatformDto
        {
            PlatformId = platform.PlatformId,
            Name = platform.Name,
            Games = platform.GamePlatforms?.Select(gp => gp.Game.Name).ToList() ?? new List<string>()
        };
    }

    // COMPANY
    public static CompanyDto ToDto(this Company company)
    {
        return new CompanyDto
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            DevelopedGames = company.DevelopedGames?
                .Select(g => g.Name)
                .ToList() ?? new List<string>(),
            PublishedGames = company.PublishedGames?
                .Select(g => g.Name)
                .ToList() ?? new List<string>()
        };
    }
}