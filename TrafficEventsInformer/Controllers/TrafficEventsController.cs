using Microsoft.AspNetCore.Mvc;
using Serilog;
using TrafficEventsInformer.Attributes;
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

        //#if !DEBUG
        //        [ApiExplorerSettings(IgnoreApi = true)]
        //#endif
        [HttpGet]
        [Route("api/trafficRoutes/fcmTest")]
        public IActionResult fcmTest(string routeName, int routeId, string eventId, string userId)
        {
            _pushNotificationService.SendEventStartNotificationAsync(DateTime.Now, routeName, routeId, eventId, userId);
            //_pushNotificationService.SendEventStartNotificationAsync(DateTime.Now, new string[] { "nazev trasy1", "nazev trasy2" }, 5, "ff808181-92d8-0768-0193-4d543be021e7", "g_106729405684925826711");
            //_pushNotificationService.SendEventEndNotificationAsync(DateTime.Now, new string[] { "nazev trasy1", "nazev trasy2" }, "1e7ea65d-60be-4cda-958f-d12e571cb671", "106729405684925826711", Models.AuthProvider.Google);
            return Ok("Message successfully sent.");
        }

        [HttpGet]
        [Route("/api/trafficRoutes/{routeId:int}/events")]
        public IActionResult GetRouteEvents(int routeId)
        {
            return Ok(_trafficEventsService.GetRouteEvents(routeId));
        }

        [HttpGet]
        [Route("api/trafficRoutes/{routeId:int}/events/{eventId:Guid}")]
        public IActionResult GetRouteEventDetail(int routeId, string eventId)
        {
            return Ok(_trafficEventsService.GetRouteEventDetail(routeId, eventId));
        }

        [HttpPut]
        [Route("api/trafficRoutes/{routeId:int}/events/{eventId:Guid}")]
        public IActionResult RenameRouteEvent(int routeId, string eventId, [FromBody] string name)
        {
            _trafficEventsService.RenameRouteEvent(routeId, eventId, name);
            return Ok();
        }

        [HttpPost]
        [Route("api/users/{userId}/trafficRoutes/{routeId:int}/events/sync")]
        public async Task<IActionResult> SyncRouteEvents(string userId, int routeId)
        {
            await _trafficEventsService.SyncRouteEventsAsync(userId, routeId);
            return Ok();
        }

        [HttpPost]
        [Route("api/events/sync")]
        [BasicAuth]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SyncEvents()
        {
            try
            {
                Log.Logger.Information("SyncEvents");

                // Ensure the request is Gzipped
                if (!Request.Headers.ContentEncoding.ToString().Contains("gzip"))
                {
                    return BadRequest("Request is not Gzipped.");
                }

                await _trafficEventsService.SyncEvents(Request.Body);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error occured during sync of events", ex);
                return StatusCode(500, new { message = "Error occured during sync of events" });
            }
        }
    }
}