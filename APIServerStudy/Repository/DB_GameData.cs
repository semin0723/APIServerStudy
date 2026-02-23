using APIServerStudy.DAO;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace APIServerStudy.Repository;

public partial class DB
{
    public async Task<(ErrorCode, UpgradeData?)> GetMasterUpgradeData(int upgradeID)
    {
        var upgradeData = await _queryFactory.Query("gamedb.master_upgrade_list").Where("upgrade_id", upgradeID).FirstOrDefaultAsync<UpgradeData>();
        if(upgradeData == null)
        {
            return (ErrorCode.DataNotFound, null);
        }
        return (ErrorCode.None, upgradeData);
    }

    public async Task<(ErrorCode, int)> GetUserUpgradeLevel(long uid, int upgradeID)
    {
        var upgradeLevel = await _queryFactory.Query("gamedb.user_upgrade_states").Where("uid", uid).Where("upgrade_id", upgradeID).FirstOrDefaultAsync<int?>();
        if(upgradeLevel == null)
        {
            return (ErrorCode.DataNotFound, 0);
        }
        return (ErrorCode.None, upgradeLevel.Value);
    }

    public async Task<(ErrorCode, int)> GetUserCredit(long uid)
    {
        var userCredit = await _queryFactory.Query("gamedb.user_goods").Where("uid", uid).FirstOrDefaultAsync<int?>();
        if (userCredit == null)
        {
            return (ErrorCode.DataNotFound, 0);
        }
        return (ErrorCode.None, userCredit.Value);
    }

    public async Task<ErrorCode> UpdateUserUpgradeData(long uid, int credit, int upgradeID, int level, bool isUnlock)
    {
        var transaction = _connection.BeginTransaction();

        try
        {
            await _queryFactory.Query("gamedb.user_goods").
                Where("uid", uid).
                UpdateAsync(new { credit = credit }, transaction: transaction);

            if(isUnlock)
            {
                await _queryFactory.Query("gamedb.user_upgrade_states").
                    InsertAsync(new { uid = uid, upgrade_id = upgradeID, upgrade_level = 1 }, transaction: transaction);
            }
            else
            {
                await _queryFactory.Query("gamedb.user_upgrade_states").
                    Where("uid", uid).
                    Where("upgrade_id", upgradeID).
                    UpdateAsync(new { upgrade_level = level }, transaction: transaction);
            }
            transaction.Commit();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogInformation($"[DB ExceptionError] Error: {ex}");
            transaction.Rollback();
            return ErrorCode.UpdateFailed;
        }
    }

    public async Task<ErrorCode> UpdateUserCredit(long uid, int credit, IDbTransaction transaction)
    {
        var result = await _queryFactory.Query("gamedb.user_goods").
                Where("uid", uid).
                UpdateAsync(new { credit = credit }, transaction: transaction);
        if (result != 1)
        {
            return ErrorCode.UpdateFailed;
        }
        return ErrorCode.None;
    }
}
