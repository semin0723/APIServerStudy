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
        private readonly IGameDB _gamedb;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(IGameDB gamedb, ILogger<LogoutController> logger)
        {
            _gamedb = gamedb;
            _logger = logger;
        }

        [HttpPost]
        public async Task Post([FromHeader] RequestHeader requestHeader)
        {
            long uid = long.Parse(requestHeader.uid);
            _logger.ZLogInformation($"[Request Logout] uid: {uid}");

            (ErrorCode code, bool result) = await _gamedb.UserLogout(uid);

            return;
        }
    }
}
