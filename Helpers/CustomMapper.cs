using HKShop.Domain;
using HKShop.DTOs;
using AutoMapper;

namespace HKShop.Helpers
{
    public class CustomMapper : Profile
    {
        public CustomMapper()
        {
            CreateMap<CustomerRequestDto, Customer>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.Fullname, opt => opt.MapFrom(s => s.FullName))
                .ForMember(d => d.Gender, opt => opt.MapFrom(s => s.Gender))
                .ForMember(d => d.Birthday, opt => opt.MapFrom(s => s.BirthDate))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Avatar, opt => opt.MapFrom(s => s.ImageUrl));

            CreateMap<CustomerRequestDto, AppUser>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.CustomerId))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role));
        }
    }
}
