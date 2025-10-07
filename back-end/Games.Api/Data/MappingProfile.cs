using AutoMapper;
using Games.Models;
using Games.DTOs;

namespace Games.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Game mappings
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Platforms,
                    opt => opt.MapFrom(src =>
                        src.GamePlatforms != null
                            ? src.GamePlatforms.Select(gp => gp.Platform.PlatformId).ToList()
                            : new List<int>()
                    ))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src =>
                        src.GameGenres != null
                            ? src.GameGenres.Select(gg => gg.Genre.GenreId).ToList()
                            : new List<int>()
                    ));
            CreateMap<CreateGameDto, Game>();
            CreateMap<UpdateGameDto, Game>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Condition(src => !string.IsNullOrEmpty(src.ImageUrl)));

            // Platform mappings
            CreateMap<Platform, PlatformDto>()
                .ForMember(dest => dest.Games,
                    opt => opt.MapFrom(src =>
                        src.GamePlatforms != null
                            ? src.GamePlatforms.Select(gp => gp.Game.Name).ToList()
                            : new List<string>()
                    ));

            CreateMap<CreatePlatformDto, Platform>();
            CreateMap<UpdatePlatformDto, Platform>();

            // Company mappings
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.DevelopedGames,
                    opt => opt.MapFrom(src =>
                        src.DevelopedGames != null
                            ? src.DevelopedGames.Select(g => g.GameId).ToList()
                            : new List<int>()
                    ))
                .ForMember(dest => dest.PublishedGames,
                    opt => opt.MapFrom(src =>
                        src.PublishedGames != null
                            ? src.PublishedGames.Select(g => g.GameId).ToList()
                            : new List<int>()
                    ));

            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.DevelopedGames, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedGames, opt => opt.Ignore());

            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.DevelopedGames, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedGames, opt => opt.Ignore());

            //Rating mappings
            CreateMap<Rating, RatingDto>().ReverseMap();

            // Genre mappings
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<CreateGenreDto, Genre>();
            CreateMap<UpdateGenreDto, Genre>();

            // User mappings
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserGenres,
                    opt => opt.MapFrom(src =>
                        src.UserGenres != null
                            ? src.UserGenres.Select(ug => ug.GenreId).ToList()
                            : new List<int>()
                    ))
                .ForMember(dest => dest.GameLibrary,
                    opt => opt.MapFrom(src =>
                        src.GameLibrary != null
                            ? src.GameLibrary.Select(ug => ug.GameId).ToList()
                            : new List<int>()
                    ));

        }
    }
}
