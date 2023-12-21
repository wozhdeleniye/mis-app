using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class InspectionCreateModel
    {
        [Required]
        public DateTime date {  get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string anamnesis { get; set; }
        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string complaints { get; set; }
        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string treatment { get; set; }
        [Required]
        public Conclusion conclusion {  get; set; } 
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        public Guid? previousInspectionId { get; set; }
        [Required]
        [MinLength(1)]
        public List<DiagnosisCreateModel> diagnoses { get; set; }
        public List<ConsultationCreateModel>? consultations { get; set; }
    }
}
