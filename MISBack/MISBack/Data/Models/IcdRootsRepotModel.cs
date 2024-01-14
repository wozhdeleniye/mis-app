namespace MISBack.Data.Models;

public class IcdRootsRepotModel
{
    public IcdRootsRepotFiltersModel? filters { get; set; }
    
    public List<IcdRootsReportRecordModel>? records { get; set; }
    
    public Dictionary<String, int>? summaryByRoot { get; set; }
}