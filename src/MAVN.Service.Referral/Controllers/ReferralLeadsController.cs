//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using AutoMapper;
//using Lykke.Common.Log;
//using Lykke.Service.CustomerProfile.Client;
//using MAVN.Service.Referral.Client;
//using MAVN.Service.Referral.Client.Enums;
//using MAVN.Service.Referral.Client.Models.Requests;
//using MAVN.Service.Referral.Client.Models.Responses;
//using MAVN.Service.Referral.Client.Models.Responses.OfferToPurchase;
//using MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase;
//using MAVN.Service.Referral.Domain.Exceptions;
//using MAVN.Service.Referral.Domain.Models;
//using MAVN.Service.Referral.Domain.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace MAVN.Service.Referral.Controllers
//{
//    [Route("api/referral-leads")]
//    [ApiController]
//    public class ReferralLeadsController : BaseController, IReferralLeadApi
//    {
//        private readonly IPropertyPurchaseService _propertyPurchaseService;
//        private readonly IOfferToPurchaseService _offerToPurchaseService;
//        private readonly IDemoLeadService _demoLeadService;
//        private readonly ICustomerProfileClient _customerProfileClient;
//        //private readonly IReferralLeadService _referralLeadService;
//        private readonly IMapper _mapper;

//        public ReferralLeadsController(
//            //IReferralLeadService referralLeadService,
//            IPropertyPurchaseService propertyPurchaseService,
//            IOfferToPurchaseService offerToPurchaseService,
//            IDemoLeadService demoLeadService,
//            ISettingsService settingsService,
//            ICustomerProfileClient customerProfileClient,
//            IMapper mapper,
//            ILogFactory logFactory) : base(settingsService, logFactory)
//        {
//            _propertyPurchaseService = propertyPurchaseService;
//            _offerToPurchaseService = offerToPurchaseService;
//            //_referralLeadService = referralLeadService;
//            _mapper = mapper;
//            _demoLeadService = demoLeadService;
//            _customerProfileClient = customerProfileClient;
//        }

//        ///// <inheritdoc/>
//        ///// <response code="200">ReferralLeadCreateResponse.</response>
//        //[HttpPost("")]
//        //[ProducesResponseType(typeof(ReferralLeadCreateResponse), (int)HttpStatusCode.OK)]
//        //public async Task<ReferralLeadCreateResponse> PostAsync(ReferralLeadCreateRequest referralLeadCreate)
//        //{
//        //    const string errorMessage = "An error occurred while creating referral lead";

//        //    try
//        //    {
//        //        if (!Guid.TryParse(referralLeadCreate.CustomerId, out _))
//        //        {
//        //            return new ReferralLeadCreateResponse
//        //            {
//        //                ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
//        //                ErrorMessage = "Customer id cannot be parsed."
//        //            };
//        //        }

//        //        var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(referralLeadCreate.CustomerId);
//        //        // Hotel service handles customer == null properly, keep it simple for the demo mode
//        //        if (customer?.Profile != null && IsDemoMode(customer.Profile.Email))
//        //        {
//        //            await _demoLeadService.CreateReferralLeadAsync(_mapper.Map<ReferralLead>(referralLeadCreate));
//        //            return new ReferralLeadCreateResponse();
//        //        }

//        //        await _referralLeadService.CreateReferralLeadAsync(_mapper.Map<ReferralLead>(referralLeadCreate));
//        //    }
//        //    #region ErrorHandling
//        //    catch (CustomerDoesNotExistException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.CustomerDoesNotExist, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (ReferralLeadConfirmationFailedException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.ReferralLeadProcessingFailed, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (ReferralLeadAlreadyConfirmedException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.ReferralLeadAlreadyConfirmed, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (CustomerNotApprovedAgentException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.CustomerNotApprovedAgent, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (CountryCodeDoesNotExistException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.CountryCodeDoesNotExist, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (ReferYourselfException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.ReferYourself, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (ReferralAlreadyExistException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.ReferralLeadAlreadyExist, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (InvalidPhoneNumberException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.InvalidPhoneNumber, ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (CampaignNotFoundException ex)
//        //    {
//        //        Log.Info(errorMessage, context: $"customerId: {referralLeadCreate.CustomerId}; error: {ex.Message}");

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.CampaignNotFound,
//        //            ErrorMessage = ex.Message
//        //        };
//        //    }
//        //    catch (InvalidReferralStakeException e)
//        //    {
//        //        Log.Info("Invalid stake.", e, process: nameof(ConfirmAsync));

//        //        return new ReferralLeadCreateResponse
//        //        {
//        //            ErrorCode = ReferralErrorCodes.InvalidStake
//        //        };
//        //    }
//        //    #endregion

//        //    return new ReferralLeadCreateResponse();
//        //}

//        /// <inheritdoc/>
//        /// <response code="200">ReferralLeadCreateResponse.</response>
//        [HttpPut("confirm")]
//        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
//        public async Task<ReferralLeadConfirmResponse> ConfirmAsync(ReferralLeadConfirmRequest referralLeadConfirmRequest)
//        {
//            try
//            {
//                if (referralLeadConfirmRequest.ConfirmationToken.EndsWith("_demo"))
//                {
//                    await _demoLeadService.ConfirmReferralLeadAsync(referralLeadConfirmRequest.ConfirmationToken);
//                    return new ReferralLeadConfirmResponse();
//                }

//                await _referralLeadService.ConfirmReferralLeadAsync(referralLeadConfirmRequest.ConfirmationToken);
//            }
//            catch (ReferralDoesNotExistException e)
//            {
//                Log.Info(e.Message, process: nameof(PostAsync), context: referralLeadConfirmRequest.ConfirmationToken);

//                return new ReferralLeadConfirmResponse
//                {
//                    ErrorCode = ReferralErrorCodes.ReferralDoesNotExist,
//                    ErrorMessage = "Referral Lead does not exist."
//                };
//            }
//            catch (ReferralAlreadyConfirmedException e)
//            {
//                Log.Info(e.Message, process: nameof(PostAsync), context: referralLeadConfirmRequest.ConfirmationToken);

//                return new ReferralLeadConfirmResponse
//                {
//                    ErrorCode = ReferralErrorCodes.LeadAlreadyConfirmed,
//                    ErrorMessage = "Referral Lead already confirmed."
//                };
//            }

//            return new ReferralLeadConfirmResponse();
//        }

//        [HttpPut("purchase")]
//        public async Task<RealEstatePurchaseResponse> AddRealEstatePurchase(RealEstatePurchaseRequest request)
//        {
//            try
//            {
//                var campaignId = await _propertyPurchaseService.AddRealEstatePurchase(_mapper.Map<PropertyPurchase>(request));

//                return new RealEstatePurchaseResponse
//                {
//                    CampaignId = campaignId,
//                    ErrorCode = RealEstatePurchaseErrorCode.None
//                };
//            }
//            catch (ReferralDoesNotExistException e)
//            {
//                Log.Info(e.Message);

//                return new RealEstatePurchaseResponse
//                {
//                    ErrorCode = RealEstatePurchaseErrorCode.ReferralNotFound
//                };
//            }
//        }

//        /// <inheritdoc/>
//        /// <response code="200">ReferralLeadListResponse.</response>
//        [HttpGet("")]
//        [ProducesResponseType(typeof(ReferralLeadListResponse), (int)HttpStatusCode.OK)]
//        public async Task<ReferralLeadListResponse> GetAsync(string agentId)
//        {
//            if (!TryParseGuid(agentId, nameof(PostAsync), out var agentIdGuid))
//            {
//                return new ReferralLeadListResponse
//                {
//                    ErrorCode = ReferralErrorCodes.GuidCanNotBeParsed,
//                    ErrorMessage = InvalidIdentifierMessage
//                };
//            }

//            var result = await _referralLeadService.GetReferralLeadsForReferrerAsync(agentIdGuid, null, null);

//            return new ReferralLeadListResponse
//            {
//                ReferralLeads = _mapper.Map<IReadOnlyList<ReferralLeadModel>>(result)
//            };
//        }

//        /// <inheritdoc/>
//        /// <response code="200">ApprovedReferralLeadListResponse.</response>
//        [HttpGet("approved")]
//        [ProducesResponseType(typeof(ApprovedReferralLeadListResponse), (int)HttpStatusCode.OK)]
//        public async Task<ApprovedReferralLeadListResponse> GetApprovedAsync()
//        {
//            var result = await _referralLeadService.GetApprovedLeadsAsync();

//            return new ApprovedReferralLeadListResponse
//            {
//                ReferralLeads = _mapper.Map<IReadOnlyList<ApprovedReferralLeadModel>>(result)
//            };
//        }

//        /// <inheritdoc/>
//        /// <response code="200">PropertyPurchaseListResponse.</response>
//        [HttpGet("property-purchases")]
//        [ProducesResponseType(typeof(PropertyPurchaseListResponse), (int)HttpStatusCode.OK)]
//        public async Task<PropertyPurchaseListResponse> GetPropertyPurchasesAsync()
//        {
//            var result = await _propertyPurchaseService.GetPropertyPurchasesAsync();

//            return new PropertyPurchaseListResponse
//            {
//                PropertyPurchases = _mapper.Map<IReadOnlyList<PropertyPurchaseModel>>(result)
//            };
//        }

//        /// <inheritdoc/>
//        /// <response code="200">LeadStatisticsResponse.</response>
//        [HttpGet("statistic")]
//        [ProducesResponseType(typeof(LeadStatisticsResponse), (int)HttpStatusCode.OK)]
//        public async Task<LeadStatisticsResponse> GetLeadStatisticAsync()
//        {
//            var result = await _referralLeadService.GetStatistic();

//            return _mapper.Map<LeadStatisticsResponse>(result);
//        }

//        /// <inheritdoc/>
//        /// <response code="200">OfferToPurchaseListResponse.</response>
//        [HttpGet("offer-to-purchases")]
//        [ProducesResponseType(typeof(OfferToPurchaseListResponse), (int)HttpStatusCode.OK)]
//        public async Task<OfferToPurchaseListResponse> GetOfferToPurchasesAsync()
//        {
//            var result = await _offerToPurchaseService.GetOffersToPurchasesAsync();

//            return new OfferToPurchaseListResponse
//            {
//                OfferToPurchases = _mapper.Map<IReadOnlyList<OfferToPurchaseModel>>(result)
//            };
//        }
        
//        // <inheritdoc/>
//        [HttpGet("approved-referrals-count-by-agents")]
//        [ProducesResponseType(typeof(List<ReferralLeadApprovedByAgentModel>), (int) HttpStatusCode.OK)]
//        public async Task<List<ReferralLeadApprovedByAgentModel>> GetApprovedReferralsCountByAgentsAsync(List<Guid> agentIds)
//        {
//            return (await _referralLeadService.GetApprovedReferralsCountByAgentsAsync(agentIds))
//                .Select(x => new ReferralLeadApprovedByAgentModel
//                {
//                    AgentId = x.Key,
//                    Count = x.Value
//                })
//                .ToList();
//        }
//    }
//}
