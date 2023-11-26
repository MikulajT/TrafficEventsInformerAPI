using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TrafficEventsInformer.Models.UsersRoute;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficEventsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IGeoService _geoService;
        public TrafficEventsController(IConfiguration config, IGeoService geoService)
        {
            _config = config;
            _geoService = geoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrafficEventsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                List<Trkpt> routePoints = _geoService.GetUsersRoute().ToList();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config["CommonTiLogin"]}:{_config["CommonTiPassword"]}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
                var apiUrl = "https://mobilitydata.rsd.cz/Resources/Dynamic/CommonTIDatex/";
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var serializer = new XmlSerializer(typeof(D2LogicalModel));
                    var situations = new List<SituationRecord>();
                    using (StringReader stringReader = new StringReader(content))
                    {
                        var filteredSituations = (D2LogicalModel)serializer.Deserialize(stringReader);
                        situations = ((SituationPublication)filteredSituations.payloadPublication).situation
                            .SelectMany(situation => situation.situationRecord)
                            .Where(record => record.validity.validityTimeSpecification.overallEndTime > DateTime.Now)
                            .ToList();
                    }
                    var routeSituations = _geoService.GetEventsOnUsersRoute(routePoints, situations);
                    return Ok(routeSituations);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}