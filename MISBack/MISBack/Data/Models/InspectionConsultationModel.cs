using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class InspectionConsultationModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public Guid? inspectionId { get; set; }
        public SpecialityModel? speciality { get; set; }
        public InspectionCommentModel? rootComment { get; set; }
        public int? commentNumber { get; set; }
    }
}
