using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validations;

public class FirstCharMayusAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var firstChar = value.ToString()!.First().ToString();
        if (firstChar != firstChar.ToUpper())
            return new ValidationResult("La primera letra debe ser mayúscula");

        return ValidationResult.Success;
    }
}