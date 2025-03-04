using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface IDopplerService
    {
        Task<DopplerSecrets> GetDopplerSecretsAsync();
    }
}
