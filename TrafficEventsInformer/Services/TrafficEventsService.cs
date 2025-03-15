using Microsoft.Extensions.Localization;
using Serilog;
using System.IO.Compression;
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
        private readonly IStringLocalizer<TrafficEventsService> _localizer;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUsersService _usersService;
        private readonly IDopplerService _dopplerService;
        public TrafficEventsService(ITrafficRoutesRepository trafficRoutesRepository,
            ITrafficEventsRepository trafficEventsRepository,
            IGeoService geoService,
            IStringLocalizer<TrafficEventsService> localizer,
            IPushNotificationService pushNotificationService,
            IUsersService usersService,
            IDopplerService dopplerService)
        {
            _trafficRoutesRepository = trafficRoutesRepository;
            _trafficEventsRepository = trafficEventsRepository;
            _geoService = geoService;
            _localizer = localizer;
            _pushNotificationService = pushNotificationService;
            _usersService = usersService;
            _dopplerService = dopplerService;
        }

        public IEnumerable<RouteEventDto> GetRouteEvents(int routeId)
        {
            return _trafficEventsRepository.GetRouteEvents(routeId).ToList();
        }

        public GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId)
        {
            RouteEventDetailEntities eventDetailEntities = _trafficEventsRepository.GetRouteEventDetail(routeId, eventId);
            GetRouteEventDetailResponse routeEventDetail = new GetRouteEventDetailResponse();
            if (eventDetailEntities?.RouteEvent != null && eventDetailEntities?.TrafficRouteRouteEvent != null)
            {
                routeEventDetail.Id = eventDetailEntities.RouteEvent.Id;
                routeEventDetail.Name = eventDetailEntities.TrafficRouteRouteEvent.Name;
                routeEventDetail.Type = _localizer[((EventType)eventDetailEntities.RouteEvent.Type).ToString()];
                routeEventDetail.Description = eventDetailEntities.RouteEvent.Description;
                routeEventDetail.StartDate = eventDetailEntities.RouteEvent.StartDate;
                routeEventDetail.EndDate = eventDetailEntities.RouteEvent.EndDate;
                routeEventDetail.DaysRemaining = (eventDetailEntities.RouteEvent.EndDate - DateTime.Now).Days < 0 ? 0 : (eventDetailEntities.RouteEvent.EndDate - DateTime.Now).Days;
                routeEventDetail.StartPointX = eventDetailEntities.RouteEvent.StartPointX;
                routeEventDetail.StartPointY = eventDetailEntities.RouteEvent.StartPointY;
                routeEventDetail.EndPointX = eventDetailEntities.RouteEvent.EndPointX;
                routeEventDetail.EndPointY = eventDetailEntities.RouteEvent.EndPointY;
            }
            return routeEventDetail;
        }

        public void RenameRouteEvent(int routeId, string eventId, string name)
        {
            _trafficEventsRepository.RenameRouteEvent(routeId, eventId, name);
        }

        public async Task SyncRouteEventsAsync(string userId, int routeId)
        {
            List<SituationRecord> activeTrafficEvents = await GetRsdTrafficEvents();
            RouteCoordinates routeCoordinates = GetRouteCoordinates(routeId);
            await ProcessRouteEvent(activeTrafficEvents, routeCoordinates, userId);
            await InvalidateExpiredRouteEventsAsync();
        }

        private async Task<List<SituationRecord>> GetRsdTrafficEvents()
        {
            var situations = new List<SituationRecord>();

            using (var httpClient = new HttpClient())
            {
                DopplerSecrets dopplerSecrets = await _dopplerService.GetDopplerSecretsAsync();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{dopplerSecrets.CommonTIUsername}:{dopplerSecrets.CommonTIPassword}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                var apiUrl = "https://mobilitydata.rsd.cz/Resources/Dynamic/CommonTIDatex/";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();
                    situations = await FilterActiveRsdEvents(stream);
                }
            }

            return situations;
        }

        public RouteCoordinates GetRouteCoordinates(int routeId)
        {
            TrafficRoute usersRoute = _trafficRoutesRepository.GetRoute(routeId);
            XmlSerializer serializer = new XmlSerializer(typeof(Gpx));
            using (StringReader stringReader = new StringReader(usersRoute.Coordinates))
            {
                Gpx route = (Gpx)serializer.Deserialize(stringReader);
                return new RouteCoordinates(usersRoute.Id, route.Trk.Trkseg.Trkpt);
            }
        }

        public async Task InvalidateExpiredRouteEventsAsync()
        {
            List<ExpiredEventDto> expiredEvents = _trafficEventsRepository.InvalidateExpiredRouteEvents().ToList();

            foreach (var expiredEvent in expiredEvents)
            {
                foreach (var route in expiredEvent.Routes)
                {
                    await _pushNotificationService.SendEventEndNotificationAsync(expiredEvent.EndDate, route.RouteName, route.RouteId, expiredEvent.EventId, route.UserId);
                }
            }
        }

        /// <summary>
        /// Updates traffic events with data from RSP API, which are received in real time
        /// </summary>
        /// <param name="trafficEvents">Traffic events from RSP API</param>
        /// <returns></returns>
        public async Task SyncEvents(Stream trafficEvents)
        {
            List<SituationRecord> activeRsdEvents = await FilterActiveRsdEvents(trafficEvents);

            foreach (var rsdEvent in activeRsdEvents)
            {
                foreach (User user in _usersService.GetUsers())
                {
                    await ProcessActiveEvents(user.Id, activeRsdEvents);
                }
            }
        }

        private async Task<List<SituationRecord>> FilterActiveRsdEvents(Stream trafficEvents)
        {
            Log.Logger.Information("SyncEvents");

            using (var decompressedStream = new GZipStream(trafficEvents, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressedStream, Encoding.UTF8))
            {
                string xmlContent = await reader.ReadToEndAsync();

                // RSD data temporary fix (The specified type is abstract: name='Roadworks')
                xmlContent = xmlContent.Replace("xsi:type=\"Roadworks\"", "xsi:type=\"ConstructionWorks\"");

                // Deserialize XML into your D2LogicalModel object
                var serializer = new XmlSerializer(typeof(D2LogicalModel));
                using (var stringReader = new StringReader(xmlContent))
                {
                    var model = (D2LogicalModel)serializer.Deserialize(stringReader);

                    var situations = ((SituationPublication)model.payloadPublication).situation
                        .Select(situation => situation.situationRecord[0])
                        .Where(record => record.validity.validityTimeSpecification.overallEndTime > DateTime.Now)
                        .ToList();

                    Log.Logger.Information($"situations count: {situations.Count}");

                    return situations;
                }
            }
        }

        private IEnumerable<RouteCoordinates> GetRoutesCoordinates(string userId)
        {
            var usersRouteCoordinates = new List<RouteCoordinates>();
            List<TrafficRoute> usersRoutes = _trafficRoutesRepository.GetRoutes(userId).ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(Gpx));
            foreach (var usersRoute in usersRoutes)
            {
                using (StringReader stringReader = new StringReader(usersRoute.Coordinates))
                {
                    Gpx route = (Gpx)serializer.Deserialize(stringReader);
                    usersRouteCoordinates.Add(new RouteCoordinates(usersRoute.Id, route.Trk.Trkseg.Trkpt));
                }
            }
            return usersRouteCoordinates;
        }

        private async Task ProcessActiveEvents(string userId, List<SituationRecord> rsdTrafficEvents)
        {
            List<RouteCoordinates> routeCoordinates = GetRoutesCoordinates(userId).ToList();
            foreach (var routeWithCoordinates in routeCoordinates)
            {
                await ProcessRouteEvent(rsdTrafficEvents, routeWithCoordinates, userId);
            }
        }

        private async Task ProcessRouteEvent(List<SituationRecord> activeEvents, RouteCoordinates route, string userId)
        {
            List<SituationRecord> routeEvents = GetEventsOnRoute(route.Coordinates, activeEvents).ToList();

            foreach (var routeEvent in routeEvents)
            {
                TrafficRoute trafficRoute = _trafficRoutesRepository.GetRoute(route.RouteId);
                var routeEventEntity = new RouteEvent()
                {
                    Id = routeEvent.id,
                    Type = (int)Enum.Parse<EventType>(routeEvent.GetType().Name),
                    Description = routeEvent.generalPublicComment[0].comment.values[0].Value,
                    StartDate = routeEvent.validity.validityTimeSpecification.overallStartTime,
                    EndDate = routeEvent.validity.validityTimeSpecification.overallEndTime,
                    StartPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskX,
                    StartPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskY,
                    EndPointX = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskX,
                    EndPointY = ((Linear)routeEvent.groupOfLocations).globalNetworkLinear.endPoint.sjtskPointCoordinates.sjtskY,
                    Expired = false
                };

                if (_trafficEventsRepository.RouteEventExists(routeEvent.id))
                {
                    _trafficEventsRepository.UpdateRouteEvent(routeEventEntity);
                    //routeEventEntity = _trafficEventsRepository.GetRouteEvent(routeEvent.id);
                }
                else
                {
                    _trafficEventsRepository.AddRouteEvent(routeEventEntity);
                }

                if (!_trafficEventsRepository.IsRouteEventAssignedToUser(routeEvent.id, userId))
                {
                    TrafficRouteRouteEvent trafficRouteRouteEvent = new TrafficRouteRouteEvent()
                    {
                        TrafficRouteId = trafficRoute.Id,
                        RouteEventId = routeEventEntity.Id,
                        Name = routeEvent.generalPublicComment[0].comment.values[0].Value.Split(',')[0],
                        UserId = userId
                    };

                    _trafficEventsRepository.AssignRouteEventToUser(trafficRouteRouteEvent);

                    _pushNotificationService.SendEventStartNotificationAsync(routeEventEntity.StartDate, trafficRoute.Name, trafficRoute.Id, routeEventEntity.Id, userId);
                }
            }
        }

        private IEnumerable<SituationRecord> GetEventsOnRoute(IEnumerable<Trkpt> coordinates, IEnumerable<SituationRecord> trafficEvents)
        {
            var result = new List<SituationRecord>();
            var convertedCoordinates = _geoService.ConvertCoordinates(trafficEvents);
            foreach (Trkpt coordinate in coordinates)
            {
                foreach (SituationRecord trafficEvent in trafficEvents)
                {
                    // TODO: Replace TryGetValue with proper solution (related to null startPoint)
                    if (convertedCoordinates.TryGetValue(trafficEvent.id, out WgsPoint wgsPoint) &&
                        _geoService.AreCoordinatesWithinRadius(coordinate.Lat, coordinate.Lon, wgsPoint.Latitude, wgsPoint.Longtitude, 50) &&
                        !result.Any(x => x.id == trafficEvent.id))
                    {
                        result.Add(trafficEvent);
                    }
                }
            }
            return result;
        }
    }
}
