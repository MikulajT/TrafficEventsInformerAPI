using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficEventsInformer.Attributes;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Controllers
{
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        [Route("/api/users")]
        public IActionResult AddUser([FromBody] AddUserRequestDto requestBody)
        {
            _usersService.AddUser(requestBody);
            return Ok();
        }

        [HttpPost]
        [Route("/api/users/{userId}/fcm-tokens")]
        [VerifyUserId]
        public IActionResult AddFcmDeviceToken([FromRoute] string userId, [FromBody] string fcmDeviceToken)
        {
            _usersService.AddFcmDeviceToken(userId, fcmDeviceToken);
            return Ok();
        }

        [HttpHead]
        [Route("/api/users/{userId}/fcm-tokens")]
        [VerifyUserId]
        public IActionResult UserHasToken([FromRoute] string userId, [FromBody] string fcmDeviceToken)
        {
            bool tokenExists = _usersService.UserHasToken(userId, fcmDeviceToken);

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