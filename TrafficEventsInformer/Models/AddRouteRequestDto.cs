namespace TrafficEventsInformer.Models
{
    public class AddRouteRequestDto
    {
        public string RouteName { get; set; }

        /// <summary>
        /// File with route coordinates in WGS-84 format
        /// </summary>
        public IFormFile RouteFile { get; set; }
        public string UserId { get; set; }
    }
}
