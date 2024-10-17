using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RestaurantBooking.API.Helpers
{
    public class ValidateNumericInput : ValidationAttribute
    {
        public ValidateNumericInput() : base("{0} only numeric characters are allowed [0-9]") { }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var val = value as string;
            string pattern = @"^\d+$"; // Esta expresión regular solo acepta números
            bool valid = Regex.IsMatch(input: val!, pattern: pattern);

            return valid ? null : new ValidationResult(base.FormatErrorMessage(validationContext.MemberName), [validationContext.MemberName]);
        }
    }
}
