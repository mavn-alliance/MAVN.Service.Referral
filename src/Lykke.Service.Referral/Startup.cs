using System;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.Profiles;
using Lykke.Service.Referral.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.Referral
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "Referral API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Extend = (collection, manager) =>
                {
                    collection.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
                    {
                        apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
                    });
                    collection.AddAutoMapper(
                        typeof(ServiceProfile),
                        typeof(AutoMapperProfile),
                        typeof(DomainServices.AutoMapperProfile));
                };

                options.Logs = logs =>
                {
                    logs.AzureTableName = "ReferralLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.ReferralService.Db.LogsConnString;
                };
            });
        }

        [UsedImplicitly]    
        public void Configure(IApplicationBuilder app, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
