using AutoMapper;
using QrMenuApi.Data.DtoModels;
using QrMenuApi.Data.Models;

namespace QrMenuApi.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Company, CompanyDto>().ReverseMap().ForMember(dest => dest.ParentCompanyId, opt => opt.MapFrom(src => 1));
            CreateMap<Restaurant, RestaurantDto>().ReverseMap().ForMember(dest=> dest.RegisterDate, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<Category, CategoryDto>().ReverseMap().ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<Food, FoodDto>().ReverseMap();

        }
    }
}
