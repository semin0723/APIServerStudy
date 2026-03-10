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
    public class LoadUserDataController : ControllerBase
    {
        private readonly IUserDataLoadService _dataLoadService;
        private readonly ILogger<LoadUserDataController> _logger;

        public LoadUserDataController(ILogger<LoadUserDataController> logger, IUserDataLoadService dataLoadService)
        {
            _dataLoadService = dataLoadService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<DataLoadResponse> Post(DataLoadRequest request)
        {
            _logger.ZLogInformation($"[Request UserData] UID: {request.uid}");

            (ErrorCode code, DataLoadResponse? gameUserData) = await _dataLoadService.LoadData(request.uid);
            if(code == ErrorCode.None)
            {
                return gameUserData;
            }

            return new DataLoadResponse { errorCode = code };
        }
    }
}
