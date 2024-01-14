using System.ComponentModel.DataAnnotations;

namespace MISBack.Services.Validation
{
    public class ValideDateTimeAttribute : ValidationAttribute
    {
        public ValideDateTimeAttribute() 
        {
        }
        public override bool IsValid(object? value)
        {
            bool result = false;
            if(value is DateTime date)
            {
                result = true;

                if(date >= DateTime.Today)
                {
                    ErrorMessage = "Date can't be later than yesterday";
                    result = false;
                }
            }
            return result;
        }
    }
}
