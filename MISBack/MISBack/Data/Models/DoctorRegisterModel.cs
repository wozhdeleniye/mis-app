using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MISBack.Data.Enums;
using MISBack.Services.ExceptionHandler;
using MISBack.Services.Validation;
using Newtonsoft.Json.Converters;

namespace MISBack.Data.Models;

public class DoctorRegisterModel 
{
    [StringLength(maximumLength: 200, MinimumLength = 1)]
    public string name { get; set; }

    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = EntityConstants.ShortOrLongPasswordError)]
    [RegularExpression(pattern: EntityConstants.PasswordRegex,
        ErrorMessage = EntityConstants.IncorrectPasswordError)]
    public string password { get; set; }

    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = EntityConstants.ShortOrLongEmailError)]
    [EmailAddress(ErrorMessage = EntityConstants.IncorrectEmailError)]
    public string email { get; set; }

    [ValideDateTime(ErrorMessage = EntityConstants.IncorrectDateError)]
    public DateTime? birthDate { get; set; }
    
    [Required]
    [EnumDataType(typeof(Gender), ErrorMessage = EntityConstants.IncorrectGenderError)]
    public Gender gender { get; set; }
    
    [Phone]
    public string? phone { get; set; }
    
    [Required]
    public Guid speciality { get; set; }
}