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
            return Ok("API is running.");
        }

        [HttpGet]
        [Route("download-db")]
        public IActionResult DownloadSQLiteDb()
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "tei.db");

            if (System.IO.File.Exists(dbPath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(dbPath);
                return File(fileBytes, "application/octet-stream", "tei.db");
            }

            return NotFound("Database file not found");
        }
    }
}
