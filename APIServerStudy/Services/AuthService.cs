using APIServerStudy.DAO;
using APIServerStudy.Repository;
using System.Security.Cryptography;
using ZLogger;

namespace APIServerStudy.Services;

public class AuthService : IAuthService
{
    private readonly IDB _gameDB;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IDB gameDB, ILogger<AuthService> logger)
    {
        _gameDB = gameDB;
        _logger = logger;
    }

    public async Task<(ErrorCode, long, string)> LoginCheck(string userID, string password)
    {
        (ErrorCode errorCode, GameUser? userAccount) = await _gameDB.GetUserAccount(userID, password);

        if(userAccount == null)
        {
            _logger.ZLogInformation($"[Request Login] Error:{errorCode.ToString()}");
            return (errorCode, 0, "");
        }

        if(password != userAccount.pw)
        {
            _logger.ZLogInformation($"[Request Login] Error: Invalid Password, UID:{userAccount.uid}");
            return (ErrorCode.InvalidPassword, 0, "");
        }

        string authToken = Guid.NewGuid().ToString();
        _logger.ZLogInformation($"[Request Login] Error: Login Success, UID: {userAccount.uid}, AuthToken: {authToken}");

        var result = await _gameDB.RefreshAuthToken(userAccount.uid, authToken);

        return (ErrorCode.None, userAccount.uid, authToken);
    }

    public async Task<ErrorCode> RegisterUser(string userID, string password)
    {
        return await _gameDB.UserRegister(userID, password);
    }

    public async Task<ErrorCode> Logout(long uid)
    {
        return await _gameDB.Logout(uid);
    }


    private string tokenElement = "0123456789abcdefghijklmnopqrstuvwxyz";
    string CreateToken()
    {
        byte[] tokenByte = new byte[16];
        Random random = new Random();
        random.NextBytes(tokenByte);

        string authToken = "";

        foreach (byte b in tokenByte)
        {
            int index = b % tokenElement.Length;
            authToken += tokenElement[index];
        }

        return authToken;
    }
}
