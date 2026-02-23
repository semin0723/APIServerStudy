using APIServerStudy.DAO;
using APIServerStudy.DTO;
using APIServerStudy.Repository;
using ZLogger;

namespace APIServerStudy.Services;

public class UpgradeService : IUpgradeService
{
    private readonly IDB _gameDB;
    private readonly ILogger<UpgradeService> _logger;

    public UpgradeService(IDB gameDB, ILogger<UpgradeService> logger)
    {
        _gameDB = gameDB;
        _logger = logger;
    }

    public async Task<(ErrorCode, UpgradeResponse?)> Upgrade(long uid, int upgradeID, int currentLevel)
    {
        (ErrorCode errorCode, var upgradeInfo) = await _gameDB.GetMasterUpgradeData(upgradeID);
        if(errorCode != ErrorCode.None)
        {
            _logger.ZLogError($"Failed to get upgrade data for upgradeID: {upgradeID}, currentLevel: {currentLevel}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var userCredit) = await _gameDB.GetUserCredit(uid);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogError($"Failed to get user credit for uid: {uid}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var upgradeLevel) = await _gameDB.GetUserUpgradeLevel(uid, upgradeID);
        if (errorCode == ErrorCode.DataNotFound)
        {
            if(upgradeLevel != currentLevel)
            {
                _logger.ZLogError($"Failed to upgrade for uid: {uid}, upgradeID: {upgradeID}. ErrorCode: {ErrorCode.UpgradeLevelMismatch}");
                return (ErrorCode.UpgradeLevelMismatch, null);
            }

            int requiredCredit = upgradeInfo.open_cost;
            if(requiredCredit > userCredit)
            {
                _logger.ZLogWarning($"Not enough credit for uid: {uid} to open upgradeID: {upgradeID}. Required: {requiredCredit}, Available: {userCredit}");
                return (ErrorCode.NotEnoughCredit, null);
            }

            int newCredit = userCredit - requiredCredit;
            errorCode = await _gameDB.UpdateUserUpgradeData(uid, newCredit, upgradeID, 1, true);

            if (errorCode != ErrorCode.None)
            {
                _logger.ZLogError($"Failed to update user data for uid: {uid} after upgrade. ErrorCode: {errorCode}");
                return (errorCode, null);
            }

            return (ErrorCode.None, new UpgradeResponse
            {
                errorCode = ErrorCode.None,
                currentLevel = 1,
                currentCredit = newCredit
            });
        }
        else
        {
            int requiredCredit = upgradeInfo.addcost_per_level * currentLevel;
            if (requiredCredit > userCredit)
            {
                _logger.ZLogWarning($"Not enough credit for uid: {uid} to open upgradeID: {upgradeID}. Required: {requiredCredit}, Available: {userCredit}");
                return (ErrorCode.NotEnoughCredit, null);
            }

            int newCredit = userCredit - requiredCredit;
            errorCode = await _gameDB.UpdateUserUpgradeData(uid, newCredit, upgradeID, currentLevel + 1, false);
            if (errorCode != ErrorCode.None)
            {
                _logger.ZLogError($"Failed to update user data for uid: {uid} after upgrade. ErrorCode: {errorCode}");
                return (errorCode, null);
            }

            return (ErrorCode.None, new UpgradeResponse
            {
                errorCode = ErrorCode.None,
                currentLevel = currentLevel + 1,
                currentCredit = newCredit
            });
        }
    }
}
