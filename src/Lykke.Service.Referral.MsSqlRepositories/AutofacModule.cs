using Autofac;
using Lykke.Common.MsSql;
using Lykke.Service.Referral.Domain.Repositories;
using Lykke.Service.Referral.MsSqlRepositories.Repositories;

namespace Lykke.Service.Referral.MsSqlRepositories
{
    public class AutofacModule : Module
    {
        private readonly string _connectionString;

        public AutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _connectionString,
                connString => new ReferralContext(connString, false),
                dbConn => new ReferralContext(dbConn));

            builder.RegisterType<ReferralRepository>()
                .As<IReferralRepository>()
                .SingleInstance();

            builder.RegisterType<FriendReferralHistoryRepository>()
                .As<IFriendReferralHistoryRepository>()
                .SingleInstance();

            builder.RegisterType<PropertyPurchaseRepository>()
                .As<IPropertyPurchaseRepository>()
                .SingleInstance();

            builder.RegisterType<ReferralLeadRepository>()
                .As<IReferralLeadRepository>()
                .SingleInstance();

            builder.RegisterType<ReferralHotelsRepository>()
                .As<IReferralHotelsRepository>()
                .SingleInstance();

            builder.RegisterType<OfferToPurchasePurchaseRepository>()
                .As<IOfferToPurchasePurchaseRepository>()
                .SingleInstance();
        }
    }
}
