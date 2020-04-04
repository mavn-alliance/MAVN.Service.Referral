using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface ICommonReferralService
    {
        Task<IReadOnlyCollection<CommonReferralModel>> GetForCustomerAsync(
            Guid customerId, 
            Guid? campaignId,
            List<CommonReferralStatus> statuses);
        Task<IDictionary<string, CommonReferralModel>> GetByReferralIdsAsync(List<Guid> referralIds);
    }
}
