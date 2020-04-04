namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Lead Statistics Response
    /// </summary>
    public class LeadStatisticsResponse
    {
        /// <summary>
        /// Total number of leads in the system 
        /// </summary>
        public int NumberOfLeads { get; set; }

        /// <summary>
        /// Total number of approved leads in the system 
        /// </summary>
        public int NumberOfApprovedLeads { get; set; }

        /// <summary>
        /// Total number of leads who has made the expected final action 
        /// </summary>
        public int NumberOfUniqueCompletedLeads { get; set; }
    }
}
