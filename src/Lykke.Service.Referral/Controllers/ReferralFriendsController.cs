using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.Referral.Client;
using Lykke.Service.Referral.Client.Enums;
using Lykke.Service.Referral.Client.Models.Requests;
using Lykke.Service.Referral.Client.Models.Responses;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.Referral.Controllers
{
    [Route("api/referral-friends")]
    public class ReferralFriendsController : BaseController, IReferralFriendsApi
    {
        private readonly IFriendReferralService _friendReferralService;

        public ReferralFriendsController(
            IFriendReferralService friendReferralService,
            ISettingsService settingsService, 
            ILogFactory logFactory) : base(settingsService, logFactory)
        {
            _friendReferralService = friendReferralService;
        }

        /// <inheritdoc/>
        /// <response code="200">ReferralLeadCreateResponse.</response>
        [HttpPost]
        public async Task<ReferralFriendCreateResponse> CreateAsync([FromBody] ReferralFriendCreateRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerIdGuid))
                {
                    return new ReferralFriendCreateResponse
                    {
                        ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
                        ErrorMessage = "Customer id cannot be parsed."
                    };
                }

                await _friendReferralService.CreateAsync(
                        customerIdGuid,
                        request.CampaignId,
                        request.Email,
                        request.FullName);

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.None
                };
            }
            #region ErrorHandling
            catch (CustomerDoesNotExistException)
            {
                Log.Info("Customer does not exist.", request.CustomerId, process: nameof(CreateAsync));

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.CustomerDoesNotExist
                };
            }
            catch (ReferralAlreadyConfirmedException)
            {
                Log.Info("Referral with given email already confirmed.", request.CustomerId, process: nameof(CreateAsync));

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.LeadAlreadyConfirmed
                };
            }
            catch (ReferralAlreadyExistException)
            {
                Log.Info("Referral with given email already exists for a referer.", request.CustomerId, process: nameof(CreateAsync));

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.ReferralFriendAlreadyExist
                };
            }
            catch (ReferYourselfException)
            {
                Log.Info("Agent can't refer himself.", request.CustomerId, process: nameof(CreateAsync));

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.ReferYourself
                };
            }
            catch (CampaignNotFoundException e)
            {
                Log.Info(e.Message, request.CampaignId, process: nameof(CreateAsync));

                return new ReferralFriendCreateResponse
                {
                    ErrorCode = ReferralErrorCodes.CampaignNotFound
                };
            }
            #endregion
        }
    }
}
