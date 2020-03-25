namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents error that could happen while confirming referral
    /// </summary>
    public enum ReferralHotelConfirmErrorCode
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
        /// Referral has already been confirmed
        /// </summary>
        ReferralAlreadyConfirmed,
        
        /// <summary>
        /// Referral has expired
        /// </summary>
        ReferralExpired
    }
}