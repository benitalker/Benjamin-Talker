using AgentsApi.Service;
using System.Globalization;

namespace AgentsApi.Middlewares
{
    public class AuthenticationMiddleware(RequestDelegate next)
    { 

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var request = context.Request;
            var idk = request.Headers.Authorization;
            if (idk.Any())
            {
        
            }
            // Call the next delegate/middleware in the pipeline.
            await next(context);
        }
    }
}
