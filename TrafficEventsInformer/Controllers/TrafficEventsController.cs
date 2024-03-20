using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    public class TrafficEventsController : ControllerBase
    {
        private readonly ITrafficEventsService _trafficEventsService;
        private readonly IPushNotificationService _pushNotificationService;

        public TrafficEventsController(ITrafficEventsService trafficEventsService, IPushNotificationService pushNotificationService)
        {
            _trafficEventsService = trafficEventsService;
            _pushNotificationService = pushNotificationService;
        }

        [HttpGet]
        [Route("api/trafficRoutes/fcmTest")]
        public IActionResult fcmTest()
        {
            _pushNotificationService.SendEventStartNotificationAsync(DateTime.Now, new string[] { "nazev trasy1", "nazev trasy2" }, "1e7ea65d-60be-4cda-958f-d12e571cb671");
            //_pushNotificationService.SendEventEndNotificationAsync(DateTime.Now, new string[] { "nazev trasy1", "nazev trasy2" }, "1e7ea65d-60be-4cda-958f-d12e571cb671");
            return Ok("Message successfully sent.");
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
            return Ok(await _trafficEventsService.SyncRouteEvents(routeId));
        }

        [HttpPut]
        [Route("api/trafficRoutes/{routeId:int}/events/{eventId:Guid}")]
        public IActionResult RenameRouteEvent(int routeId, string eventId, [FromBody] string name)
        {
            _trafficEventsService.RenameRouteEvent(routeId, eventId, name);
            return Ok();
        }
    }
}