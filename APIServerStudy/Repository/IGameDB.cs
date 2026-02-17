using APIServerStudy.Controllers;
using APIServerStudy.DAO;

namespace APIServerStudy.Repository
{
    public interface IGameDB : IDisposable
    {
        // ### Auth ###
        public Task<(ErrorCode, GameUser?)> GetUserAccount(string userID, string password);
        public Task<ErrorCode> UserRegister(string userID, string password);
        public Task<ErrorCode> CheckAuthToken(string authToken);

        public Task<ErrorCode> RefreshAuthToken(long uid, string authToken);
        public Task<ErrorCode> RefreshRequestTime(long uid);
        public Task<ErrorCode> Logout(long uid);
        // ### Auth End ###

        // ### Attendance ###
        public Task<(ErrorCode, UserAttendanceInfo?)> GetAttendance(long uid);
        public Task<(ErrorCode, AttendanceReward?)> GetAttendanceReward(int attendanceDate);
        // ### Attendance End ###
    }
}
