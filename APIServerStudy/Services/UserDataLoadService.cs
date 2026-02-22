using APIServerStudy.DTO;
using APIServerStudy.DAO;
using APIServerStudy.Repository;
using ZLogger;

namespace APIServerStudy.Services;

public class UserDataLoadService : IUserDataLoadService
{
    private readonly IDB _gameDB;
    private readonly ILogger<UserDataLoadService> _logger;

    public UserDataLoadService(IDB gameDB, ILogger<UserDataLoadService> logger)
    {
        _gameDB = gameDB;
        _logger = logger;
    }

    public async Task<(ErrorCode, DataLoadResponse?)> LoadData(long uid)
    {

        (ErrorCode errorCode, var masterAttendanceRewards) = await _gameDB.LoadAttendanceReward();

        (errorCode, var userAttendanceInfo) = await _gameDB.GetAttendance(uid);
        if(userAttendanceInfo == null)
        {
            _logger.ZLogInformation($"[Load Data] UID: {uid}, user attendance data not exist.");
            return (errorCode, null);
        }

        (errorCode, var masterUpgradeData) = await _gameDB.LoadUpgradeData();
        if (userAttendanceInfo == null)
        {
            _logger.ZLogInformation($"[Load Data] UID: {uid}, master upgrade data not exist.");
            return (errorCode, null);
        }

        (errorCode, var userGoodsData) = await _gameDB.LoadUserGoodsData(uid);
        if(userGoodsData == null)
        {
            _logger.ZLogInformation($"[Load Data] UID: {uid}, user goods data not exist.");
            return (errorCode, null); 
        }

        var lastAttendanceDay = userAttendanceInfo.last_attendance.Day;

        bool alreadyAttendance = false;
        if(lastAttendanceDay == DateTime.Now.Day)
        {
            alreadyAttendance = true;
        }

        return (errorCode, new DataLoadResponse
        {
            errorCode = errorCode,
            attendanceRewards = masterAttendanceRewards,
            attendanceCount = userAttendanceInfo.attendance_count,
            upgradeData = masterUpgradeData,
            canAttendance = !alreadyAttendance,
            goodsData = userGoodsData
        });
    }
}
