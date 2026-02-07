using APIServerStudy.DTO;
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
        private readonly IUserAuthDB _authDB;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, IUserAuthDB authDB)
        {
            _logger = logger;
            _authDB = authDB;
        }

        [HttpPost]
        public async Task<LoginResponse> Post(LoginRequest request)
        {
            _logger.ZLogInformation($"[Request Login] ID:{request.userID}, PW:{request.password}");
            var response = new LoginResponse();

            // DB Check Has User and Send Result.
            (ErrorCode errorCode, long uid) = await _authDB.LoginCheck(request.userID, request.password);

            response.errorCode = errorCode;

            if (errorCode != ErrorCode.None)
            {
                response.uid = 0;
                return response;
            }

            string authToken = CreateToken();
            response.authToken = authToken;
            response.uid = uid;

            errorCode = await _authDB.RefreshAuthToken(uid, authToken);

            return response;
        }

        private string tokenElement = "0123456789abcdefghijklmnopqrstuvwxyz";
        string CreateToken()
        {
            byte[] tokenByte = new byte[16];
            Random random = new Random();
            random.NextBytes(tokenByte);

            string authToken = "";

            foreach (byte b in tokenByte)
            {
                int index = b % tokenElement.Length;
                authToken += tokenElement[index];
            }

            return authToken;
        }
    }
}
