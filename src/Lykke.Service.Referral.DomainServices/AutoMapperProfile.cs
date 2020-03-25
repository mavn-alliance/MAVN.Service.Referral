using AutoMapper;
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.DomainServices
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ReferralHotelEncrypted, ReferralHotel>(MemberList.Destination)
                .ForMember(src => src.Email, opt => opt.Ignore())
                .ForMember(src => src.PhoneNumber, opt => opt.Ignore())
                .ForMember(src => src.FullName, opt => opt.Ignore());

            CreateMap<ReferralLead, ReferralLeadEncrypted>(MemberList.Source)
                .ForSourceMember(src => src.FirstName, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.LastName, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.PhoneNumber, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.Email, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.Note, opt => opt.DoNotValidate());

            CreateMap<ReferralLeadEncrypted, ReferralLead>(MemberList.Destination)
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Note, opt => opt.Ignore());

            CreateMap<ReferralLeadEncryptedWithDetails, ReferralLeadWithDetails>(MemberList.Destination)
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Note, opt => opt.Ignore());

            CreateMap<ReferralHotel, ReferralHotelWithProfile>(MemberList.Source)
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore());
        }
    }
}
