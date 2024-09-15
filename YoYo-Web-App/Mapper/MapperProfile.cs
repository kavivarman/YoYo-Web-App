using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using YoYo_Web_App.Models;
using YoYo_Web_App.ViewModels;

namespace YoYo_Web_App.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AthleteInfo, AthleteInfo>();
            CreateMap<IEnumerable<AthleteInfo>, YoyoViewModel>()
                .ForMember(dest => dest.AthleteInfos, opt => opt.MapFrom(src => src));

            CreateMap<SelectListItem, SelectListItem>();
            CreateMap<IEnumerable<SelectListItem>, YoyoViewModel>()
                .ForMember(dest => dest.LevelShuttles, opt => opt.MapFrom(src => src));
        }
    }
}
