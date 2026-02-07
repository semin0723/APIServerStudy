namespace APIServerStudy.Repository
{
    public interface IUserAuthDB : IDisposable
    {
        public Task<Tuple<ErrorCode, long>> LoginCheck(string userID, string password);
        public Task<ErrorCode> UserRegister(string userID, string password);
        public Task<ErrorCode> CheckAuthToken(string authToken);
        
        public Task<ErrorCode> RefreshAuthToken(long uid, string authToken);
        public Task<ErrorCode> RefreshRequestTime(long uid);
        public Task<ErrorCode> Logout(long uid);
    }
}
