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
            CreateMap<Game, GameDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>();
            CreateMap<UpdateGameDto, Game>();

            // Platform mappings
            CreateMap<Platform, PlatformDto>().ReverseMap();
            CreateMap<CreatePlatformDto, Platform>();
            CreateMap<UpdatePlatformDto, Platform>();

            // Company mappings
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();

            // Genre mappings
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<CreateGenreDto, Genre>();
            CreateMap<UpdateGenreDto, Genre>();
        }
    }
}
