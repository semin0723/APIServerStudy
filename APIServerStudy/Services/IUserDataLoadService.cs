using APIServerStudy.DTO;

namespace APIServerStudy.Services;

public interface IUserDataLoadService
{
    public Task<(ErrorCode, DataLoadResponse?)> LoadData(long uid);
}
