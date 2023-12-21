using MISBack.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class DoctorEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string name { get; set; }

        public DateTime? birthDate { get; set; }

        [Required]
        public Gender gender { get; set; }

        [Phone]
        public string? phone { get; set; }
    }
}
