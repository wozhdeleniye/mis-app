using MISBack.Data.Enums;
using MISBack.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Inspection
    {
        [Key]
        [Required]
        public Guid id {  get; set; }
        [Required]
        public Guid docId { get; set; }
        [Required]
        public Guid patientId { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public string anamnesis { get; set; }
        [Required]
        public string complaints { get; set; }
        [Required]
        public string treatment { get; set; }
        [Required]
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        public Guid baseInspectionId { get; set; }
        public Guid? previousInspectionId { get; set; }
    }
}
