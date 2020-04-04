namespace MAVN.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents error that could happen while creating referral
    /// </summary>
    public enum ReferralHotelCreateErrorCode
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        
        /// <summary>
        /// Customer does not exist
        /// </summary>
        CustomerDoesNotExist,
        
        /// <summary>
        /// Referral has already been confirmed
        /// </summary>
        ReferralAlreadyConfirmed,
        
        /// <summary>
        /// Referral limit exceeded
        /// </summary>
        ReferralsLimitExceeded,
        
        /// <summary>
        /// Referral for given email and referrer already exists
        /// </summary>
        ReferralAlreadyExist,
        
        /// <summary>
        /// Agent can't refer himself
        /// </summary>
        AgentCantReferHimself,

        /// <summary>
        /// Campaign not found.
        /// </summary>
        CampaignNotFound,

        /// <summary>
        /// Referral has expired
        /// </summary>
        ReferralExpired,

        /// <summary>
        /// The stake for the referral is invalid
        /// </summary>
        InvalidStake
    }
}
