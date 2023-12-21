using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficEventsController : ControllerBase
    {
        private readonly ITrafficEventsService _trafficEventsService;

        public TrafficEventsController(ITrafficEventsService trafficEventsService)
        {
            _trafficEventsService = trafficEventsService;
        }

        [HttpGet]
        [Route("api/trafficRoutes/{routeId:int}/events")]
        public IActionResult GetRouteEventNames(int routeId)
        {
            return Ok(_trafficEventsService.GetRouteEventNames(routeId));
        }

        [HttpGet]
        [Route("api/trafficRoutes/{routeId:int}/events/{eventId:Guid}")]
        public IActionResult GetRouteEventDetail(int routeId, string eventId)
        {
            return Ok(_trafficEventsService.GetRouteEventDetail(routeId, eventId));
        }

        [HttpPost]
        [Route("api/trafficRoutes/events/sync")]
        public async Task<IActionResult> SyncAllRouteEvents()
        {
            await _trafficEventsService.SyncAllRouteEvents();
            return Ok();
        }

        [HttpPost]
        [Route("api/trafficRoutes/{routeId:int}/events/sync")]
        public async Task<IActionResult> SyncRouteEvents(int routeId)
        {
            await _trafficEventsService.SyncRouteEvents(routeId);
            return Ok();
        }
    }
}