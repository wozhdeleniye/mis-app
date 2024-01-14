namespace MISBack.Data.Models
{
    public class SpecialtiesPagedListModel
    {
        public List<SpecialityModel>? specialities { get; set; }
        public PageInfoModel? pagination { get; set; }
    }
}
