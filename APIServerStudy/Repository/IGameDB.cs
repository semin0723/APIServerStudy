using APIServerStudy.Controllers;

namespace APIServerStudy.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<Tuple<ErrorCode, long>> AuthCheck(string userID, string password);
        public Task<Tuple<ErrorCode, long>> RegistUser(string userID, string password);

        public Task<Tuple<ErrorCode, bool>> CheckLoginState(long uid);
        public Task<Tuple<ErrorCode, ResponseGameUserData>> GetGameUserData(long uid);
    }
}
