using APIServerStudy.DTO;
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
        private readonly IUserAuthDB _authDB;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger, IUserAuthDB authDB)
        {
            _logger = logger;
            _authDB = authDB;
        }

        [HttpPost]
        public async Task<CreateAccountResponse> Post(CreateAccountRequest request)
        {
            _logger.ZLogInformation($"[Request Regist] ID:{request.userID}, PW:{request.password}");

            ErrorCode errorCode = await _authDB.UserRegister(request.userID, request.password); 

            var response = new CreateAccountResponse();

            response.errorCode = errorCode;
            return response;
        }
    }
}
