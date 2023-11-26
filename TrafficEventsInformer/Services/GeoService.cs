using TrafficEventsInformer.Models;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using TrafficEventsInformer.Models.UsersRoute;
using System.Xml.Serialization;

namespace TrafficEventsInformer.Services
{
    public class GeoService : IGeoService
    {
        private const double EarthRadiusKm = 6371.0;
        private readonly IConfiguration _config;

        public GeoService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<Trkpt> GetUsersRoute()
        {
            string filePath = _config["RoutePath"];
            string xmlData = File.ReadAllText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(Gpx));
            using (StringReader stringReader = new StringReader(xmlData))
            {
                Gpx route = (Gpx)serializer.Deserialize(stringReader);
                return route.Trk.Trkseg.Trkpt;
            }
        }

        public IEnumerable<SituationRecord> GetEventsOnUsersRoute(IEnumerable<Trkpt> routePoints, IEnumerable<SituationRecord> situations)
        {
            var result = new List<SituationRecord>();
            var convertedCoordinates = ConvertCoordinates(situations);
            foreach (Trkpt routePoint in routePoints)
            {
                foreach (SituationRecord situation in situations)
                {
                    WgsPoint wgsPoint = convertedCoordinates[situation.id];
                    if (AreCoordinatesWithinRadius(routePoint.Lat, routePoint.Lon, wgsPoint.Latitude, wgsPoint.Longtitude, 50) &&
                        !result.Any(x => x.id == situation.id))
                    {
                        result.Add(situation);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts coordinates of <paramref name="situations"/> from S-JTSK to WGS-84
        /// </summary>
        /// <param name="situations"></param>
        /// <returns>Dictionary with id of situation as a key and WGS-84 coordinate as a value</returns>
        private Dictionary<string, WgsPoint> ConvertCoordinates(IEnumerable<SituationRecord> situations)
        {
            var convertedCoordinates = new Dictionary<string, WgsPoint>();
            foreach (SituationRecord situation in situations)
            {
                situation.id = situation.id.Split('_')[0];
                WgsPoint wgsPoint = ConvertSjtskToWgs84(((Linear)situation.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskX, ((Linear)situation.groupOfLocations).globalNetworkLinear.startPoint.sjtskPointCoordinates.sjtskY);
                convertedCoordinates[situation.id] = wgsPoint;
            }
            return convertedCoordinates;
        }

        private WgsPoint ConvertSjtskToWgs84(double xSjtsk, double ySjtsk)
        {
            // Define the coordinate system for S-JTSK and WGS84
            ICoordinateSystemFactory csFactory = new CoordinateSystemFactory();
            ICoordinateSystem sjtskCs = csFactory.CreateFromWkt(ProjNetCoordinateSystems.SJTSK);
            ICoordinateSystem wgs84Cs = csFactory.CreateFromWkt(ProjNetCoordinateSystems.WGS84);

            // Define the coordinate transformation parameters
            CoordinateTransformationFactory ctFactory = new CoordinateTransformationFactory();
            ICoordinateTransformation transformation = ctFactory.CreateFromCoordinateSystems(sjtskCs, wgs84Cs);

            // Perform the coordinate transformation
            double[] sjtskCoord = new double[] { xSjtsk, ySjtsk };
            double[] wgs84Coord = transformation.MathTransform.Transform(sjtskCoord);

            return new WgsPoint()
            {
                Latitude = wgs84Coord[1],
                Longtitude = wgs84Coord[0]
            };
        }

        private bool AreCoordinatesWithinRadius(double lat1, double lon1, double lat2, double lon2, double radiusMeters)
        {
            double distance = CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2) * 1000; // Convert to meters
            return distance <= radiusMeters;
        }

        private double CalculateDistanceBetweenCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
