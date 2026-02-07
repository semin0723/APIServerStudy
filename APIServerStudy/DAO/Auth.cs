namespace APIServerStudy.DAO;

public class GameUser
{
    public string id { get; set; }
    public string pw { get; set; }
    public long uid { get; set; }
}

public class UserLoginState
{
    public long uid { get; set; }
    public string authToken { get; set; }
    public DateTime lastRequestTime { get; set; }
}


