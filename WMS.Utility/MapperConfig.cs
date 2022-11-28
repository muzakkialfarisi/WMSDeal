using AutoMapper;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.Utility
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<SecMenu, MenuViewModel>().ReverseMap();
            CreateMap<SecUser, UserViewModel>().ReverseMap();
        }
    }
}
