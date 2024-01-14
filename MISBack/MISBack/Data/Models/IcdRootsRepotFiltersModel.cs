namespace MISBack.Data.Models;

public class IcdRootsRepotFiltersModel
{
    public DateTime? start { get; set; }
    public DateTime? end { get; set; }
    public List<int>? icdRoots { get; set; }
}