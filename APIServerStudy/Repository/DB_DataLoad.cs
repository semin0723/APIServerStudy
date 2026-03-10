using APIServerStudy.DAO;
using SqlKata.Execution;

namespace APIServerStudy.Repository;

public partial class DB
{
    public async Task<(ErrorCode, List<AttendanceReward>)> LoadAttendanceReward()
    {
        var attendanceRewards = await _queryFactory.Query("gamedb.master_reward_attendance").GetAsync<AttendanceReward>();
        List<AttendanceReward> rewards = attendanceRewards.ToList();
        return (ErrorCode.None, rewards);
    }

    public async Task<(ErrorCode, List<UpgradeData>)> LoadUpgradeData()
    {
        var upgradeDataList = await _queryFactory.Query("gamedb.master_upgrade_list").GetAsync<UpgradeData>();
        List<UpgradeData> upgrades = upgradeDataList.ToList();
        return (ErrorCode.None, upgrades);
    }

    public async Task<(ErrorCode, List<UserUpgradeData>)> LoadUserUpgradeData(long uid)
    {
        var userUpgradeData = await _queryFactory.Query("gamedb.user_upgrade_states").Where("uid", uid).GetAsync<UserUpgradeData>();
        List<UserUpgradeData> upgrades = userUpgradeData.ToList();
        return (ErrorCode.None, upgrades);
    }

    public async Task<(ErrorCode, UserGoodsData?)> LoadUserGoodsData(long uid)
    {
        var userGoodsData = await _queryFactory.Query("gamedb.user_goods").Where("uid", uid).FirstOrDefaultAsync<UserGoodsData>();
        if (userGoodsData == null)
        {
            return (ErrorCode.InvalidUserID, null);
        }

        return (ErrorCode.None, userGoodsData);
    }
}
