using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        [Route("/api/users/{userId}/fcm-tokens")]
        public IActionResult AddFcmDeviceToken([FromRoute] string userId, [FromBody] string fcmDeviceToken)
        {
            ServiceResult serviceResult = _usersService.AddFcmDeviceToken(userId, fcmDeviceToken);

            if (serviceResult == ServiceResult.Success)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
        }

        [HttpHead]
        [Route("/api/users/{userId}/fcm-tokens/{fcmDeviceToken}")]
        public IActionResult FcmDeviceTokenExists([FromRoute] string userId, [FromRoute] string fcmDeviceToken)
        {
            bool tokenExists = _usersService.FcmDeviceTokenExists(userId, fcmDeviceToken);

            if (tokenExists)
            {
                return Ok();

            }
            else
            {
                return NotFound();
            }
        }
    }
}