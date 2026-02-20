namespace APIServerStudy.DAO;

public class UpgradeData
{
    public int upgrade_id { get; set; }
    public string upgrade_name { get; set; }
    public string description { get; set; }
    public int max_level { get; set; }
    public int default_level { get; set; }
    public int open_cost { get; set; }
    public int addcost_per_level { get; set; }
    public double addstat_per_level { get; set; }
    public string add_type { get; set; }
}
