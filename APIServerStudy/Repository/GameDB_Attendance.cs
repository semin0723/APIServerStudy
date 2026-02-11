using APIServerStudy.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace APIServerStudy.Repository;

public partial class GameDB
{    
    public async Task<ErrorCode> CheckAttendance(long uid)
    {
        var userAttendanceInfo = await _queryFactory.Query("gamedb.user_attendance").Where("uid", uid).FirstOrDefaultAsync<UserAttendanceInfo>();
        if(userAttendanceInfo == null)
        {
            return ErrorCode.InvalidUserID;
        }

        int lastAttendanceMonth = userAttendanceInfo.lastAttendance.Month;
        int lastAttendanceDay = userAttendanceInfo.lastAttendance.Day;

        return ErrorCode.None;
    }
}
