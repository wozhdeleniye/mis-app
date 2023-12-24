using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Doctor
    {
        [Key]
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        public string name {  get; set; }
        public DateTime? birthday {  get; set; }
        [Required]
        public Gender gender {  get; set; }
        [Required]
        [EmailAddress]
        public string email {  get; set; }
        [Phone]
        public string? phone {  get; set; }
        [Required]
        [MinLength(6)]
        public string password { get; set; }
        [Required]
        public Guid speciality { get; set; }
    }
}
