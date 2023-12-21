using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class CommentModel
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    public DateTime createTime { get; set; }
    
    public DateTime? modifiedDate { get; set; }
    
    [Required]
    [MinLength(1)]
    public string content { get; set; }
    
    [Required]
    public Guid authorId { get; set; }
    
    [Required]
    public string author { get; set; }
    
    public Guid? parentId { get; set; }
}