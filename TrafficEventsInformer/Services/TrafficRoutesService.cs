using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficRoutesService : ITrafficRoutesService
    {
        private readonly ITrafficRoutesRepository _trafficRouteRepository;
        private readonly IGeoService _geoService;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<TrafficRoutesService> _localizer;

        public TrafficRoutesService(ITrafficRoutesRepository trafficRouteRepository, IGeoService geoService, IConfiguration configuration, IStringLocalizer<TrafficRoutesService> stringLocalizer)
        {
            _trafficRouteRepository = trafficRouteRepository;
            _geoService = geoService;
            _config = configuration;
            _localizer = stringLocalizer;
        }

        public IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames()
        {
            List<GetTrafficRouteNamesResponse> result = new List<GetTrafficRouteNamesResponse>();
            List<TrafficRoute> trafficRoutes = _trafficRouteRepository.GetTrafficRouteNames().ToList();
            foreach (var trafficRoute in trafficRoutes)
            {
                result.Add(new GetTrafficRouteNamesResponse()
                {
                    Id = trafficRoute.Id,
                    Name = trafficRoute.Name
                });
            }
            return result;
        }

        public async Task AddRouteAsync(AddRouteRequest routeRequest)
        {
            var serializer = new XmlSerializer(typeof(Gpx));
            using (var reader = new StreamReader(routeRequest.RouteFile.OpenReadStream()))
            {
                Gpx routeCoordinates = (Gpx)serializer.Deserialize(reader);
                using (var stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, routeCoordinates);
                    string serializedFileContents = stringWriter.ToString();
                    _trafficRouteRepository.AddRoute(routeRequest.RouteName, serializedFileContents);
                }
            }
        }

        public void DeleteRoute(int routeId)
        {
            _trafficRouteRepository.DeleteRoute(routeId);
        }
    }
}
