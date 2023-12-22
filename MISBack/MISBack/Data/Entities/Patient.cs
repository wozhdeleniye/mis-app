using MISBack.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Patient
    {
        [Key]
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        [Required]
        public Gender gender { get; set; }
    }
}
