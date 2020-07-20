using Autofac;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Repositories;

namespace MAVN.Service.Referral.MsSqlRepositories
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
            builder.RegisterPostgreSQL(
                _connectionString,
                connString => new ReferralContext(connString, false),
                dbConn => new ReferralContext(dbConn));

            builder.RegisterType<ReferralRepository>()
                .As<IReferralRepository>()
                .SingleInstance();

            builder.RegisterType<FriendReferralHistoryRepository>()
                .As<IFriendReferralHistoryRepository>()
                .SingleInstance();

            builder.RegisterType<ReferralHotelsRepository>()
                .As<IReferralHotelsRepository>()
                .SingleInstance();
        }
    }
}
