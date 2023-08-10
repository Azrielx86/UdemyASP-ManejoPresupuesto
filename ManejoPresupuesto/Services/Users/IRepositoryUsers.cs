using ManejoPresupuesto.Models.Users;

namespace ManejoPresupuesto.Services.Users;

public interface IRepositoryUsers
{
    Task<int> CreateUserAsync(User user);
    Task<User> GetUserByEmail(string emailNormalizado);
}