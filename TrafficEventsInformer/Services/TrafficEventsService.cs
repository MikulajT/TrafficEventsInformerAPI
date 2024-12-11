using Microsoft.Extensions.Localization;
using Serilog;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.Configuration;
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
        private readonly IUsersService _usersService;
        public TrafficEventsService(ITrafficRoutesRepository trafficRoutesRepository,
            ITrafficEventsRepository trafficEventsRepository,
            IGeoService geoService,
            IConfiguration config,
            IStringLocalizer<TrafficEventsService> localizer,
            IPushNotificationService pushNotificationService,
            IUsersService usersService)
        {
            _trafficRoutesRepository = trafficRoutesRepository;
            _trafficEventsRepository = trafficEventsRepository;
            _geoService = geoService;
            _config = config;
            _localizer = localizer;
            _pushNotificationService = pushNotificationService;
            _usersService = usersService;
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
                routeEventDetail.DaysRemaining = (eventDetailEntities.RouteEvent.EndDate - DateTime.Now).Days;
                routeEventDetail.StartPointX = eventDetailEntities.RouteEvent.StartPointX;
                routeEventDetail.StartPointY = eventDetailEntities.RouteEvent.StartPointY;
                routeEventDetail.EndPointX = eventDetailEntities.RouteEvent.EndPointX;
                routeEventDetail.EndPointY = eventDetailEntities.RouteEvent.EndPointY;
            }
            return routeEventDetail;
        }

        /// <summary>
        /// Used for automatic periodical sync
        /// </summary>
        /// <returns></returns>
        public async Task SyncRouteEventsAsync()
        {
            List<SituationRecord> activeTrafficEvents = await GetRsdTrafficEvents();

            foreach (User user in _usersService.GetUsers())
            {
                await AddRouteEvents(user.Id, activeTrafficEvents);
            }

            InvalidateExpiredRouteEvents();
        }

        public async Task SyncRouteEventsAsync(string userId)
        {
            List<SituationRecord> activeTrafficEvents = await GetRsdTrafficEvents();
            await AddRouteEvents(userId, activeTrafficEvents);
            InvalidateExpiredRouteEvents();
        }

        private async Task<List<SituationRecord>> GetRsdTrafficEvents()
        {
            using (var httpClient = new HttpClient())
            {
                CommonTI commonTICredentials = _config.GetSection("CommonTI").Get<CommonTI>();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{commonTICredentials.Username}:{commonTICredentials.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                var apiUrl = "https://mobilitydata.rsd.cz/Resources/Dynamic/CommonTIDatex/";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                var situations = new List<SituationRecord>();

                if (response.IsSuccessStatusCode)
                {
                    string content = "";

                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                        {
                            using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                            using (var reader = new StreamReader(decompressedStream, Encoding.UTF8))
                            {
                                content = await reader.ReadToEndAsync();
                            }
                        }
                    }

                    // RSD data temporary fix (The specified type is abstract: name='Roadworks')
                    content = content.Replace("xsi:type=\"Roadworks\"", "xsi:type=\"ConstructionWorks\"");

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

        private async Task AddRouteEvents(string userId, List<SituationRecord> rsdTrafficEvents)
        {
            List<RouteCoordinates> routeCoordinates = GetRouteCoordinates(userId).ToList();
            foreach (var routeWithCoordinates in routeCoordinates)
            {
                await AddRouteEvent(rsdTrafficEvents, routeWithCoordinates, userId);
            }
        }

        private IEnumerable<RouteCoordinates> GetRouteCoordinates(string userId)
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

        private void InvalidateExpiredRouteEvents()
        {
            _trafficEventsRepository.InvalidateExpiredRouteEvents();
        }

        private async Task AddNewRouteEvents(int routeId, string userId, List<SituationRecord> rsdTrafficEvents)
        {
            RouteCoordinates routeWithCoordinates = GetRouteCoordinates(routeId);
            await AddRouteEvent(rsdTrafficEvents, routeWithCoordinates, userId);
        }

        private void InvalidateExpiredRouteEvents(int routeId)
        {
            List<ExpiredRouteEventDto> expiredRouteEvents = _trafficEventsRepository.InvalidateExpiredRouteEvents(routeId).ToList();
            foreach (var expiredRouteEvent in expiredRouteEvents)
            {
                _pushNotificationService.SendEventEndNotificationAsync(expiredRouteEvent.EndDate, expiredRouteEvent.RouteNames, routeId, expiredRouteEvent.Id, expiredRouteEvent.UserId);
            }
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

        private async Task AddRouteEvent(List<SituationRecord> activeEvents, RouteCoordinates route, string userId)
        {
            List<SituationRecord> routeEvents = GetEventsOnRoute(route.Coordinates, activeEvents).ToList();
            foreach (var routeEvent in routeEvents)
            {
                TrafficRoute trafficRoute = _trafficRoutesRepository.GetRoute(route.RouteId);
                RouteEvent routeEventEntity;

                if (_trafficEventsRepository.RouteEventExists(routeEvent.id))
                {
                    routeEventEntity = _trafficEventsRepository.GetRouteEvent(routeEvent.id);
                }
                else
                {
                    routeEventEntity = new RouteEvent()
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
                    _pushNotificationService.SendEventStartNotificationAsync(routeEventEntity.StartDate, new string[] { trafficRoute.Name }, trafficRoute.Id, routeEventEntity.Id, userId);
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

        public void RenameRouteEvent(int routeId, string eventId, string name)
        {
            _trafficEventsRepository.RenameRouteEvent(routeId, eventId, name);
        }
    }
}
