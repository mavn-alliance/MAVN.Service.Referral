namespace MAVN.Service.Referral.Domain.Models
{
    /// <summary>
    /// Represents referral lead with details.
    /// </summary>
    public class ReferralLeadWithDetails : ReferralLead
    {
        /// <summary>
        /// The number of SPA (sales purchase agreement)
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// The number of OTP (offers to purchase).
        /// </summary>
        public int OffersCount { get; set; }
    }
}
