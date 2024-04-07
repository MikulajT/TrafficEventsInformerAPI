using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.Configuration;
using TrafficEventsInformer.Models.Fcm;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficEventsService : ITrafficEventsService
    {
        private readonly ITrafficRoutesRepository _trafficRoutesRepository;
        private readonly ITrafficEventsRepository _trafficEventsRepository;
        private readonly IGeoService _geoService;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<TrafficEventsService> _localizer;
        private readonly IPushNotificationService _pushNotificationService;
        public TrafficEventsService(ITrafficRoutesRepository trafficRoutesRepository,
            ITrafficEventsRepository trafficEventsRepository,
            IGeoService geoService,
            IConfiguration config,
            IStringLocalizer<TrafficEventsService> localizer,
            IPushNotificationService pushNotificationService)
        {
            _trafficRoutesRepository = trafficRoutesRepository;
            _trafficEventsRepository = trafficEventsRepository;
            _geoService = geoService;
            _config = config;
            _localizer = localizer;
            _pushNotificationService = pushNotificationService;
        }

        public IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId)
        {
            List<GetRouteEventNamesResponse> result = new List<GetRouteEventNamesResponse>();
            Dictionary<string, string> routeEvents = _trafficEventsRepository.GetRouteEventNames(routeId);
            foreach (var routeEvent in routeEvents)
            {
                result.Add(new GetRouteEventNamesResponse()
                {
                    Id = routeEvent.Key,
                    Name = routeEvent.Value
                });
            }
            return result;
        }

        public GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId)
        {
            RouteEventDetailEntities eventDetailEntities = _trafficEventsRepository.GetRouteEventDetail(routeId, eventId);
            GetRouteEventDetailResponse routeEventDetail = new GetRouteEventDetailResponse();
            if (eventDetailEntities != null)
            {
                routeEventDetail.Id = eventDetailEntities.RouteEvent.Id;
                routeEventDetail.Name = eventDetailEntities.TrafficRouteRouteEvent.Name;
                routeEventDetail.Type = _localizer[((EventTypes)eventDetailEntities.RouteEvent.Type).ToString()];
                routeEventDetail.Description = eventDetailEntities.RouteEvent.Description;
                routeEventDetail.StartDate = eventDetailEntities.RouteEvent.StartDate;
                routeEventDetail.EndDate = eventDetailEntities.RouteEvent.EndDate;
                routeEventDetail.DaysRemaining = (eventDetailEntities.RouteEvent.EndDate - DateTime.Now).Days;
                routeEventDetail.StartPointX = eventDetailEntities.RouteEvent.StartPointX;
                routeEventDetail.StartPointY = eventDetailEntities.RouteEvent.StartPointY;
                routeEventDetail.EndPointX = eventDetailEntities.RouteEvent.EndPointX;
                routeEventDetail.EndPointY = eventDetailEntities.RouteEvent.EndPointY;
            }
            return routeEventDetail;
        }

        public async Task SyncAllRouteEvents()
        {
            await AddNewRouteEvents();
            InvalidateExpiredRouteEvents();
        }

        public async Task<IEnumerable<GetRouteEventNamesResponse>> SyncRouteEvents(int routeId)
        {
            await AddNewRouteEvents(routeId);
            InvalidateExpiredRouteEvents(routeId);
            return GetRouteEventNames(routeId);
        }

        private async Task AddNewRouteEvents()
        {
            List<SituationRecord> activeTrafficEvents = await GetActiveTrafficEvents();
            List<RouteWithCoordinates> routesWithCoordinates = GetUsersRoutesWithCoordinates().ToList();
            foreach (var routeWithCoordinates in routesWithCoordinates)
            {
                await AddRouteEvent(activeTrafficEvents, routeWithCoordinates);
            }
        }

        private void InvalidateExpiredRouteEvents()
        {
            _trafficEventsRepository.InvalidateExpiredRouteEvents();
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

        private async Task AddNewRouteEvents(int routeId)
        {
            List<SituationRecord> activeTrafficEvents = await GetActiveTrafficEvents();
            RouteWithCoordinates routeWithCoordinates = GetUsersRouteWithCoordinates(routeId);
            await AddRouteEvent(activeTrafficEvents, routeWithCoordinates);
        }

        private void InvalidateExpiredRouteEvents(int routeId)
        {
            List<ExpiredRouteEventDto> expiredRouteEvents = _trafficEventsRepository.InvalidateExpiredRouteEvents(routeId).ToList();
            foreach (var expiredRouteEvent in expiredRouteEvents)
            {
                _pushNotificationService.SendEventEndNotificationAsync(expiredRouteEvent.EndDate, expiredRouteEvent.RouteNames, expiredRouteEvent.Id);
            }
        }

        private async Task<List<SituationRecord>> GetActiveTrafficEvents()
        {
            using (var httpClient = new HttpClient())
            {
                CommonTI commonTICredentials = _config.GetSection("CommonTI").Get<CommonTI>();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{commonTICredentials.Username}:{commonTICredentials.Password}"));
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

        public RouteWithCoordinates GetUsersRouteWithCoordinates(int routeId)
        {
            TrafficRoute usersRoute = _trafficRoutesRepository.GetUsersRoute(routeId);
            XmlSerializer serializer = new XmlSerializer(typeof(Gpx));
            using (StringReader stringReader = new StringReader(usersRoute.Coordinates))
            {
                Gpx route = (Gpx)serializer.Deserialize(stringReader);
                return new RouteWithCoordinates(usersRoute.Id, route.Trk.Trkseg.Trkpt);
            }
        }

        private async Task AddRouteEvent(List<SituationRecord> activeEvents, RouteWithCoordinates route)
        {
            List<SituationRecord> routeEvents = GetEventsOnUsersRoute(route.RouteCoordinates, activeEvents).ToList();
            foreach (var routeEvent in routeEvents)
            {
                if (!_trafficEventsRepository.RouteEventExists(routeEvent.id))
                {
                    TrafficRoute trafficRoute = _trafficRoutesRepository.GetUsersRoute(route.RouteId);
                    RouteEvent newRouteEvent = new RouteEvent()
                    {
                        Id = routeEvent.id,
                        Type = (int)Enum.Parse<EventTypes>(routeEvent.GetType().Name),
                        Description = routeEvent.generalPublicComment[0].comment.values[0].Value,
                        StartDate = routeEvent.validity.validityTimeSpecification.overallStartTime,
                        EndDate = routeEvent.validity.validityTimeSpecification.overallEndTime,
                        StartPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskX,
                        StartPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskY,
                        EndPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskX,
                        EndPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskY,
                        Expired = false
                    };

                    TrafficRouteRouteEvent trafficRouteRouteEvent = new TrafficRouteRouteEvent()
                    {
                        TrafficRouteId = trafficRoute.Id,
                        RouteEventId = newRouteEvent.Id,
                        Name = routeEvent.generalPublicComment[0].comment.values[0].Value.Split(',')[0]
                    };

                    _trafficEventsRepository.AddRouteEvent(newRouteEvent, trafficRouteRouteEvent);
                    _pushNotificationService.SendEventStartNotificationAsync(newRouteEvent.StartDate, new string[] { trafficRoute.Name }, newRouteEvent.Id);
                }
            }
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

        public void RenameRouteEvent(int routeId, string eventId, string name)
        {
            _trafficEventsRepository.RenameRouteEvent(routeId, eventId, name);
        }
    }
}
