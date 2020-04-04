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

            CreateMap<PropertyPurchase, PropertyPurchaseEntity>();

            CreateMap<PropertyPurchaseEntity, PropertyPurchase>()
                .ForMember(src => src.VatAmount, opt => opt.Ignore())
                .ForMember(src => src.SellingPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.NetPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.DiscountAmount, opt => opt.Ignore())
                .ForMember(src => src.CalculatedCommissionAmount, opt => opt.Ignore())
                .ForMember(src => src.CurrencyCode, opt => opt.Ignore());

            CreateMap<Domain.Entities.ReferralLeadState, ReferralLeadState>();

            CreateMap<OfferToPurchase, OfferToPurchaseEntity>();

            CreateMap<OfferToPurchaseEntity, OfferToPurchase>()
                .ForMember(src => src.VatAmount, opt => opt.Ignore())
                .ForMember(src => src.SellingPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.NetPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.DiscountAmount, opt => opt.Ignore())
                .ForMember(src => src.CurrencyCode, opt => opt.Ignore())
                .ForMember(src => src.UnitLocationCode, opt => opt.Ignore());

            CreateMap<ReferralHotelEncrypted, ReferralHotelEntity>(MemberList.Source);
            CreateMap<ReferralHotelEntity, ReferralHotelEncrypted>(MemberList.Destination);

            CreateMap<ReferralLeadEncrypted, ReferralLeadEntity>(MemberList.Source)
                .ForMember(src => src.OffersToPurchase, opt => opt.Ignore())
                .ForMember(src => src.PropertyPurchases, opt => opt.Ignore());
            CreateMap<ReferralLeadEntity, ReferralLeadEncrypted>(MemberList.Destination)
                .ForSourceMember(src => src.OffersToPurchase, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.PropertyPurchases, opt => opt.DoNotValidate());

            CreateMap<ReferralLeadEntity, ReferralLeadEncryptedWithDetails>(MemberList.Destination)
                .ForMember(dest => dest.OffersCount, opt => opt.MapFrom(src => src.OffersToPurchase.Count))
                .ForMember(dest => dest.PurchaseCount, opt => opt.MapFrom(src => src.PropertyPurchases.Count));

            // Friend Referral
            CreateMap<ReferralFriend, FriendReferralEntity>(MemberList.Destination);


            CreateMap<FriendReferralEntity, ReferralFriend>(MemberList.Destination)
                .ForMember(src => src.Email, opt => opt.Ignore())
                .ForMember(src => src.FullName, opt => opt.Ignore());
        }
    }
}
