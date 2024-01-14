using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class TokenResponseModel
    {
        [Required]
        [MinLength(1)]
        public string Token { get; set; }
    }
}
