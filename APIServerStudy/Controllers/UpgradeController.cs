using APIServerStudy.DTO;
using APIServerStudy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpgradeController : ControllerBase
    {
        private readonly IUpgradeService _upgradeService;
        private readonly ILogger<UpgradeController> _logger;

        public UpgradeController(IUpgradeService upgradeService, ILogger<UpgradeController> logger)
        {
            _upgradeService = upgradeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<UpgradeResponse> Post(UpgradeRequest request)
        {
            _logger.ZLogInformation($"[Request Upgrade] UID: {request.uid}, UpgradeID: {request.upgradeID}");
            (ErrorCode code, var upgradeResult) = await _upgradeService.Upgrade(request.uid, request.upgradeID, request.currentLevel);
            if (code == ErrorCode.None)
            {
                return upgradeResult;
            }
            return new UpgradeResponse { errorCode = code };
        }

        [HttpPost]
        public async Task<UnlockUpgradeResponse> Post(UnlockUpgradeRequest request)
        {
            _logger.ZLogInformation($"[Request Upgrade] UID: {request.uid}, UpgradeID: {request.upgradeID}");
            (ErrorCode code, var unlockResult) = await _upgradeService.UnlockUpgrade(request.uid, request.upgradeID);
            if (code == ErrorCode.None)
            {
                return unlockResult;
            }
            return new UnlockUpgradeResponse { errorCode = code };
        }
    }
}
