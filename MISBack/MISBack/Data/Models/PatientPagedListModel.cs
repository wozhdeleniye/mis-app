namespace MISBack.Data.Models
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patients { get; set; }
        public PageInfoModel? pagination { get; set; }
    }
}
