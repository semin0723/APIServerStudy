using APIServerStudy.Controllers;
using APIServerStudy.DAO;
using System.Data;

namespace APIServerStudy.Repository
{
    public interface IDB : IDisposable
    {
        // ### Transaction ###
        public Task<ErrorCode> Transaction(Func<IDbTransaction, Task<ErrorCode>> transactionOperator);
        // ### Transaction End ###

        // ### Auth ###
        public Task<(ErrorCode, GameUser?)> GetUserAccount(string userID, string password);
        public Task<ErrorCode> UserRegister(string userID, string password);
        public Task<ErrorCode> CheckAuthToken(string authToken);

        public Task<ErrorCode> RefreshAuthToken(long uid, string authToken);
        public Task<ErrorCode> RefreshRequestTime(long uid);
        public Task<ErrorCode> Logout(long uid);
        // ### Auth End ###

        // ### DataLoad ###
        public Task<(ErrorCode, List<AttendanceReward>)> LoadAttendanceReward();
        public Task<(ErrorCode, List<UpgradeData>)> LoadUpgradeData();
        public Task<(ErrorCode, UserGoodsData?)> LoadUserGoodsData(long uid);
        public Task<(ErrorCode, List<UserUpgradeData>)> LoadUserUpgradeData(long uid);

        // ### DataLoad End ###

        // ### Attendance ###
        public Task<(ErrorCode, UserAttendanceInfo?)> GetAttendance(long uid);
        public Task<(ErrorCode, AttendanceReward?)> GetAttendanceReward(int attendanceDate);
        public Task<ErrorCode> UpdateAttendance(long uid, int attendanceDate, IDbTransaction transaction);
        // ### Attendance End ###

        // ### Game Data ###
        public Task<(ErrorCode, UpgradeData?)> GetMasterUpgradeData(int upgradeID);
        public Task<(ErrorCode, int)> GetUserUpgradeLevel(long uid, int upgradeID);
        public Task<(ErrorCode, int)> GetUserCredit(long uid);
        public Task<ErrorCode> UpdateUserUpgradeData(long uid, int credit, int upgradeID, int level, bool isUnlock);
        public Task<ErrorCode> UpdateUserCredit(long uid, int credit, IDbTransaction transaction);
       

        // ### Game Data End ###
    }
}
