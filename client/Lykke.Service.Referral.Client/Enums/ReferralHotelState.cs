namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents state of the hotel referral
    /// </summary>
    public enum ReferralHotelState
    {
        /// <summary>
        /// Referral has been created
        /// </summary>
        Pending,
        
        /// <summary>
        /// Referral has been confirmed
        /// </summary>
        Confirmed,
        
        /// <summary>
        /// Referral has been used
        /// </summary>
        Used,
        
        /// <summary>
        /// Referral has expired
        /// </summary>
        Expired
    }
}