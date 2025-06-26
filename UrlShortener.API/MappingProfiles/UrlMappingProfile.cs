using AutoMapper;
using UrlShortener.API.Models.DTOs.Urls;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.MappingProfiles;

public class UrlMappingProfile : Profile
{
    public UrlMappingProfile()
    {
        CreateMap<Url, UrlDto>()
           .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.UserName));
    }
}
