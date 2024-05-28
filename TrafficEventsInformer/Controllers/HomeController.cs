using Microsoft.AspNetCore.Mvc;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return Ok("API is running.");
        }
    }
}
