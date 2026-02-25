using APIServerStudy.DAO;

namespace APIServerStudy.DTO;

public class AttendanceRequest
{
    public long uid { get; set; }
    public int nextAttendanceDay { get; set; }
}

public class AttendanceResponse
{
    public ErrorCode errorCode { get; set; }
    public int attendanceDay { get; set; }
    public int updatedCredit { get; set; }
}