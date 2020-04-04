using AutoMapper;

namespace MAVN.Service.Referral.Tests
{
    public class MapperHelper
    {
        public static IMapper CreateAutoMapper()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Profiles.ServiceProfile());
                cfg.AddProfile(new Referral.DomainServices.AutoMapperProfile());
                cfg.AddProfile(new Referral.MsSqlRepositories.AutoMapperProfile());
            });

            return mockMapper.CreateMapper();
        }
    }
}
