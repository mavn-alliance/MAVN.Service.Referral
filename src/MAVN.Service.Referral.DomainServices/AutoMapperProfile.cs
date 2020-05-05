using AutoMapper;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.DomainServices
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ReferralHotelEncrypted, ReferralHotel>(MemberList.Destination)
                .ForMember(src => src.Email, opt => opt.Ignore())
                .ForMember(src => src.PhoneNumber, opt => opt.Ignore())
                .ForMember(src => src.FullName, opt => opt.Ignore());

            CreateMap<ReferralHotel, ReferralHotelWithProfile>(MemberList.Source)
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore());
        }
    }
}
