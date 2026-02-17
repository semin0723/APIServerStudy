namespace APIServerStudy.Services;

public interface IAuthService
{
    public Task<(ErrorCode, long)> LoginCheck(string userID, string password);
    public Task<ErrorCode> RegisterUser(string userID, string password);
    public Task<ErrorCode> Logout(long uid);
}
