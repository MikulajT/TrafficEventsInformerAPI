using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public class TrafficRoutesService : ITrafficRoutesService
    {
        private readonly ITrafficRoutesRepository _trafficRouteRepository;

        public TrafficRoutesService(ITrafficRoutesRepository trafficRouteRepository)
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

        public int AddRoute(AddRouteRequest routeRequest)
        {
            var serializer = new XmlSerializer(typeof(Gpx));
            using (var reader = new StreamReader(routeRequest.RouteFile.OpenReadStream()))
            {
                Gpx routeCoordinates = (Gpx)serializer.Deserialize(reader);
                using (var stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, routeCoordinates);
                    string textCoordinates = stringWriter.ToString();
                    textCoordinates = SanitizeXml(textCoordinates);
                    return _trafficRouteRepository.AddRoute(routeRequest.RouteName, textCoordinates);
                }
            }
        }

        public void DeleteRoute(int routeId)
        {
            _trafficRouteRepository.DeleteRoute(routeId);
        }

        public void UpdateRoute(UpdateRouteRequest requestData)
        {
            _trafficRouteRepository.UpdateRoute(requestData);
        }

        private string SanitizeXml(string xml)
        {
            // Define a regex pattern to match forbidden characters
            string forbiddenPattern = @"[\x00-\x08\x0B\x0C\x0E-\x1F]";

            // Use Regex.Replace to remove forbidden characters
            string sanitizedXml = Regex.Replace(xml, forbiddenPattern, string.Empty);

            return sanitizedXml;
        }
    }
}
