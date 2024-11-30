using Microsoft.AspNetCore.Mvc;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ControllerBase
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return Ok(@"

  _______ ______ _____            _____ _____   _                              _
 |__   __|  ____|_   _|     /\   |  __ \_   _| (_)                            (_)
    | |  | |__    | |      /  \  | |__) || |    _ ___   _ __ _   _ _ __  _ __  _ _ __   __ _
    | |  |  __|   | |     / /\ \ |  ___/ | |   | / __| | '__| | | | '_ \| '_ \| | '_ \ / _` |
    | |  | |____ _| |_   / ____ \| |    _| |_  | \__ \ | |  | |_| | | | | | | | | | | | (_| |
    |_|  |______|_____| /_/    \_\_|   |_____| |_|___/ |_|   \__,_|_| |_|_| |_|_|_| |_|\__, |
                                                                                        __/ |
                                                                                       |___/
            ");
        }
    }
}
