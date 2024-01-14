using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class CommentCreateModel
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string content { get; set; }
    
    public Guid? parentId { get; set; }
}