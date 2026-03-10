using APIServerStudy.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace APIServerStudy.Repository;

public partial class DB
{    
    public async Task<(ErrorCode, UserAttendanceInfo?)> GetAttendance(long uid)
    {
        var userAttendanceInfo = await _queryFactory.Query("gamedb.user_attendance").Where("uid", uid).FirstOrDefaultAsync<UserAttendanceInfo>();

        if (userAttendanceInfo == null)
        {
            return (ErrorCode.InvalidUserID, null);
        }

        return (ErrorCode.None, userAttendanceInfo);
    }

    public async Task<(ErrorCode, AttendanceReward?)> GetAttendanceReward(int attendanceDate)
    {
        var reward = await _queryFactory.Query("gamedb.master_reward_attendance").Where("attendancedate", attendanceDate).FirstOrDefaultAsync<AttendanceReward>();
        
        if (reward == null)
        {
            return (ErrorCode.InvalidAttendanceDate, null);
        }

        return (ErrorCode.None, reward);
    }

    public async Task<ErrorCode> UpdateAttendance(long uid, int attendanceDate, IDbTransaction transaction)
    {
        var result = await _queryFactory.Query("gamedb.user_attendance").Where("uid", uid).UpdateAsync(new { attendance_count = attendanceDate, last_attendance = DateTime.Now }, transaction: transaction);
        if (result == 0)
        {
            return ErrorCode.UpdateFailed;
        }
        return ErrorCode.None;

    }
}
