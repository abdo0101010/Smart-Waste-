using SmartWaste.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Validations
{
    public class UniqueEmailAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Email is required.");
            }
            string email =value.ToString();
            smartwasteContext dbContext = (smartwasteContext)validationContext.GetService(typeof(smartwasteContext));
            User user=  dbContext.Users.FirstOrDefault(e=>e.Email == email);
            if(user != null)
            {
                return new ValidationResult("Email already exists.");
            }
            return ValidationResult.Success;
        } 
    }
}
