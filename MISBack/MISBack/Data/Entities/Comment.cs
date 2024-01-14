using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Comment
    {
        [Key]
        [Required]
        public Guid id { get; set; }

        [Required] 
        public Guid authorId { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public DateTime? modifiedDate { get; set; }

        [Required]
        public Guid consultationId { get; set; }

        public Guid? parentId { get; set; }

        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string content { get; set; }
    }
}
