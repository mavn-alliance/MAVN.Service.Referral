using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Log;
using MAVN.Service.Referral.Client;
using MAVN.Service.Referral.Client.Enums;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.Referral.Controllers
{
    [Route("api/referrals")]
    [ApiController]
    public class ReferralsController : BaseController, IReferralApi
    {
        private readonly IReferralService _referralService;
        private readonly IFriendReferralService _friendReferralService;
        private readonly IMapper _mapper;

        public ReferralsController(
            IReferralService referralService,
            IFriendReferralService friendReferralService,
            ISettingsService settingsService,
            IMapper mapper,
            ILogFactory logFactory) : base(settingsService, logFactory)
        {
            _referralService = referralService;
            _friendReferralService = friendReferralService;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralResultResponse.</response>
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(ReferralResultResponse), (int)HttpStatusCode.OK)]
        public async Task<ReferralResultResponse> GetAsync(string customerId)
        {
            try
            {
                if (!TryParseGuid(customerId, nameof(GetAsync), out var customerIdGuid))
                {
                    return new ReferralResultResponse
                    {
                        ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
                        ErrorMessage = InvalidIdentifierMessage
                    };
                }

                var referral = await _referralService.GetReferralByCustomerIdAsync(customerIdGuid);
                return _mapper.Map<ReferralResultResponse>(referral);
            }
            catch (CustomerNotFoundException e)
            {
                Log.Info(e.Message, customerId);
                return new ReferralResultResponse()
                {
                    ErrorCode = ReferralErrorCodes.ReferralNotFound,
                    ErrorMessage = e.Message
                };
            }
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralCreateResponse.</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(ReferralCreateResponse), (int)HttpStatusCode.OK)]
        public async Task<ReferralCreateResponse> PostAsync([FromBody] ReferralCreateRequest referralCreate)
        {
            if (!TryParseGuid(referralCreate.CustomerId, nameof(PostAsync), out var customerIdGuid))
            {
                return new ReferralCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
                    ErrorMessage = InvalidIdentifierMessage
                };
            }

            var result = await _referralService.GetOrCreateReferralForCustomerIdAsync(customerIdGuid);

            return new ReferralCreateResponse() { ReferralCode = result };
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralCustomerHistoryResponse.</response>
        [HttpGet("friends/{customerId}")]
        [ProducesResponseType(typeof(ReferralCustomerHistoryResponse), (int)HttpStatusCode.OK)]
        public async Task<ReferralCustomerHistoryResponse> GetCustomerFriendsReferralHistory(string customerId)
        {
            if (!TryParseGuid(customerId, nameof(GetCustomerFriendsReferralHistory), out var customerIdGuid))
            {
                return new ReferralCustomerHistoryResponse
                {
                    ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
                    ErrorMessage = InvalidIdentifierMessage
                };
            }

            var referral = await _friendReferralService.GetAllReferralsForCustomerAsync(customerIdGuid);

            return new ReferralCustomerHistoryResponse
            {
                ReferredCustomers = referral?.ReferredIds ?? new List<string>()
            };
        }
    }
}
