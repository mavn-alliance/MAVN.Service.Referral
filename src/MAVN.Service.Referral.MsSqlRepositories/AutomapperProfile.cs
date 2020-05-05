using AutoMapper;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.MsSqlRepositories.Entities;

namespace MAVN.Service.Referral.MsSqlRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.Models.Referral, ReferralEntity>()
                .ForMember(r => r.ReferralCode, opt => opt.MapFrom(x => x.ReferralCode))
                .ForMember(r => r.PurchasesReferred, opt => opt.Ignore())
                .ForMember(r => r.PurchaseReferrers, opt => opt.Ignore());

            CreateMap<ReferralEntity, Domain.Models.Referral>()
                .ForMember(r => r.ReferralCode, opt => opt.MapFrom(x => x.ReferralCode))
                .ForMember(r => r.Id, opt => opt.MapFrom(r => r.CustomerId));

            CreateMap<ReferralHotelEncrypted, ReferralHotelEntity>(MemberList.Source);
            CreateMap<ReferralHotelEntity, ReferralHotelEncrypted>(MemberList.Destination);

            // Friend Referral
            CreateMap<ReferralFriend, FriendReferralEntity>(MemberList.Destination);

            CreateMap<FriendReferralEntity, ReferralFriend>(MemberList.Destination)
                .ForMember(src => src.Email, opt => opt.Ignore())
                .ForMember(src => src.FullName, opt => opt.Ignore());
        }
    }
}
