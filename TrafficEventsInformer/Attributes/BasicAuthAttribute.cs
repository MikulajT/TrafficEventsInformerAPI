using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer.Attributes
{
    public class BasicAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var dopplerService = context.HttpContext.RequestServices.GetRequiredService<IDopplerService>();
            DopplerSecrets commonTICredentials = dopplerService.GetDopplerSecretsAsync().Result;

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Decode Base64 authorization header
            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

            // Extract username and password
            var parts = decodedCredentials.Split(':', 2);
            if (parts.Length != 2 || parts[0] != commonTICredentials.CommonTIBasicAuthUsername || parts[1] != commonTICredentials.CommonTIBasicAuthPassword)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
