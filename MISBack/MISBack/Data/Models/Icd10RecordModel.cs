using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class Icd10RecordModel
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    public DateTime createTime { get; set; }
    
    public string? code { get; set; }
    
    public string? name { get; set; }
}