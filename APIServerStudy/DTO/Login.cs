namespace APIServerStudy.DTO;

public class LoginRequest
{
    public string userID { get; set; }
    public string password { get; set; }
}

public class LoginResponse
{
    public ErrorCode errorCode { get; set; }
    public long uid { get; set; }
    public string authToken { get; set; }
}
