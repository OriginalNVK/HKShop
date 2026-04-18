using AutoMapper;
using HKShop.Models;
using HKShop.DTOs;

namespace HKShop.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<ClientRequest, Customer>()
                .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.MaKH))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.HoTen))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.GioiTinh))
                .ForMember(d => d.BirthDate, opt => opt.MapFrom(s => s.NgaySinh))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.DiaChi))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.DienThoai))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Hinh));

            CreateMap<ClientRequest, User>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.MaKH))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.VaiTro));
        }
    }
}
