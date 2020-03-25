namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// The error codes of the real restate creation operation
    /// </summary>
    public enum RealEstatePurchaseErrorCode 
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        None,
        /// <summary>
        /// The referral in the operation does not exist.
        /// </summary>
        ReferralNotFound
    }
}
