using APIServerStudy.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServerStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadUserDataController : ControllerBase
    {
        private readonly IGameDB _gameDB;
        private readonly ILogger<LoadUserDataController> _logger;

        public LoadUserDataController(ILogger<LoadUserDataController> logger, IGameDB gameDB)
        {
            _gameDB = gameDB;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ResponseGameUserData> Post([FromHeader] RequestHeader header)
        {
            long uid = long.Parse(header.uid);

            _logger.ZLogInformation($"[Request UserData] UID: {uid}");

            (ErrorCode code, ResponseGameUserData gameUserData) = await _gameDB.GetGameUserData(uid);
            if(code == ErrorCode.None)
            {
                return gameUserData;
            }

            return new ResponseGameUserData();
        }
    }

    public class RequestHeader
    {
        [FromHeader]
        public string uid { get; set; }
    }

    public class ResponseGameUserData
    {
        public long uid {  get; set; }
        public int maxHp { get; set; }
        public int maxMp { get; set; }
        public int attackPower { get; set; }
        public int defence { get; set; }
    }
}
