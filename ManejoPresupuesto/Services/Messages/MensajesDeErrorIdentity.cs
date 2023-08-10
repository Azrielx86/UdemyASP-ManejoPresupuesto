using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Services.Messages;

public class MensajesDeErrorIdentity : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresLower() => new()
    {
        Code = nameof(PasswordRequiresLower),
        Description = "La contraseña requiere al menos un carácter en minúsculas ('a'-'z')."
    };

    public override IdentityError PasswordRequiresUpper() => new()
    {
        Code = nameof(PasswordRequiresLower),
        Description = "La contraseña requiere al menos un carácter en mayúsculas ('A'-'Z')"
    };

    public override IdentityError PasswordRequiresDigit() => new()
    {
        Code = nameof(PasswordRequiresDigit),
        Description = "La contraseña requiere al menos un dígito."
    };

    public override IdentityError PasswordTooShort(int length) => new()
    {
        Code = nameof(PasswordTooShort),
        Description = $"La contraseña debe de tener al menos {length} dígitos."
    };
}