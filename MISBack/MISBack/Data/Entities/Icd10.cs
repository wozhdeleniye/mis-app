using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Icd10
    {
        [Key]
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public string? code { get; set; }

        public string? name { get; set; }
    }
}
