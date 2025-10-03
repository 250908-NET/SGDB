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
            // CreateMap<Game, GameDto>()
            //     .ForMember(dest => dest.Platforms,
            //         opt => opt.MapFrom(src =>
            //             src.GamePlatforms != null
            //                 ? src.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
            //                 : new List<string>()
            //         ));
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Platforms,
                    opt => opt.MapFrom(src =>
                        src.GamePlatforms != null
                            ? src.GamePlatforms.Select(gp => gp.Platform.Name).ToList()
                            : new List<string>()
                    ))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src =>
                        src.GameGenres != null
                            ? src.GameGenres.Select(gg => gg.Genre.Name).ToList()
                            : new List<string>()
                    ));
            CreateMap<CreateGameDto, Game>();
            CreateMap<UpdateGameDto, Game>();

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
                            ? src.DevelopedGames.Select(g => g.Name).ToList()
                            : new List<string>()
                    ))
                .ForMember(dest => dest.PublishedGames,
                    opt => opt.MapFrom(src =>
                        src.PublishedGames != null
                            ? src.PublishedGames.Select(g => g.Name).ToList()
                            : new List<string>()
                    ));
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();

            //Rating mappings
            CreateMap<Rating, RatingDto>().ReverseMap();

            // Genre mappings
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<CreateGenreDto, Genre>();
            CreateMap<UpdateGenreDto, Genre>();

            // User mappings
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}
