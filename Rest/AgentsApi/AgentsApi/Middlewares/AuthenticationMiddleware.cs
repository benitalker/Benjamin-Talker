using AgentsApi.Service;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NuGet.Protocol;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AgentsApi.Middlewares
{
    public class AuthenticationMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var request = context.Request.Headers.Authorization;
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                var a = headerValue.FirstOrDefault()?.Split(" ")[1] ?? string.Empty;
                var res = jwtService.ValidateToken(a);
                if (res)
                {
                   await next(context);
                } else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            } else
            {
                await next(context);
            }
        }
    }
}
