using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Speciality
    {
        [Key]
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
    }
}
