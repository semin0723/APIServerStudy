using APIServerStudy.DTO;

namespace APIServerStudy.Services;

public interface IUpgradeService
{
    public Task<(ErrorCode, UpgradeResponse?)> Upgrade(long uid, int upgradeID, int currentLevel);
    public Task<(ErrorCode, UnlockUpgradeResponse?)> UnlockUpgrade(long uid, int upgradeID);
}
