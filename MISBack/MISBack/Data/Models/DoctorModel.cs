using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class DoctorModel
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    public DateTime createTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public string name { get; set; }

    public DateTime? birthDate { get; set; }
    
    [Required]
    public Gender gender { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string email { get; set; }
    
    [Phone]
    public string? phone { get; set; }
}