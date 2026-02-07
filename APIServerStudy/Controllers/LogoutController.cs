using APIServerStudy.DTO;
using APIServerStudy.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly IUserAuthDB _authDB;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(IUserAuthDB authDB, ILogger<LogoutController> logger)
        {
            _authDB = authDB;
            _logger = logger;
        }

        [HttpPost]
        public async Task Post(LogoutRequest logoutRequest)
        {
            _logger.ZLogInformation($"[Request Logout] uid: {logoutRequest.uid}");

            ErrorCode code = await _authDB.Logout(logoutRequest.uid);

            return;
        }
    }
}
