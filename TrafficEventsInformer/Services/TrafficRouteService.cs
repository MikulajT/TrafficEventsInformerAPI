using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficRouteService : ITrafficRouteService
    {
        private readonly ITrafficRouteRepository _trafficRouteRepository;

        public TrafficRouteService(ITrafficRouteRepository trafficRouteRepository)
        {
            _trafficRouteRepository = trafficRouteRepository;
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

        public GetRouteEventDetailResponse GetRouteEventDetail(int routeId, int eventId)
        {
            RouteEvent routeEvent = _trafficRouteRepository.GetRouteEventDetail(routeId, eventId);
            GetRouteEventDetailResponse result = new GetRouteEventDetailResponse();
            if (routeEvent != null)
            {
                result.EventId = routeEvent.Id;
                result.EventType = routeEvent.Type;
                result.Description = routeEvent.Description;
                result.StartDate = routeEvent.StartDate;
                result.EndDate = routeEvent.EndDate;
                result.DaysRemaining = (routeEvent.EndDate - routeEvent.StartDate).Days;
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
    }
}
