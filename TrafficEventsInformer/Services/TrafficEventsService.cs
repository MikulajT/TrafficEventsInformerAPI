using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficEventsService : ITrafficEventsService
    {
        private readonly ITrafficRoutesRepository _trafficRoutesRepository;
        private readonly ITrafficEventsRepository _trafficEventsRepository;
        private readonly IGeoService _geoService;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<TrafficRoutesService> _localizer;
        public TrafficEventsService(ITrafficRoutesRepository trafficRoutesRepository,
            ITrafficEventsRepository trafficEventsRepository,
            IGeoService geoService,
            IConfiguration config,
            IStringLocalizer<TrafficRoutesService> localizer)
        {
            _trafficRoutesRepository = trafficRoutesRepository;
            _trafficEventsRepository = trafficEventsRepository;
            _geoService = geoService;
            _config = config;
            _localizer = localizer;
        }

        public IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId)
        {
            List<GetRouteEventNamesResponse> result = new List<GetRouteEventNamesResponse>();
            List<RouteEvent> routeEvents = _trafficEventsRepository.GetRouteEventNames(routeId).ToList();
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
            RouteEvent routeEvent = _trafficEventsRepository.GetRouteEventDetail(routeId, eventId);
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

        public async Task SyncAllRouteEvents()
        {
            await AddNewRouteEvents();
            InvalidateExpiredRouteEvents();
        }

        public async Task SyncRouteEvents(int routeId)
        {
            throw new NotImplementedException();
        }

        private async Task AddNewRouteEvents()
        {
            List<SituationRecord> activeTrafficEvents = await GetActiveTrafficEvents();
            List<RouteWithCoordinates> routesWithCoordinates = GetUsersRoutesWithCoordinates().ToList();
            foreach (var routeWithCoordinates in routesWithCoordinates)
            {
                List<SituationRecord> routeEvents = GetEventsOnUsersRoute(routeWithCoordinates.RouteCoordinates, activeTrafficEvents).ToList();
                foreach (var routeEvent in routeEvents)
                {
                    if (!_trafficEventsRepository.RouteEventExists(routeWithCoordinates.RouteId, routeEvent.id))
                    {
                        _trafficEventsRepository.AddRouteEvent(new RouteEvent()
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

        public IEnumerable<RouteWithCoordinates> GetUsersRoutesWithCoordinates()
        {
            var usersRouteCoordinates = new List<RouteWithCoordinates>();
            List<TrafficRoute> usersRoutes = _trafficRoutesRepository.GetUsersRoutes().ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(Gpx));
            foreach (var usersRoute in usersRoutes)
            {
                using (StringReader stringReader = new StringReader(usersRoute.Coordinates))
                {
                    Gpx route = (Gpx)serializer.Deserialize(stringReader);
                    usersRouteCoordinates.Add(new RouteWithCoordinates(usersRoute.Id, route.Trk.Trkseg.Trkpt));
                }
            }
            return usersRouteCoordinates;
        }

        public IEnumerable<SituationRecord> GetEventsOnUsersRoute(IEnumerable<Trkpt> routePoints, IEnumerable<SituationRecord> situations)
        {
            var result = new List<SituationRecord>();
            var convertedCoordinates = _geoService.ConvertCoordinates(situations);
            foreach (Trkpt routePoint in routePoints)
            {
                foreach (SituationRecord situation in situations)
                {
                    WgsPoint wgsPoint = convertedCoordinates[situation.id];
                    if (_geoService.AreCoordinatesWithinRadius(routePoint.Lat, routePoint.Lon, wgsPoint.Latitude, wgsPoint.Longtitude, 50) &&
                        !result.Any(x => x.id == situation.id))
                    {
                        result.Add(situation);
                    }
                }
            }
            return result;
        }

        private void InvalidateExpiredRouteEvents()
        {
            _trafficEventsRepository.InvalidateExpiredRouteEvents();
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
                    var content = await response.Content.ReadAsStringAsync();
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
