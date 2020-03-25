namespace Lykke.Service.Referral.Domain.Models
{
    /// <summary>
    /// Represents an encrypted referral lead with details.
    /// </summary>
    public class ReferralLeadEncryptedWithDetails : ReferralLeadEncrypted
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
