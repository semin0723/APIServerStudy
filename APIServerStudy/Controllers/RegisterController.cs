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
    public class RegisterController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost]
        public async Task<CreateAccountResponse> Post(CreateAccountRequest request)
        {
            _logger.ZLogInformation($"[Request Regist] ID:{request.userID}, PW:{request.password}");

            ErrorCode errorCode = await _authService.RegisterUser(request.userID, request.password); 

            var response = new CreateAccountResponse();

            response.errorCode = errorCode;
            return response;
        }
    }
}
