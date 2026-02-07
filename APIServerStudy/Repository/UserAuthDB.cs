using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace APIServerStudy.Repository
{
    public class UserAuthDB : IUserAuthDB
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection? _connection;
        private readonly SqlKata.Compilers.MySqlCompiler _sqlCompiler;
        private readonly QueryFactory _queryFactory;

        public UserAuthDB(IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;

            _connection = new MySqlConnection(_dbConfig.Value.GameDB);
            _connection?.Open();

            _sqlCompiler = new SqlKata.Compilers.MySqlCompiler();
            _queryFactory = new QueryFactory(_connection, _sqlCompiler);
        }

        public void Dispose() 
        {
            _connection?.Dispose();
        }
        
        public async Task<Tuple<ErrorCode, long>> LoginCheck(string userID, string password)
        {
            var userAccountData = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();
            if (userAccountData is null)
            {
                return new Tuple<ErrorCode, long>(ErrorCode.InvalidUserID, 0);
            }

            if(password != userAccountData.pw)
            {
                return new Tuple<ErrorCode, long>(ErrorCode.InvalidPassword, 0);
            }

            return new Tuple<ErrorCode, long>(ErrorCode.None, userAccountData.uid);
        }

        public async Task<ErrorCode> UserRegister(string userID, string password)
        {
            var userAccountData = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();
            if (userAccountData is not null)
            {
                return ErrorCode.UserAlreadyExists;
            }

            long? maxUid = _queryFactory.Query("gamedb.users").Max<long?>("uid");
            if (maxUid == null)
            {
                maxUid = 10000000;
            }
            long newUid = maxUid.Value + 1;

            var result = await _queryFactory.Query("gamedb.users").InsertAsync(new { id = userID, pw = password, uid = newUid });
            if(result != 1)
            {
                return ErrorCode.RegisterFailed;
            }

            result = await _queryFactory.Query("gamedb.user_loginstate").InsertAsync(new {uid = newUid});
            if (result != 1)
            {
                return ErrorCode.RegisterFailed;
            }

            result = await _queryFactory.Query("gamedb.user_goods").InsertAsync(new { uid = newUid });
            if(result != 1)
            {
                return ErrorCode.RegisterFailed; 
            }

            return ErrorCode.None;
        }

        public async Task<ErrorCode> CheckAuthToken(string authToken)
        {
            var userLoginState = await _queryFactory.Query("gamedb.user_loginstate").Where("authToken", authToken).FirstOrDefaultAsync<UserLoginState>();
            if(userLoginState is null)
            {
                return ErrorCode.AuthTokenNotMatch;
            }

            return ErrorCode.None;
        }

        public async Task<ErrorCode> RefreshAuthToken(long uid, string authToken)
        {
            var result = await _queryFactory.Query("gamedb.user_loginstate").
                Where("uid", uid).UpdateAsync(new {authToken = authToken, lastRequestTime = DateTime.Now});

            if(result != 1)
            {
                return ErrorCode.InvalidUserID;
            }

            return ErrorCode.None;
        }

        public async Task<ErrorCode> RefreshRequestTime(long uid)
        {
            var result = await _queryFactory.Query("gamedb.user_loginstate").
                Where("uid", uid).UpdateAsync(new { lastRequestTime = DateTime.Now });

            if (result != 1)
            {
                return ErrorCode.InvalidUserID;
            }

            return ErrorCode.None;
        }

        public async Task<ErrorCode> Logout(long uid)
        {
            var result = await _queryFactory.Query("gamedb.user_loginstate").
                Where("uid", uid).UpdateAsync(new { authToken = "", lastRequestTime = DateTime.Now });
            if (result != 1)
            {
                return ErrorCode.InvalidUserID;
            }
            return ErrorCode.None;
        }
    }
}
