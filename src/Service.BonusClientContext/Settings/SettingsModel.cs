using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BonusClientContext.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BonusClientContext.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BonusClientContext.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BonusClientContext.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("BonusClientContext.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("BonusClientContext.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}
