using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class InspectionShortModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        public DateTime date {  get; set; }
        [Required]
        public DiagnosisModel diagnosis {  get; set; }
    }
}
