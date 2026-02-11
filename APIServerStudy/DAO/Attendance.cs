namespace APIServerStudy.DAO;

public class UserAttendanceInfo
{
    public DateTime lastAttendance {  get; set; }
    public int attendanceCount { get; set; }
}

public class AttendanceReward
{
    public int rewardID {  get; set; }
    public int rewardAmount { get; set; }
}
