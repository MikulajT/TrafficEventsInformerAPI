using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficEventsController : ControllerBase
    {
        private readonly ITrafficRouteService _trafficRouteService;

        public TrafficEventsController(ITrafficRouteService trafficRouteService)
        {
            _trafficRouteService = trafficRouteService;
        }

        [HttpPost]
        public async Task<IActionResult> SyncUsersRouteEvents()
        {
            await _trafficRouteService.SyncUsersRouteEvents();
            return Ok();
        }
    }
}