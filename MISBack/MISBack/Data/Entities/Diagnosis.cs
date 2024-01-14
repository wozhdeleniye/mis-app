using MISBack.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Diagnosis
    {
        [Key]
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public string? description { get; set; }

        [Required]
        public DiagnosisType type { get; set; }

        [Required]
        public int icd10Id { get; set; }
        [Required]
        public Guid InspectionId {  get; set; }
    }
}
