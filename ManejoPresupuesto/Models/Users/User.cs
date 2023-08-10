namespace ManejoPresupuesto.Models.Users;

#nullable disable

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string EmailNormalizado { get; set; }
    public string PasswordHash { get; set; }
}