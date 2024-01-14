using MISBack.Data.Entities;

namespace MISBack.Data.Models
{
    public class LastInspectionSortingModel
    {
        public Patient patient { get; set; }
        public DateTime lastInspection {  get; set; }
    }
}
