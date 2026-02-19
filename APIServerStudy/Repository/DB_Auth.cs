using APIServerStudy.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace APIServerStudy.Repository
{
    public class DbConfig
    {
        public string GameDB { get; set; }
    }

    public partial class DB : IDB
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection? _connection;
        private readonly SqlKata.Compilers.MySqlCompiler _sqlCompiler;
        private readonly QueryFactory _queryFactory;
        private readonly ILogger<DB> _logger;

        public DB(IOptions<DbConfig> dbConfig, ILogger<DB> logger) 
        {
            _logger = logger;
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

        public async Task<(ErrorCode, GameUser?)> GetUserAccount(string userID, string password)
        {
            var userAccountData = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();

            if (userAccountData is null)
            {
                return (ErrorCode.InvalidUserID, null);
            }

            return (ErrorCode.None, userAccountData);
        }

        public async Task<ErrorCode> UserRegister(string userID, string password)
        {
            var userData = await _queryFactory.Query("gamedb.users").Where("id", userID).FirstOrDefaultAsync<GameUser>();
            if (userData != null)
            {
                return ErrorCode.UserAlreadyExists;
            }

            var transaction = _connection.BeginTransaction();

            try
            {
                var newUid = await _queryFactory.Query("gamedb.users").InsertGetIdAsync<long>(new { id = userID, pw = password }, transaction : transaction);

                await _queryFactory.Query("gamedb.user_loginstate").InsertAsync(new { uid = newUid }, transaction: transaction);
                await _queryFactory.Query("gamedb.user_attendance").InsertAsync(new { uid = newUid }, transaction: transaction);
                await _queryFactory.Query("gamedb.user_goods").InsertAsync(new { uid = newUid }, transaction: transaction);

                transaction.Commit();

                return ErrorCode.None;
            }
            catch (Exception ex)
            {
                _logger.ZLogInformation($"[DB ExceptionError] Error: {ex}");
                transaction.Rollback();
                return ErrorCode.RegisterFailed;
            }
        }

        public async Task<ErrorCode> CheckAuthToken(string authToken)
        {
            var userLoginState = await _queryFactory.Query("gamedb.user_loginstate").Where("authToken", authToken).FirstOrDefaultAsync<UserLoginState>();
            if (userLoginState is null)
            {
                return ErrorCode.AuthTokenNotMatch;
            }

            return ErrorCode.None;
        }

        public async Task<ErrorCode> RefreshAuthToken(long uid, string authToken)
        {
            var result = await _queryFactory.Query("gamedb.user_loginstate").
                Where("uid", uid).UpdateAsync(new { authToken = authToken, lastRequestTime = DateTime.Now });

            if (result != 1)
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
