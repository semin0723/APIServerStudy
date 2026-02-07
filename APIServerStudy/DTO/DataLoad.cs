using APIServerStudy.DAO;

namespace APIServerStudy.DTO;

public class DataLoadRequest
{
    public long uid {  get; set; }
}

public class DataLoadResponse
{
    UserGoodsData goodsData { get; set; }
}
