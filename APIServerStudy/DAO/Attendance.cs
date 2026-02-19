namespace APIServerStudy.DAO;

public class UserAttendanceInfo
{
    public DateTime last_attendance {  get; set; }
    public int attendance_count { get; set; }
}

public class AttendanceReward
{
    public int reward_id {  get; set; }
    public int reward_amount { get; set; }
}