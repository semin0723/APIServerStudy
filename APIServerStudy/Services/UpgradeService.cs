using APIServerStudy.DAO;
using APIServerStudy.DTO;
using APIServerStudy.Repository;

namespace APIServerStudy.Services;

public class UpgradeService
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
        int nextLevel = currentLevel + 1;

        (ErrorCode errorCode, var upgradeInfo) = await _gameDB.GetUpgradeData(upgradeID);
        if(errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to get upgrade data for upgradeID: {upgradeID}, currentLevel: {currentLevel}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var userUpgradeInfo) = await _gameDB.GetUserUpgradeLevel(uid, upgradeID);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to get user upgrade level for uid: {uid}, upgradeID: {upgradeID}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var userCredit) = await _gameDB.GetUserCredit(uid);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to get user credit for uid: {uid}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        int consumedCredit = upgradeInfo.addcost_per_level * nextLevel;
        if (consumedCredit > userCredit)
        {
            return (ErrorCode.NotEnoughCredit, null);
        }

        int newCredit = userCredit - consumedCredit;
        errorCode = await _gameDB.UpdateUserData(uid, newCredit, upgradeID, nextLevel, false);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to update user data for uid: {uid} after upgrade. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        return (ErrorCode.None, new UpgradeResponse
        {
            errorCode = ErrorCode.None,
            currentLevel = nextLevel,
            currentCredit = newCredit
        });
        
    }

    public async Task<(ErrorCode, UnlockUpgradeResponse?)> UnlockUpgrade(long uid, int upgradeID)
    {
        (ErrorCode errorCode, var upgradeInfo) = await _gameDB.GetUpgradeData(upgradeID);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to get upgrade data for upgradeID: {upgradeID}, currentLevel: 1. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var userUpgradeInfo) = await _gameDB.GetUserUpgradeLevel(uid, upgradeID);
        if (errorCode == ErrorCode.None)
        {
            _logger.LogError($"Upgrade is already unlocked. Invalid upgrade id: {uid}, upgradeID: {upgradeID}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        (errorCode, var userCredit) = await _gameDB.GetUserCredit(uid);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to get user credit for uid: {uid}. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        int consumedCredit = upgradeInfo.open_cost;
        if (consumedCredit > userCredit)
        {
            return (ErrorCode.NotEnoughCredit, null);
        }

        int newCredit = userCredit - consumedCredit;
        errorCode = await _gameDB.UpdateUserData(uid, newCredit, upgradeID, 1, false);
        if (errorCode != ErrorCode.None)
        {
            _logger.LogError($"Failed to update user data for uid: {uid} after upgrade. ErrorCode: {errorCode}");
            return (errorCode, null);
        }

        return (ErrorCode.None, new UnlockUpgradeResponse
        {
            errorCode = ErrorCode.None,
            currentCredit = newCredit
        });
    }
}
