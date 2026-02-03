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

                return new Tuple<ErrorCode, long>(ErrorCode.None, 0);
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

                var result = await _queryFactory.Query("gamedb.users").InsertAsync(new { id = userID, pw = password });
                if(result != 1)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
                }
                return new Tuple<ErrorCode, long>(ErrorCode.RegistSuccess, 0);
            }
            catch
            {
                return new Tuple<ErrorCode, long>(ErrorCode.UnKnownError, 0);
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
    }
}
