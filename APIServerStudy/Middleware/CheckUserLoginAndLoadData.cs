using APIServerStudy.Controllers;
using APIServerStudy.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;

namespace APIServerStudy.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CheckUserLoginAndLoadData
    {
        private readonly RequestDelegate _next;
        private readonly IUserAuthDB _authDB;
        private readonly ILogger<CheckUserLoginAndLoadData> _logger;

        public CheckUserLoginAndLoadData(RequestDelegate next, ILogger<CheckUserLoginAndLoadData> logger, IUserAuthDB authDB)
        {
            _next = next;
            _authDB = authDB;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var stringValue = httpContext.Request.Path.Value;
            if(string.Compare(stringValue, "/api/Login", StringComparison.OrdinalIgnoreCase) == 0 || 
                string.Compare(stringValue, "/api/Register", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(stringValue, "/api/Logout", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await _next(httpContext);
                return;
            }

            var (isAuthTokenExist, authToken) = await GetAuthToken(httpContext);
            if(!isAuthTokenExist)
            {
                _logger.ZLogInformation($"[Check AuthToken Exist] AuthToken Not Exist.");
                return;
            }           
            
            _logger.ZLogInformation($"[Check AuthToken Exist] AuthToken: {authToken}");

            if(authToken == null)
            {
                _logger.ZLogInformation($"[Check AuthToken Exist] AuthToken is Null.");
                return;
            }

            ErrorCode code = await _authDB.CheckAuthToken(authToken);
            if(code != ErrorCode.None)
            {
                return;
            }

            await _next(httpContext);
        }

        async Task<(bool, string?)> GetAuthToken(HttpContext httpContext)
        {
            if(httpContext.Request.Headers.TryGetValue("authToken", out var authToken))
            {
                return (true, authToken);
            }

            httpContext.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            var errorJsonResponse = JsonSerializer.Serialize(
                new MiddlewareResponse
                {
                    result = ErrorCode.AuthTokenNotMatch
                });
            await httpContext.Response.WriteAsync(errorJsonResponse);

            return (false, "");
        }

        class MiddlewareResponse
        {
            public ErrorCode result { get; set; }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CheckUserLoginAndLoadDataExtensions
    {
        public static IApplicationBuilder UseCheckUserLoginAndLoadData(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckUserLoginAndLoadData>();
        }
    }
}
