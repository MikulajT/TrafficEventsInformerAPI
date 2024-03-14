using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Models.Fcm;
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
        public async Task<IActionResult> fcmTest()
        {
            bool success = await _pushNotificationService.SendPushNotificationAsync(new PushNotificationDto()
            {
                Body = "body",
                Title = "title",
                DeviceToken = "dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS"
            });
            if (success)
            {
                return Ok("Message successfully sent.");
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
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
    }
}