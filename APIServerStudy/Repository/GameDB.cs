using APIServerStudy.Controllers;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace APIServerStudy.Repository
{
    public class GameDB : IGameDB
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection? _connection;
        private readonly SqlKata.Compilers.MySqlCompiler _sqlCompiler;
        private readonly QueryFactory _queryFactory;

        public GameDB(IOptions<DbConfig> dbConfig) 
        { 
            _dbConfig = dbConfig;

            _connection = new MySqlConnection(_dbConfig.Value.GameDB);
            _connection?.Open();

            _sqlCompiler = new SqlKata.Compilers.MySqlCompiler();
            _queryFactory = new QueryFactory(_connection, _sqlCompiler);
        }

        public void Dispose()
        {
            _connection?.Close();
        }

        public async Task<Tuple<ErrorCode, long>> AuthCheck(string userID, string password)
        {
            try
            {
                var userInfo = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();

                if(userInfo == null)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.InvalidUserID, 0);
                }

                if(userInfo.pw != password)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.InvalidPassword, 0);
                }

                var userLoginData = await _queryFactory.Query("gamedb.user_loginstate").
                    Where("uid", userInfo.uid).FirstOrDefaultAsync<UserLoginState>();
                if(userLoginData.login == true)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
                }
                var result = await _queryFactory.Query("gamedb.user_loginstate").Where("uid", userInfo.uid).UpdateAsync(new { login = 1, lastlogindate = DateTime.UtcNow });

                return new Tuple<ErrorCode, long>(ErrorCode.None, userInfo.uid);
            }
            catch
            {
                return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
            }
        }

        public async Task<Tuple<ErrorCode, long>> RegistUser(string userID, string password)
        {
            try
            {
                var userExistCheck = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();
                if(userExistCheck != null)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.UserAlreadyExists, 0);
                }

                long? maxUid = _queryFactory.Query("gamedb.users").Max<long?>("uid");
                if(maxUid == null)
                {
                    maxUid = 10000000;
                }

                long newUid = maxUid.Value + 1;

                var result = await _queryFactory.Query("gamedb.users").InsertAsync(new { id = userID, pw = password, uid = newUid });
                if(result != 1)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
                }

                result = await _queryFactory.Query("gamedb.user_loginstate").InsertAsync(new { uid = newUid, login = 0, lastlogindate = DateTime.UtcNow });
                result = await _queryFactory.Query("gamedb.user_stats").InsertAsync(new { uid = newUid });

                return new Tuple<ErrorCode, long>(ErrorCode.RegistSuccess, newUid);
            }
            catch
            {
                return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
            }
        }

        public async Task<Tuple<ErrorCode, bool>> CheckLoginState(long uid)
        {
            try
            {
                var loginState = await _queryFactory.Query("gamedb.user_loginstate").
                    Where("uid", uid).FirstOrDefaultAsync<UserLoginState>();
                if(loginState == null)
                {
                    return new Tuple<ErrorCode, bool>(ErrorCode.InvalidUserID, false);
                }
                return new Tuple<ErrorCode, bool>(ErrorCode.None, loginState.login);
            }
            catch
            {
                return new Tuple<ErrorCode, bool>(ErrorCode.UnKnownError, true);
            }
        }

        public async Task<Tuple<ErrorCode, ResponseGameUserData>> GetGameUserData(long uid)
        {
            try
            {
                var gameUserData = await _queryFactory.Query("gamedb.user_stats").Where("uid", uid).FirstOrDefaultAsync<ResponseGameUserData>();
                if(gameUserData == null)
                {
                    return new Tuple<ErrorCode, ResponseGameUserData>(ErrorCode.InvalidUserID, new ResponseGameUserData());
                }

                return new Tuple<ErrorCode, ResponseGameUserData>(ErrorCode.None, gameUserData);
            }
            catch
            {
                return new Tuple<ErrorCode, ResponseGameUserData>(ErrorCode.UnKnownError, new ResponseGameUserData());
            }
        }

        public async Task<Tuple<ErrorCode, bool>> UserLogout(long uid)
        {
            try
            {
                var result = await _queryFactory.Query("gamedb.user_loginstate").
                    Where("uid", uid).UpdateAsync(new { login = 0, lastlogindate = DateTime.UtcNow });
                if (result != 1)
                {
                    return new Tuple<ErrorCode, bool>(ErrorCode.UnKnownError, false);
                }
                return new Tuple<ErrorCode, bool>(ErrorCode.None, true);
            }
            catch
            {
                return new Tuple<ErrorCode, bool>(ErrorCode.UnKnownError, false);
            }
        }
    }

    public class DbConfig
    {
        public string GameDB { get; set; }
    }
    public class GameUser
    {
        public string id { get; set; }
        public string pw { get; set; }

        public long uid { get; set; }
    }

    public class UserLoginState
    {
        public long uid { get; set; }
        public bool login { get; set; }
        public DateTime lastlogindate { get; set; }
    }

    public class LoginCheckUID
    {
        public long uid { get; set; }
    }
}
