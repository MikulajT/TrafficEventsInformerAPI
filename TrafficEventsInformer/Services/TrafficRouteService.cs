using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficRouteService : ITrafficRouteService
    {
        private readonly ITrafficRouteRepository _trafficRouteRepository;
        private readonly IGeoService _geoService;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<TrafficRouteService> _localizer;

        public TrafficRouteService(ITrafficRouteRepository trafficRouteRepository, IGeoService geoService, IConfiguration configuration, IStringLocalizer<TrafficRouteService> stringLocalizer)
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

        public IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId)
        {
            List<GetRouteEventNamesResponse> result = new List<GetRouteEventNamesResponse>();
            List<RouteEvent> routeEvents = _trafficRouteRepository.GetRouteEventNames(routeId).ToList();
            foreach (var routeEvent in routeEvents)
            {
                result.Add(new GetRouteEventNamesResponse()
                {
                    Id = routeEvent.Id,
                    Name = routeEvent.Name
                });
            }
            return result;
        }

        public GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId)
        {
            RouteEvent routeEvent = _trafficRouteRepository.GetRouteEventDetail(routeId, eventId);
            GetRouteEventDetailResponse result = new GetRouteEventDetailResponse();
            if (routeEvent != null)
            {
                result.Id = routeEvent.Id;
                result.Type = _localizer[((EventTypes)routeEvent.Type).ToString()];
                result.Description = routeEvent.Description;
                result.StartDate = routeEvent.StartDate;
                result.EndDate = routeEvent.EndDate;
                result.DaysRemaining = (routeEvent.EndDate - routeEvent.StartDate).Days;
                result.StartPointX = routeEvent.StartPointX;
                result.StartPointY = routeEvent.StartPointY;
                result.EndPointX = routeEvent.EndPointX;
                result.EndPointY = routeEvent.EndPointY;
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

        public async Task SyncUsersRouteEvents()
        {
            await AddNewRouteEvents();
            InvalidateExpiredRouteEvents();
        }

        private async Task AddNewRouteEvents()
        {
            List<SituationRecord> activeTrafficEvents = await GetActiveTrafficEvents();
            List<RouteWithCoordinates> routesWithCoordinates = _geoService.GetUsersRouteWithCoordinates().ToList();
            foreach (var routeWithCoordinates in routesWithCoordinates)
            {
                List<SituationRecord> routeEvents = _geoService.GetEventsOnUsersRoute(routeWithCoordinates.RouteCoordinates, activeTrafficEvents).ToList();
                foreach (var routeEvent in routeEvents)
                {
                    if (!_trafficRouteRepository.RouteEventExists(routeWithCoordinates.RouteId, routeEvent.id))
                    {
                        _trafficRouteRepository.AddRouteEvent(new RouteEvent()
                        {
                            Id = routeEvent.id,
                            Type = (int)Enum.Parse<EventTypes>(routeEvent.GetType().Name),
                            Name = routeEvent.generalPublicComment[0].comment.values[0].Value.Split(',')[0],
                            Description = routeEvent.generalPublicComment[0].comment.values[0].Value,
                            StartDate = routeEvent.validity.validityTimeSpecification.overallStartTime,
                            EndDate = routeEvent.validity.validityTimeSpecification.overallEndTime,
                            StartPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskX,
                            StartPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskY,
                            EndPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskX,
                            EndPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskY,
                            RouteId = routeWithCoordinates.RouteId
                        });
                    }
                }
            }
        }

        private void InvalidateExpiredRouteEvents()
        {
            _trafficRouteRepository.InvalidateExpiredRouteEvents();
        }

        private async Task<List<SituationRecord>> GetActiveTrafficEvents()
        {
            using (var httpClient = new HttpClient())
            {
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config["CommonTiLogin"]}:{_config["CommonTiPassword"]}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
                var apiUrl = "https://mobilitydata.rsd.cz/Resources/Dynamic/CommonTIDatex/";
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                var situations = new List<SituationRecord>();
                if (response.IsSuccessStatusCode)
                {
                    var content =  await response.Content.ReadAsStringAsync();
                    var serializer = new XmlSerializer(typeof(D2LogicalModel));
                    using (StringReader stringReader = new StringReader(content))
                    {
                        var filteredSituations = (D2LogicalModel)serializer.Deserialize(stringReader);
                        situations = ((SituationPublication)filteredSituations.payloadPublication).situation
                            .Select(situation => situation.situationRecord[0])
                            .Where(record => record.validity.validityTimeSpecification.overallEndTime > DateTime.Now)
                            .ToList();
                    }
                }
                return situations;
            }
        }
    }
}
