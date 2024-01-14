namespace MISBack.Data.Models;
using MISBack.Data.Enums;
public class IcdRootsReportRecordModel
{
    public string? patientName { get; set; }
    public DateTime? patientBirthdate { get; set; }
    public Gender? gender { get; set; }
    public Dictionary<string, int>? visitsByRoot { get; set; }
}