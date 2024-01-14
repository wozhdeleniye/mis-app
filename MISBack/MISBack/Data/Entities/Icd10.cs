using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Icd10
    {
        [Key]
        [Required]
        public int id { get; set; }

        public DateTime? createTime { get; set; }

        public string? code { get; set; }

        public string? name { get; set; }
        
        public int? parentId { get; set; }
    }
}
