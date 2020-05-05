using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Lykke.Common.Log;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Referral.Client.Enums;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.Referral.Controllers
{
    [Route("api/referral-hotels")]
    [ApiController]
    public class ReferralHotelsController : BaseController, IReferralHotelsApi
    {
        private readonly IReferralHotelsService _referralHotelsService;
        private readonly IMapper _mapper;
        private readonly IDemoHotelService _demoHotelService;
        private readonly ICustomerProfileClient _customerProfileClient;
        public ReferralHotelsController(
            IReferralHotelsService referralHotelsService,
            IDemoHotelService demoHotelService,
            ICustomerProfileClient customerProfileClient,
            ISettingsService settingsService,
            IMapper mapper,
            ILogFactory logFactory)
            : base(settingsService, logFactory)
        {
            _referralHotelsService = referralHotelsService;
            _mapper = mapper;
            _demoHotelService = demoHotelService;
            _customerProfileClient = customerProfileClient;
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralHotelCreateResponse.</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(ReferralHotelCreateResponse), (int) HttpStatusCode.OK)]
        public async Task<ReferralHotelCreateResponse> CreateAsync(ReferralHotelCreateRequest request)
        {
            try
            {
                var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(request.ReferrerId);
                ReferralHotel referral;
                // Hotel service handles customer == null properly, keep it simple for the demo mode
                if (customer?.Profile == null || !IsDemoMode(customer.Profile.Email))
                {
                    referral = await _referralHotelsService.CreateAsync(
                        request.Email, request.ReferrerId, request.CampaignId, 
                        request.PhoneCountryCodeId, request.PhoneNumber, request.FullName);
                }
                else
                {
                    referral = await _demoHotelService.CreateHotelReferralAsync(
                        request.CampaignId,
                        request.Email,
                        request.ReferrerId,
                        request.FullName,
                        request.PhoneCountryCodeId,
                        request.PhoneNumber);
                }

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.None,
                    HotelReferral = _mapper.Map<ReferralHotelModel>(referral)
                };
            }
            #region ErrorHandling
            catch (CustomerDoesNotExistException)
            {
                Log.Info("Customer does not exist.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.CustomerDoesNotExist
                };
            }
            catch (ReferralAlreadyConfirmedException)
            {
                Log.Info("Referral with given email already confirmed.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.ReferralAlreadyConfirmed
                };
            }
            catch (ReferralAlreadyExistException)
            {
                Log.Info("Referral with given email already exists for a referer.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.ReferralAlreadyExist
                };
            }
            catch (ReferYourselfException)
            {
                Log.Info("Agent can't refer himself.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.AgentCantReferHimself
                };
            }
            catch (ReferralHotelLimitExceededException)
            {
                Log.Info("Limit exceeded.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.ReferralsLimitExceeded
                };
            }
            catch (CampaignNotFoundException)
            {
                Log.Info("Limit exceeded.", request.ReferrerId, process: nameof(CreateAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.CampaignNotFound
                };
            }
            catch (ReferralExpiredException e)
            {
                Log.Info("Referral expired.", e.ReferralId, process: nameof(ConfirmAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.ReferralExpired
                };
            }
            catch (InvalidReferralStakeException e)
            {
                Log.Info("Invalid stake.", e, process: nameof(ConfirmAsync));

                return new ReferralHotelCreateResponse
                {
                    ErrorCode = ReferralHotelCreateErrorCode.InvalidStake
                };
            }
            #endregion
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralHotelConfirmResponse.</response>
        [HttpPut("confirm")]
        [ProducesResponseType(typeof(ReferralHotelConfirmResponse), (int) HttpStatusCode.OK)]
        public async Task<ReferralHotelConfirmResponse> ConfirmAsync(ReferralHotelConfirmRequest referralHotelConfirm)
        {
            Log.Info($"ConfirmAsync {referralHotelConfirm.ConfirmationToken}");
            try
            {
                ReferralHotel referral;
                if (!referralHotelConfirm.ConfirmationToken.EndsWith("_demo"))
                {
                    referral = await _referralHotelsService.ConfirmAsync(referralHotelConfirm.ConfirmationToken);
                }
                else
                {
                    referral = await _demoHotelService.ConfirmReferralHotelAsync(referralHotelConfirm.ConfirmationToken);
                }

                return new ReferralHotelConfirmResponse
                {
                    HotelReferral = _mapper.Map<ReferralHotelModel>(referral),
                    ErrorCode = ReferralHotelConfirmErrorCode.None
                };
            }
            catch (ReferralDoesNotExistException)
            {
                Log.Info("Referral does not exist.", referralHotelConfirm, process: nameof(ConfirmAsync));

                return new ReferralHotelConfirmResponse
                {
                    ErrorCode = ReferralHotelConfirmErrorCode.ReferralDoesNotExist
                };
            }
            catch (ReferralAlreadyConfirmedException)
            {
                Log.Info("Referral with given email already confirmed.", referralHotelConfirm, process: nameof(ConfirmAsync));

                return new ReferralHotelConfirmResponse
                {
                    ErrorCode = ReferralHotelConfirmErrorCode.ReferralAlreadyConfirmed
                };
            }
            catch (ReferralExpiredException e)
            {
                Log.Info("Referral expired.", e.ReferralId, process: nameof(ConfirmAsync));

                return new ReferralHotelConfirmResponse
                {
                    ErrorCode = ReferralHotelConfirmErrorCode.ReferralExpired
                };
            }
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralHotelUseResponse.</response>
        [HttpPut("use")]
        [ProducesResponseType(typeof(ReferralHotelUseResponse), (int) HttpStatusCode.OK)]
        public async Task<ReferralHotelUseResponse> UseAsync(ReferralHotelUseRequest referralHotelUse)
        {
            try
            {
                var referral =
                    await _referralHotelsService.UseAsync(_mapper.Map<ReferralHotelUseModel>(referralHotelUse));

                return new ReferralHotelUseResponse
                {
                    HotelReferral = _mapper.Map<ReferralHotelModel>(referral),
                    ErrorCode = ReferralHotelUseErrorCode.None,
                };
            }
            catch (ReferralDoesNotExistException)
            {
                Log.Info("Referral does not exist.", 
                    new
                    {
                        hashedEmail = referralHotelUse.BuyerEmail.CalculateHexHash64()
                    },
                    process: nameof(UseAsync));

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.ReferralDoesNotExist
                };
            }
            catch (ReferralNotConfirmedException e)
            {
                Log.Info("Referral with given email hasn't been confirmed.", e.ReferralId, process: nameof(UseAsync));

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.ReferralNotConfirmed
                };
            }
            catch (ReferralExpiredException e)
            {
                Log.Info("Referral expired.", e.ReferralId, process: nameof(UseAsync));

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.ReferralExpired
                };
            }
            catch (ReferralAlreadyUsedException e)
            {
                Log.Info("Referral already used.", e.ReferralId, process: nameof(UseAsync));

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.ReferralAlreadyUsed
                };
            }
            catch (PartnerNotFoundException e)
            {
                Log.Info(e.Message, exception: e);

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.PartnerNotFound
                };
            }
            catch (InvalidCurrencyException)
            {
                Log.Info("Currency does not exist", 
                    new
                    {
                        hashedEmail = referralHotelUse.BuyerEmail.CalculateHexHash64(),
                        referralHotelUse.CurrencyCode
                    },
                    process: nameof(UseAsync));

                return new ReferralHotelUseResponse
                {
                    ErrorCode = ReferralHotelUseErrorCode.CurrencyDoesNotExist
                };
            }
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralHotelsListByReferrerIdResponse.</response>
        [HttpGet("byReferrerId")]
        [ProducesResponseType(typeof(ReferralHotelsListByReferrerIdResponse), (int) HttpStatusCode.OK)]
        public async Task<ReferralHotelsListByReferrerIdResponse> GetByReferrerIdAsync(string referrerId)
        {
            try
            {
                var referrals = await _referralHotelsService.GetByReferrerIdAsync(referrerId, null, null);

                return new ReferralHotelsListByReferrerIdResponse
                {
                    ErrorCode = ReferralHotelsGetErrorCode.None,
                    HotelReferrals = _mapper.Map<IReadOnlyList<ReferralHotelModel>>(referrals)
                };
            }
            catch (CustomerDoesNotExistException)
            {
                Log.Info("Customer does not exist.", referrerId, process: nameof(GetByReferrerIdAsync));

                return new ReferralHotelsListByReferrerIdResponse
                {
                    ErrorCode = ReferralHotelsGetErrorCode.CustomerDoesNotExist
                };
            }
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralHotelsListByEmailResponse.</response>
        [HttpPost("getbyemail")]
        [ProducesResponseType(typeof(ReferralHotelsListByEmailResponse), (int)HttpStatusCode.OK)]
        public async Task<ReferralHotelsListByEmailResponse> GetByEmailAsync(GetHotelReferralsByEmailRequestModel request)
        {
            var referrals = await _referralHotelsService.GetByEmailAsync(
                request.Email,
                request.PartnerId,
                request.Location);

            return new ReferralHotelsListByEmailResponse
            {
                HotelReferrals = _mapper.Map<IReadOnlyList<ReferralHotelModel>>(referrals)
            };
        }
    }
}
