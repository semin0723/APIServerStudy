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
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IDB _gameDB;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, IAuthService authService, IDB gameDB)
        {
            _logger = logger;
            _authService = authService;
            _gameDB = gameDB;
        }

        [HttpPost]
        public async Task<LoginResponse> Post(LoginRequest request)
        {
            _logger.ZLogInformation($"[Request Login] ID:{request.userID}, PW:{request.password}");
            var response = new LoginResponse();

            // DB Check Has User and Send Result.
            (ErrorCode errorCode, long uid, string authToken) = await _authService.LoginCheck(request.userID, request.password);

            response.errorCode = errorCode;
            response.uid = uid;
            response.authToken = authToken;

            if (errorCode != ErrorCode.None)
            {
                response.uid = 0;
                return response;
            }

            return response;
        }
    }
}
