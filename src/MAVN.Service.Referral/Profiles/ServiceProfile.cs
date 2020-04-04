using System;
using System.Collections.Generic;
using AutoMapper;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Referral.Client.Models.Responses.OfferToPurchase;
using MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase;
using MAVN.Service.Referral.Domain.Models;
using CommonReferralModel = Lykke.Service.Referral.Domain.Models.CommonReferralModel;

namespace MAVN.Service.Referral.Profiles
{
    public class ServiceProfile: Profile
    {
        public ServiceProfile()
        {
            CreateMap<ReferralResultResponse, Domain.Models.Referral>(MemberList.Source)
                .ForMember(c => c.CustomerId, opt => opt.Ignore())
                .ForMember(c => c.Id, opt => opt.Ignore())
                .ForSourceMember(c => c.ErrorCode, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.ErrorMessage, opt => opt.DoNotValidate());

            CreateMap<Domain.Models.Referral, ReferralResultResponse>(MemberList.Source)
                .ForMember(c => c.ErrorCode, opt => opt.Ignore())
                .ForMember(c => c.ErrorMessage, opt => opt.Ignore())
                .ForSourceMember(c => c.CustomerId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.Id, opt => opt.DoNotValidate());

            // Referral lead
            CreateMap<ReferralLeadCreateRequest, ReferralLead>(MemberList.Source)
                .ForMember(c => c.AgentId, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(c => c.Id, opt => opt.Ignore())
                .ForMember(c => c.SalesforceId, opt => opt.Ignore())
                .ForMember(c => c.CreationDateTime, opt => opt.Ignore())
                .ForMember(c => c.ResponseStatus, opt => opt.Ignore())
                .ForMember(c => c.AgentSalesforceId, opt => opt.Ignore())
                .ForMember(c => c.ConfirmationToken, opt => opt.Ignore())
                .ForMember(c => c.State, opt => opt.Ignore());

            CreateMap<ReferralLead, ReferralLeadCreateRequest>(MemberList.Source)
                .ForMember(c => c.CustomerId, opt => opt.MapFrom(c => c.AgentId))
                .ForSourceMember(c => c.Id, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.SalesforceId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.CreationDateTime, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.ResponseStatus, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.AgentSalesforceId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.ConfirmationToken, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.State, opt => opt.DoNotValidate());
            
            CreateMap<ReferralLeadWithDetails, ReferralLeadModel>(MemberList.Source)
                .ForSourceMember(c => c.ResponseStatus, opt => opt.DoNotValidate());

            CreateMap<ReferralLead, ApprovedReferralLeadModel>(MemberList.Destination)
                .ForMember(c => c.ReferralLeadId, opt => opt.MapFrom(dest => dest.Id))
                .ForMember(c => c.Timestamp, opt => opt.MapFrom(dest => dest.CreationDateTime))
                .ForSourceMember(c => c.FirstName, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.LastName, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.PhoneCountryCodeId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.PhoneNumber, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.Email, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.Note, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.AgentId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.AgentSalesforceId, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.ResponseStatus, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.ConfirmationToken, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.State, opt => opt.DoNotValidate())
                .ForSourceMember(c => c.CreationDateTime, opt => opt.DoNotValidate());

            // Property purchase
            CreateMap<PropertyPurchase, PropertyPurchaseModel>(MemberList.Destination);

            CreateMap<PropertyPurchaseModel, PropertyPurchase>(MemberList.Source)
                .ForMember(src => src.VatAmount, opt => opt.Ignore())
                .ForMember(src => src.SellingPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.NetPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.DiscountAmount, opt => opt.Ignore())
                .ForMember(src => src.CurrencyCode, opt => opt.Ignore());

            CreateMap<RealEstatePurchaseRequest, PropertyPurchase>(MemberList.Source)
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ForMember(src => src.ReferralLeadId, opt => opt.MapFrom(dest => dest.ReferralId))
                .ForMember(src => src.VatAmount, opt => opt.Ignore())
                .ForMember(src => src.SellingPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.NetPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.DiscountAmount, opt => opt.Ignore())
                .ForMember(src => src.CurrencyCode, opt => opt.Ignore());

            // Offer to purchase
            CreateMap<OfferToPurchase, OfferToPurchaseModel>(MemberList.Destination);

            CreateMap<OfferToPurchaseModel, OfferToPurchase>(MemberList.Source)
                .ForMember(src => src.VatAmount, opt => opt.Ignore())
                .ForMember(src => src.SellingPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.NetPropertyPrice, opt => opt.Ignore())
                .ForMember(src => src.DiscountAmount, opt => opt.Ignore())
                .ForMember(src => src.CurrencyCode, opt => opt.Ignore());

            // Hotel Referral
            CreateMap<ReferralHotel, ReferralHotelModel>(MemberList.Source);
            CreateMap<ReferralHotelCreateRequest, ReferralHotel>(MemberList.Destination)
                .ForMember(c => c.Id, opt => opt.Ignore())
                .ForMember(c => c.ConfirmationToken, opt => opt.Ignore())
                .ForMember(c => c.State, opt => opt.Ignore())
                .ForMember(c => c.Location, opt => opt.Ignore())
                .ForMember(c => c.PartnerId, opt => opt.Ignore())
                .ForMember(c => c.ExpirationDateTime, opt => opt.Ignore())
                .ForMember(c => c.CreationDateTime, opt => opt.Ignore());

            CreateMap<ReferralHotelWithProfile, ReferralHotelModel>(MemberList.Source);
            CreateMap<ReferralHotelUseRequest, ReferralHotelUseModel>(MemberList.Destination);

            // Statistics
            CreateMap<LeadStatisticModel, LeadStatisticsResponse>();

            // Common Referrals
            CreateMap<Client.Enums.ReferralType, Domain.Models.ReferralType>(MemberList.Destination);
            CreateMap<Client.Enums.CommonReferralStatus, Domain.Models.CommonReferralStatus>(MemberList.Destination);
            CreateMap<Domain.Models.CommonReferralStatus, Domain.Entities.ReferralLeadState>(MemberList.Source)
                .ConvertUsing(value => ConvertCommonReferralStatusToLeadState(value));
            CreateMap<Domain.Models.CommonReferralStatus, Domain.Models.ReferralHotelState>(MemberList.Source)
                .ConvertUsing(value => ConvertCommonReferralStatusToHotelState(value));
            CreateMap<Domain.Models.CommonReferralStatus, Domain.Models.ReferralFriendState>(MemberList.Source)
                .ConvertUsing(value => ConvertCommonReferralStatusToFriendState(value));

            CreateMap<Domain.Models.ReferralHotelState, Domain.Models.CommonReferralStatus>(MemberList.Source)
                .ConvertUsing(value => ConvertHotelStateToCommonReferralStatus(value));
            CreateMap<Domain.Entities.ReferralLeadState, Domain.Models.CommonReferralStatus>(MemberList.Source)
                .ConvertUsing(value => ConvertLeadStateToCommonReferralStatus(value));
            CreateMap<Domain.Models.ReferralFriendState, Domain.Models.CommonReferralStatus>(MemberList.Source)
                .ConvertUsing(value => ConvertFriendStateToCommonReferralStatus(value));

            CreateMap<ReferralLeadWithDetails, CommonReferralModel>(MemberList.Destination)
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.CreationDateTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.ReferralType, opt => opt.MapFrom(src => ReferralType.RealEstate))
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore());

            CreateMap<ReferralHotelWithProfile, CommonReferralModel>(MemberList.Destination)
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.CreationDateTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.ReferralType, opt => opt.MapFrom(src => ReferralType.Hospitality));

            CreateMap<ReferralFriend, CommonReferralModel>(MemberList.Destination)
                .ConvertUsing(value => CovertReferralFriendToCommonReferral(value));

            CreateMap<Domain.Models.CommonReferralModel, Client.Models.Responses.CommonReferral.CommonReferralModel>();
        }

        private CommonReferralModel CovertReferralFriendToCommonReferral(ReferralFriend value)
        {
            string[] nameSurname = new string[2];
            string[] nameSurnameTemp = value.FullName.Trim(' ').Split(' ');
            if (nameSurnameTemp.Length == 1)
            {
                nameSurname[0] = value.FullName;
                nameSurname[1] = string.Empty;
            }
            else
            {
                for (int i = 0; i < nameSurnameTemp.Length; i++)
                {
                    if (i < nameSurnameTemp.Length - 1)
                    {
                        if (!string.IsNullOrEmpty(nameSurname[0]))
                            nameSurname[0] += " " + nameSurnameTemp[i];
                        else
                            nameSurname[0] += nameSurnameTemp[i];
                    }
                    else
                        nameSurname[1] = nameSurnameTemp[i];
                }
            }

            return new CommonReferralModel
            {
                CampaignId = value.CampaignId,
                Email = value.Email,
                FirstName = nameSurname[0],
                LastName = nameSurname[1],
                Id = value.Id.ToString("D"),
                ReferralType = ReferralType.Friend,
                Status = ConvertFriendStateToCommonReferralStatus(value.State),
                TimeStamp = value.CreationDateTime
            };
        }

        private Domain.Models.CommonReferralStatus ConvertFriendStateToCommonReferralStatus(ReferralFriendState value)
        {
            switch (value)
            {
                case ReferralFriendState.Pending:
                    return CommonReferralStatus.Pending;
                // Confirmed and accepted swapped for a reason
                case ReferralFriendState.Confirmed:
                    return CommonReferralStatus.Accepted;
                case ReferralFriendState.Accepted:
                    return CommonReferralStatus.Confirmed;
                default:
                    return CommonReferralStatus.Expired;
            }
        }

        private ReferralFriendState ConvertCommonReferralStatusToFriendState(CommonReferralStatus value)
        {
            switch (value)
            {
                case CommonReferralStatus.Pending:
                    return Domain.Models.ReferralFriendState.Pending;
                // Confirmed and accepted swapped for a reason
                case CommonReferralStatus.Confirmed:
                    return Domain.Models.ReferralFriendState.Accepted;
                case CommonReferralStatus.Accepted:
                    return Domain.Models.ReferralFriendState.Confirmed;
                default:
                    return default(ReferralFriendState);
            }
        }

        private Domain.Entities.ReferralLeadState ConvertCommonReferralStatusToLeadState(CommonReferralStatus value)
        {
            switch (value)
            {
                case CommonReferralStatus.Pending:
                    return Domain.Entities.ReferralLeadState.Pending;
                case CommonReferralStatus.Confirmed:
                    return Domain.Entities.ReferralLeadState.Confirmed;
                case CommonReferralStatus.Accepted:
                    return Domain.Entities.ReferralLeadState.Approved;
                default:
                    return Domain.Entities.ReferralLeadState.Rejected;
            }
        }

        private CommonReferralStatus ConvertHotelStateToCommonReferralStatus(ReferralHotelState value)
        {
            switch (value)
            {
                case ReferralHotelState.Pending:
                    return CommonReferralStatus.Pending;
                case ReferralHotelState.Confirmed:
                    return CommonReferralStatus.Confirmed;
                case ReferralHotelState.Used:
                    return CommonReferralStatus.Accepted;
                default:
                    return CommonReferralStatus.Expired;
            }
        }

        private ReferralHotelState ConvertCommonReferralStatusToHotelState(CommonReferralStatus value)
        {
            switch (value)
            {
                case CommonReferralStatus.Pending:
                    return ReferralHotelState.Pending;
                case CommonReferralStatus.Confirmed:
                    return ReferralHotelState.Confirmed;
                case CommonReferralStatus.Accepted:
                    return ReferralHotelState.Used;
                default:
                    return ReferralHotelState.Expired;
            }
        }

        private CommonReferralStatus ConvertLeadStateToCommonReferralStatus(Domain.Entities.ReferralLeadState value)
        {
            switch (value)
            {
                case Domain.Entities.ReferralLeadState.Pending:
                    return CommonReferralStatus.Pending;
                case Domain.Entities.ReferralLeadState.Confirmed:
                    return CommonReferralStatus.Confirmed;
                case Domain.Entities.ReferralLeadState.Approved:
                    return CommonReferralStatus.Accepted;
                default:
                    return CommonReferralStatus.Expired;
            }
        }
    }
}
