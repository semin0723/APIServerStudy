namespace APIServerStudy.DTO;

public class CreateAccountRequest
{
    public string userID { get; set; }
    public string password { get; set; }
}

public class CreateAccountResponse
{
    public ErrorCode errorCode { get; set; }
}