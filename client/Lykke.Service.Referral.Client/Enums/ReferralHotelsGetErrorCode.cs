namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents error that could happen while getting referrals
    /// </summary>
    public enum ReferralHotelsGetErrorCode
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        
        /// <summary>
        /// Customer does not exist
        /// </summary>
        CustomerDoesNotExist
    }
}