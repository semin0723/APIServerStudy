using APIServerStudy.DTO;
using APIServerStudy.Repository;
using APIServerStudy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(IAuthService authService, ILogger<LogoutController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        public async Task Post(LogoutRequest logoutRequest)
        {
            _logger.ZLogInformation($"[Request Logout] uid: {logoutRequest.uid}");

            ErrorCode code = await _authService.Logout(logoutRequest.uid);

            return;
        }
    }
}
