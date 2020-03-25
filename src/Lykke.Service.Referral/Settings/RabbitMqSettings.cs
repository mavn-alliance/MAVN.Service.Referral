using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Referral.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}
