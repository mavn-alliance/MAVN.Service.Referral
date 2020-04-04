using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.Referral.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}
