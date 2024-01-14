using MISBack.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Consultation
    {
        [Key]
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid? inspectionId { get; set; }

        [Required]
        public Guid specialityId { get; set; }

        [Required]
        public Guid authorId { get; set; }

        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string comment { get; set; }
    }
}
