namespace MISBack.Data.Models;

public class IcdRootsRepotModel
{
    public IcdRootsRepotFiltersModel? filters { get; set; }
    
    public List<IcdRootsReportRecordModel>? records { get; set; }
    
    public List<int>? summaryByRoot { get; set; }
}