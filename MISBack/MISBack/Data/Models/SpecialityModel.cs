using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class SpecialityModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
    }
}
