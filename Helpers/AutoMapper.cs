using AutoMapper;
using HKShop.Models;
using HKShop.DTOs;

namespace HKShop.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<CustomerRequestDto, Customer>()
                .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FullName))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.Gender))
                .ForMember(d => d.BirthDate, opt => opt.MapFrom(s => s.BirthDate))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Image, opt => opt.MapFrom(s => s.ImageUrl));

            CreateMap<CustomerRequestDto, User>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.CustomerId))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role));
        }
    }
}
