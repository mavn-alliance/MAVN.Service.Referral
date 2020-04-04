using System;
using System.Text.RegularExpressions;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.Referral.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.Referral.Controllers
{
    public class BaseController: Controller
    {
        private readonly ISettingsService _settingsService;
        protected const string InvalidIdentifierMessage = "Invalid identifier";
        protected readonly ILog Log;
        public BaseController(
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _settingsService = settingsService;
            Log = logFactory.CreateLog(this);
        }

        protected bool TryParseGuid(string input, string processName, out Guid output)
        {
            if (!Guid.TryParse(input, out output))
            {
                Log.Info(InvalidIdentifierMessage, process: processName, context: input);

                return false;
            }

            return true;
        }

        protected bool IsDemoMode(string customerEmail)
        {
            var r = new Regex($"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\\.)?[a-zA-Z]+\\.)?({_settingsService.GetDemoEmailIdentifier()})$");

            return r.Match(customerEmail).Success;
        }
    }
}
