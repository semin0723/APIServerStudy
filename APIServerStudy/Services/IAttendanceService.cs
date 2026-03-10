using APIServerStudy.DTO;

namespace APIServerStudy.Services;

public interface IAttendanceService
{
    public Task<(ErrorCode, AttendanceResponse?)> TryAttendance(long uid, int attendanceDay);
}
