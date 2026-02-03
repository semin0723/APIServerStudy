using APIServerStudy.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IGameDB _gameDB;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger, IGameDB gameDB)
        {
            _logger = logger;
            _gameDB = gameDB;
        }

        [HttpPost]
        public async Task<RegisterResponse> Post(RegisterRequest request)
        {
            _logger.ZLogInformation($"[Request Regist] ID:{request.UserID}, PW:{request.Password}");

            (ErrorCode errorCode, long uid) = await _gameDB.RegistUser(request.UserID, request.Password); 

            var response = new RegisterResponse();

            response.RegisterResult = (int)errorCode;
            return response;
        }
    }

    public class RegisterRequest
    {
        public string UserID { get; set; }
        public string Password { get; set; }
    }   
    public class RegisterResponse
    {
        public int RegisterResult { get; set; }
    }
}
