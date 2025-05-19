namespace TrafficEventsInformer
{
    public class AuthSchemeSelectorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthSchemeSelectorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? userId = context.Request.RouteValues["userId"]?.ToString()
                ?? context.Request.Query["userId"].ToString()
                ?? GetUserIdFromBody(context);

            if (!string.IsNullOrEmpty(userId))
            {
                if (userId.StartsWith("f_"))
                {
                    context.Items["auth_scheme"] = "Facebook";
                }
                else if (userId.StartsWith("g_"))
                {
                    context.Items["auth_scheme"] = "Google";
                }
            }

            await _next(context);
        }

        private string? GetUserIdFromBody(HttpContext context)
        {
            // Optional: only needed if you pass userId in JSON body
            // You can implement reading JSON body here (requires buffering the request body)
            return null;
        }
    }
}
