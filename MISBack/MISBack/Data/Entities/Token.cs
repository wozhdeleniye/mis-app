using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities
{
    public class Token
    {
        [Required]
        public string InvalidToken { get; set; }
        [Required]
        public DateTime ExpiredDate { get; set; }
    }
}
