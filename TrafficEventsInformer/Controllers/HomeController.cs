using Microsoft.AspNetCore.Mvc;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
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
