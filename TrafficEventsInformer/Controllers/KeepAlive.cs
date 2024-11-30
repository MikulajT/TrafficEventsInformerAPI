using Microsoft.AspNetCore.Mvc;

namespace TrafficEventsInformer.Controllers
{
    public class KeepAlive : ControllerBase
    {

        /// <summary>
        /// Endpoint is being called by external service to keep app alive on hosting site
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("api/keep-alive")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
