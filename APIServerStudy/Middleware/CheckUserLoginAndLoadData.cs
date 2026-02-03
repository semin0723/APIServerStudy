using APIServerStudy.Controllers;
using APIServerStudy.Repository;
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
        private readonly IGameDB _gamedb;
        private readonly ILogger<CheckUserLoginAndLoadData> _logger;

        public CheckUserLoginAndLoadData(RequestDelegate next, ILogger<CheckUserLoginAndLoadData> logger, IGameDB gameDB)
        {
            _next = next;
            _gamedb = gameDB;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var stringValue = httpContext.Request.Path.Value;
            if(string.Compare(stringValue, "/api/Login", StringComparison.OrdinalIgnoreCase) == 0 || 
                string.Compare(stringValue, "/api/Register", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await _next(httpContext);
                return;
            }

            var (isUIDExist, uidString) = await IsUIDExist(httpContext);
            if(!isUIDExist)
            {
                _logger.ZLogInformation($"[Search UserData] UID: {uidString} Not Exist.");
                return;
            }

            long uid = long.Parse(uidString);
            _logger.ZLogInformation($"[Search UserData] UID: {uid} Search Success.");

            (ErrorCode code, bool result) = await _gamedb.CheckLoginState(uid);
            if(code != ErrorCode.None)
            {
                return;
            }

            await _next(httpContext);
        }

        async Task<(bool, string)> IsUIDExist(HttpContext httpContext)
        {
            if(httpContext.Request.Headers.TryGetValue("uid", out var uid))
            {
                return (true, uid);
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errorJsonResponse = JsonSerializer.Serialize(
                new MiddlewareResponse
                {
                    result = ErrorCode.UnKnownError
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
