using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Referral.Client.Models.Responses.OfferToPurchase;
using MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase;
using Refit;

namespace MAVN.Service.Referral.Client
{
    /// <summary>
    /// Referral Leads client API interface.
    /// </summary>
    [PublicAPI]
    public interface IReferralLeadApi
    {
        /// <summary>
        /// Create referral lead.
        /// </summary>
        /// <param name="referralLeadCreate">The model containing the data for which referral lead would be created</param>
        /// <returns>A referral model containing the result of the generation.</returns>
        [Post("/api/referral-leads")]
        Task<ReferralLeadCreateResponse> PostAsync(ReferralLeadCreateRequest referralLeadCreate);

        /// <summary>
        /// Confirm the lead.
        /// </summary>
        /// <param name="referralLeadConfirmRequest">The model containing the data for which referral lead would be confirmed</param>
        /// <returns>Whenever the response is successful or not.</returns>
        [Put("/api/referral-leads/confirm")]
        Task<ReferralLeadConfirmResponse> ConfirmAsync(ReferralLeadConfirmRequest referralLeadConfirmRequest);

        /// <summary>
        /// Created real estate purchase.
        /// </summary>
        /// <param name="request">The model creating the purchase info.</param>
        /// <returns>A model containing the result form the operation.</returns>
        [Put("/api/referral-leads/purchase")]
        Task<RealEstatePurchaseResponse> AddRealEstatePurchase(RealEstatePurchaseRequest request);

        /// <summary>
        /// Get referral leads.
        /// </summary>
        /// <returns>A referral lead list model containing all referral leads.</returns>
        [Get("/api/referral-leads")]
        Task<ReferralLeadListResponse> GetAsync(string agentId);

        /// <summary>
        /// Get approved referral leads.
        /// </summary>
        /// <returns>A referral lead list model containing all approved referral leads.</returns>
        [Get("/api/referral-leads/approved")]
        Task<ApprovedReferralLeadListResponse> GetApprovedAsync();

        /// <summary>
        /// Get property purchases.
        /// </summary>
        /// <returns>A property purchase list model containing all purchased properties.</returns>
        [Get("/api/referral-leads/property-purchases")]
        Task<PropertyPurchaseListResponse> GetPropertyPurchasesAsync();

        /// <summary>
        /// Get leads' statistic
        /// </summary>
        /// <returns>Statistic Lead Response Model</returns>
        [Get("/api/referral-leads/statistic")]
        Task<LeadStatisticsResponse> GetLeadStatisticAsync();

        /// <summary>
        /// Get offers to purchase.
        /// </summary>
        /// <returns>A offer to purchase list model containing all offers to purchases.</returns>
        [Get("/api/referral-leads/offer-to-purchases")]
        Task<OfferToPurchaseListResponse> GetOfferToPurchasesAsync();

        /// <summary>
        /// Get Approved referrals count by Agent Ids.
        /// </summary>
        /// <param name="agentIds">A list of Agent Ids.</param>
        /// <returns>Referrals count.</returns>
        [Get("/api/referral-leads/approved-referrals-count-by-agents")]
        Task<List<ReferralLeadApprovedByAgentModel>> GetApprovedReferralsCountByAgentsAsync([Query(CollectionFormat.Multi)]List<Guid> agentIds);
    }
}
