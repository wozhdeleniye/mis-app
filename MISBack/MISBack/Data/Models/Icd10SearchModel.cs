namespace MISBack.Data.Models;

public class Icd10SearchModel
{
    public List<Icd10RecordModel>? records { get; set; }
    
    public PageInfoModel? pagination { get; set; }
}