using System.Text.Json.Serialization;

namespace TrafficEventsInformer.Models
{
    public class DopplerSecrets
    {
        [JsonPropertyName("DB_CONNECTION_STRING")]
        public string DbConnectionString { get; set; }

        [JsonPropertyName("COMMON_TI_USERNAME")]
        public string CommonTIUsername { get; set; }

        [JsonPropertyName("COMMON_TI_PASSWORD")]
        public string CommonTIPassword { get; set; }

        [JsonPropertyName("COMMON_TI_BASIC_AUTH_USERNAME")]
        public string CommonTIBasicAuthUsername { get; set; }

        [JsonPropertyName("COMMON_TI_BASIC_AUTH_PASSWORD")]
        public string CommonTIBasicAuthPassword { get; set; }

        [JsonPropertyName("GOOGLE_CLIENT_ID")]
        public string GoogleClientId { get; set; }
    }
}
