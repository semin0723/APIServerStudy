namespace APIServerStudy.DTO;

public class UpgradeRequest
{
    public long uid { get; set; }
    public int upgradeID { get; set; }
    public int currentLevel { get; set; }
}

public class UpgradeResponse
{
    public ErrorCode errorCode { get; set; }
    public int currentLevel { get; set; }
    public int currentCredit { get; set; }
}