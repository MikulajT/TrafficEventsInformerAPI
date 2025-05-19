using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using TrafficEventsInformer.Models;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer
{
    public class FacebookTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IDopplerService _dopplerService;

        public FacebookTokenHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration,
            IDopplerService dopplerService)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
            _dopplerService = dopplerService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorization = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("No Bearer token found");
            }

            var accessToken = authorization["Bearer ".Length..].Trim();

            DopplerSecrets dopplerSecrets = await _dopplerService.GetDopplerSecretsAsync();
            var appId = dopplerSecrets.FacebookAppId;
            var appSecret = dopplerSecrets.FacebookAppSecret;

            // Exchange and debug token
            var httpClient = new HttpClient();
            var validationUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";
            var response = await httpClient.GetStringAsync(validationUrl);

            using var json = JsonDocument.Parse(response);
            var isValid = json.RootElement
                .GetProperty("data")
                .GetProperty("is_valid")
                .GetBoolean();

            if (!isValid)
            {
                return AuthenticateResult.Fail("Invalid Facebook token");
            }

            var userId = json.RootElement.GetProperty("data").GetProperty("user_id").GetString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "facebook_" + userId),
                new Claim("provider", "Facebook")
            };

            var identity = new ClaimsIdentity(claims, nameof(FacebookTokenHandler));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}