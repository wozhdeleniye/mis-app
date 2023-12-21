using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class DoctorRegisterModel
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string name { get; set; }
    
    [Required]
    [MinLength(6)]
    public string password { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string email { get; set; }
    
    public DateTime? birthDate { get; set; }
    
    [Required]
    public Gender gender { get; set; }
    
    [Phone]
    public string? phone { get; set; }
    
    [Required]
    public Guid speciality { get; set; }
}