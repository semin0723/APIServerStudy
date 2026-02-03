using APIServerStudy.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IGameDB _gameDB;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, IGameDB gameDB)
        {
            _logger = logger;
            _gameDB = gameDB;
        }

        [HttpPost]
        public async Task<LoginResponse> Post(LoginRequest request)
        {
            _logger.ZLogInformation($"[Request Login] ID:{request.UserID}, PW:{request.Password}");
            var response = new LoginResponse();

            // DB Check Has User and Send Result.
            (ErrorCode errorCode, long uid) = await _gameDB.AuthCheck(request.UserID, request.Password);
            if(errorCode != 0)
            {
                response.LoginResult = (int)errorCode;
                return response;
            }

            return response;
        }
    }

    public class LoginRequest
    {
        public string UserID { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public int LoginResult { get; set; }
    }
}
