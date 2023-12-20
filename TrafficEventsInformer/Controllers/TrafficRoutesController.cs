using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    public class TrafficRoutesController : ControllerBase
    {
        private readonly ITrafficRouteService _trafficRouteService;
        private readonly IStringLocalizer<TrafficRoutesController> _localizer;

        public TrafficRoutesController(ITrafficRouteService trafficRouteService, IStringLocalizer<TrafficRoutesController> localizer)
        {
            _trafficRouteService = trafficRouteService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("api/trafficRoutes")]
        public IActionResult GetTrafficRoutes()
        {
            return Ok(_trafficRouteService.GetTrafficRouteNames());
        }

        [HttpGet]
        [Route("api/trafficRoutes/{routeId:int}/events")]
        public IActionResult GetRouteEventNames(int routeId)
        {
            return Ok(_trafficRouteService.GetRouteEventNames(routeId));
        }

        [HttpGet]
        [Route("api/trafficRoutes/{routeId:int}/events/{eventId:Guid}")]
        public IActionResult GetRouteEventDetail(int routeId, string eventId)
        {
            return Ok(_trafficRouteService.GetRouteEventDetail(routeId, eventId));
        }

        [HttpPost]
        [Route("api/trafficRoutes")]
        public async Task<IActionResult> AddRouteAsync([FromForm] AddRouteRequest requestData)
        {
            await _trafficRouteService.AddRouteAsync(requestData);
            return Ok();
        }

        [HttpDelete]
        [Route("api/trafficRoutes")]
        public IActionResult DeleteRoute(int routeId)
        {
            _trafficRouteService.DeleteRoute(routeId);
            return Ok();
        }
    }
}
