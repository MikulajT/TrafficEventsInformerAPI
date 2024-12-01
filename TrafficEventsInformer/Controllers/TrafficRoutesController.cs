using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    public class TrafficRoutesController : ControllerBase
    {
        private readonly ITrafficRoutesService _trafficRouteService;

        public TrafficRoutesController(ITrafficRoutesService trafficRouteService)
        {
            _trafficRouteService = trafficRouteService;
        }

        [HttpGet]
        [Route("api/users/{userId}/trafficRoutes")]
        public IActionResult GetRoutes(string userId)
        {
            return Ok(_trafficRouteService.GetTrafficRouteNames(userId));
        }

        [HttpPost]
        [Route("api/trafficRoutes")]
        public async Task<IActionResult> AddRouteAsync([FromForm] AddRouteRequestDto requestData)
        {
            int routeId = _trafficRouteService.AddRoute(requestData);
            return Ok(routeId);
        }

        [HttpDelete]
        [Route("api/trafficRoutes/{routeId:int}")]
        public IActionResult DeleteRoute(int routeId)
        {
            _trafficRouteService.DeleteRoute(routeId);
            return Ok();
        }

        [HttpPut]
        [Route("api/trafficRoutes/{routeId:int}")]
        public IActionResult RenameRoute(int routeId, [FromBody] UpdateRouteRequest requestData)
        {
            requestData.RouteId = routeId;
            _trafficRouteService.RenameRoute(requestData);
            return Ok();
        }
    }
}
