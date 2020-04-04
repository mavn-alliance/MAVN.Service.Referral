namespace MAVN.Service.Referral.Client.Enums
{
    /// <summary>
    /// Represents Referrals error codes
    /// </summary>
    public enum ReferralErrorCodes
    {
        /// <summary>
        /// Empty code
        /// </summary>
        None,
        /// <summary>
        /// Referral code not found.
        /// </summary>
        ReferralNotFound,
        /// <summary>
        /// Passed values can not be parsed to guid.
        /// </summary>
        GuidCanNotBeParsed,
        /// <summary>
        /// Processing of the referral lead failed.
        /// </summary>
        ReferralLeadProcessingFailed,
        /// <summary>
        /// Lead with the same Phone number or Email is already referred.
        /// </summary>
        ReferralLeadAlreadyExist,
        /// <summary>
        /// Lead with the provided id does not exist.
        /// </summary>
        ReferralDoesNotExist,
        /// <summary>
        /// Lead with the credentials of the given referral is already confirmed.
        /// </summary>
        LeadAlreadyConfirmed,
        /// <summary>
        /// Customer isn't an approved agent.
        /// </summary>
        CustomerNotApprovedAgent,
        /// <summary>
        /// Customer doesn't exist.
        /// </summary>
        CustomerDoesNotExist,
        /// <summary>
        /// Country code doesn't exist.
        /// </summary>
        CountryCodeDoesNotExist,
        /// <summary>
        /// Can not refer yourself.
        /// </summary>
        ReferYourself,
        /// <summary>
        /// Lead is already confirmed
        /// </summary>
        ReferralLeadAlreadyConfirmed,

        /// <summary>
        /// Invalid phone number.
        /// </summary>
        InvalidPhoneNumber,

        /// <summary>
        /// Campaign not found.
        /// </summary>
        CampaignNotFound,

        /// <summary>
        /// The stake for the referral is invalid
        /// </summary>
        InvalidStake,
        /// <summary>
        /// Referred friend with the same Phone number or Email is already referred.
        /// </summary>
        ReferralFriendAlreadyExist,
    }
}
