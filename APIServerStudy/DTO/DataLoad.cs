using APIServerStudy.DAO;

namespace APIServerStudy.DTO;

public class DataLoadRequest
{
    public long uid {  get; set; }
}

public class DataLoadResponse
{
    public ErrorCode errorCode { get; set; }
    public bool canAttendance { get; set; }
    public List<AttendanceReward> attendanceRewards { get; set; }
    public UserGoodsData goodsData { get; set; }
}
