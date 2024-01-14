using MISBack.Data.Enums;
using MISBack.Services.Validation;
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models
{
    public class DoctorEditModel
    {
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = EntityConstants.ShortOrLongEmailError)]
        [EmailAddress(ErrorMessage = EntityConstants.IncorrectEmailError)]
        public string email { get; set; }

        [StringLength(maximumLength: 200, MinimumLength = 1)]
        public string name { get; set; }

        public DateTime? birthDate { get; set; }

        [Required]
        [EnumDataType(typeof(Gender), ErrorMessage = EntityConstants.IncorrectGenderError)]
        public Gender gender { get; set; }

        [Phone]
        public string? phone { get; set; }
    }
}
