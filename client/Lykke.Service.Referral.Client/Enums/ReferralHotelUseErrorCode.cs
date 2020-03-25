namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents error that could happen while using referral
    /// </summary>
    public enum ReferralHotelUseErrorCode
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        
        /// <summary>
        /// Referral does not exist
        /// </summary>
        ReferralDoesNotExist,
        
        /// <summary>
        /// Currency does not exist
        /// </summary>
        CurrencyDoesNotExist,
        
        /// <summary>
        /// Referral has expired
        /// </summary>
        ReferralExpired,
        
        /// <summary>
        /// Referral has not been confirmed
        /// </summary>
        ReferralNotConfirmed,
        
        /// <summary>
        /// Referral has already been used
        /// </summary>
        ReferralAlreadyUsed,

        /// <summary>
        /// Request partner was not found
        /// </summary>
        PartnerNotFound
    }
}
