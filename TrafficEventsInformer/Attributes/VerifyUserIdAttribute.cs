using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TrafficEventsInformer.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class VerifyUserIdAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _userIdRouteParameterName;

        public VerifyUserIdAttribute(string userIdRouteParameterName = "userId")
        {
            _userIdRouteParameterName = userIdRouteParameterName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Read user ID from claims (the 'sub' claim from the JWT token)
            var authenticatedUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                      user.FindFirst("sub")?.Value;

            if (authenticatedUserId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Extract id in case of Facebook
            if (authenticatedUserId.StartsWith("facebook"))
            {
                authenticatedUserId = authenticatedUserId.Split('_')[1];
            }

            // Read userId from route parameters
            if (context.RouteData.Values.TryGetValue(_userIdRouteParameterName, out var routeUserIdObj))
            {
                var routeUserId = routeUserIdObj?.ToString();
                string userId = routeUserId?.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[1];

                if (userId != authenticatedUserId)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult($"Missing route parameter '{_userIdRouteParameterName}'.");
                return;
            }

            await next(); // Authorization successful, continue to the action
        }
    }
}
