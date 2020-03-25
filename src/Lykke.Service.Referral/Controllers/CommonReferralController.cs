using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Log;
using Lykke.Service.Referral.Client;
using Lykke.Service.Referral.Client.Enums;
using Lykke.Service.Referral.Client.Models.Requests;
using Lykke.Service.Referral.Client.Models.Responses.CommonReferral;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.Referral.Controllers
{
    [Route("api/common-referral")]
    [ApiController]
    public class CommonReferralController: BaseController, ICommonReferralApi
    {
        private readonly ICommonReferralService _commonReferralService;
        private readonly IMapper _mapper;

        public CommonReferralController(
            ICommonReferralService commonReferralService,
            ISettingsService settingsService, 
            IMapper mapper,
            ILogFactory logFactory) : base(settingsService, logFactory)
        {
            _commonReferralService = commonReferralService;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralResultResponse.</response>
        [HttpPost("byCustomer")]
        [ProducesResponseType(typeof(CommonReferralByCustomerIdResponse), (int)HttpStatusCode.OK)]
        public async Task<CommonReferralByCustomerIdResponse> GetReferralsByCustomerIdAsync(CommonReferralByCustomerIdRequest request)
        {
            try
            {
                var referrals = await _commonReferralService
                    .GetForCustomerAsync(
                        request.CustomerId,
                        request.CampaignId,
                        _mapper.Map<List<Domain.Models.CommonReferralStatus>>(request.Statuses));
                return new CommonReferralByCustomerIdResponse
                {
                    Referrals = _mapper.Map<List<CommonReferralModel>>(referrals)
                };
            }
            catch (CustomerDoesNotExistException e)
            {
                Log.Info(e.Message, request.CustomerId);
                return new CommonReferralByCustomerIdResponse()
                {
                    ErrorCode = ReferralErrorCodes.ReferralNotFound,
                    ErrorMessage = e.Message
                };
            }
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralResultResponse.</response>
        [HttpPost("list")]
        [ProducesResponseType(typeof(CommonReferralByReferralIdsResponse), (int)HttpStatusCode.OK)]
        public async Task<CommonReferralByReferralIdsResponse> GetReferralsByReferralIdsAsync(CommonReferralByReferralIdsRequest request)
        {
            var referrals = await _commonReferralService.GetByReferralIdsAsync(request.ReferralIds);

            return new CommonReferralByReferralIdsResponse
            {
                CommonReferrals = _mapper.Map<Dictionary<string, CommonReferralModel>>(referrals)
            };
        }
    }
}
