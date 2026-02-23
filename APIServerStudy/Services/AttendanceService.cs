using APIServerStudy.DTO;
using APIServerStudy.Repository;
using ZLogger;

namespace APIServerStudy.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IDB _gameDB;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(IDB gameDB, ILogger<AttendanceService> logger)
    {
        _gameDB = gameDB;
        _logger = logger;
    }

    public async Task<(ErrorCode, AttendanceResponse?)> TryAttendance(long uid, int attendanceDay)
    {
        (ErrorCode errorCode, var attendanceInfo) = await _gameDB.GetAttendance(uid);
        if(attendanceInfo == null)
        {
            _logger.ZLogInformation($"[Attendance] Error: {errorCode.ToString()}, UID: {uid}");
            return (errorCode, null);
        }

        if(attendanceInfo.attendance_count + 1 != attendanceDay)
        {
            _logger.ZLogInformation($"[Attendance] Error: Invalid Attendance Day, UID: {uid}, AttendanceDay: {attendanceInfo.attendance_count}, RequestAttendanceDay: {attendanceDay}");
            return (ErrorCode.InvalidAttendanceDate, null);
        }

        var today = DateTime.Now;
        if(attendanceInfo.last_attendance.Date == today.Date)
        {
            _logger.ZLogInformation($"[Attendance] Error: Already Attended Today, UID: {uid}, LastAttendance: {attendanceInfo.last_attendance}");
            return (ErrorCode.InvalidAttendanceDate, null);
        }

        (errorCode, var reward) = await _gameDB.GetAttendanceReward(attendanceDay);
        if(reward == null)
        {
            _logger.ZLogInformation($"[Attendance] Error: Attendance Reward Not Found, UID: {uid}, AttendanceDay: {attendanceDay}");
            return (ErrorCode.DataNotFound, null);
        }

        (errorCode) = await _gameDB.UpdateAttendance(uid, attendanceDay);
        if(errorCode != ErrorCode.None)
        {
            _logger.ZLogInformation($"[Attendance] Error: Failed to Update Attendance Info, UID: {uid}, AttendanceDay: {attendanceDay}");
            return (errorCode, null);
        }

        _logger.ZLogInformation($"[Attendance] Success, UID: {uid}, AttendanceDay: {attendanceDay}, Reward: {reward}");
        return (ErrorCode.None, new AttendanceResponse
        {
            errorCode = ErrorCode.None,
            attendanceDay = attendanceDay,
            reward = reward
        });
    }
}
