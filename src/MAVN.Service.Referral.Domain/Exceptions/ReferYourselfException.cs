using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferYourselfException : Exception
    {
        public ReferYourselfException()
        {
            
        }
        
        public ReferYourselfException(string message)
        : base(message)
        {

        }
    }
}
