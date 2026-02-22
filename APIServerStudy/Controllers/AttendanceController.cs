using APIServerStudy.DTO;
using APIServerStudy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<AttendanceResponse> Post(AttendanceRequest request)
        {
            _logger.ZLogInformation($"[Attendance] UID: {request.uid}");
            (ErrorCode code, var reward) = await _attendanceService.TryAttendance(request.uid, request.nextAttendanceDay);
            if (code == ErrorCode.None)
            {
                return reward;
            }
            return new AttendanceResponse { errorCode = code };
        }
    }
}
