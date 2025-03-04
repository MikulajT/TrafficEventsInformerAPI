using System.Text.Json;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public class DopplerService : IDopplerService
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _config;

        public DopplerService(IHostEnvironment environment, IConfiguration config)
        {
            _environment = environment;
            _config = config;
        }

        public async Task<DopplerSecrets> GetDopplerSecretsAsync()
        {
            var dopplerToken = _environment.IsDevelopment() ? _config["DopplerToken"] : Environment.GetEnvironmentVariable("DOPPLER_TOKEN");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", dopplerToken);
                var streamTask = client.GetStreamAsync("https://api.doppler.com/v3/configs/config/secrets/download?format=json");
                var secrets = await JsonSerializer.DeserializeAsync<DopplerSecrets>(await streamTask);

                return secrets;
            }
        }
    }
}
