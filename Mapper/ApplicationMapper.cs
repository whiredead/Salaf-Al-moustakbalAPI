using AutoMapper;
using SalafAlmoustakbalAPI.DTOs;
using SalafAlmoustakbalAPI.Models;
namespace SalafAlmoustakbalAPI.Mapper
{
    public class ApplicationMapper: Profile 
    {
        public ApplicationMapper()
        {
            CreateMap<Bar, BarDto>()
                .ForMember(dest => dest.MenusDto, opt => opt.MapFrom(src => src.Menus));
            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.BarId, opt => opt.MapFrom(src => src.Bar.Id));
        }
    }
}
