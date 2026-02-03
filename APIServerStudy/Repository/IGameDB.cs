namespace APIServerStudy.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<Tuple<ErrorCode, long>> AuthCheck(string userID, string password);
        public Task<Tuple<ErrorCode, long>> RegistUser(string userID, string password);
    }
}
