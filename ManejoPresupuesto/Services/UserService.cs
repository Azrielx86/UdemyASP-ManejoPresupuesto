using System.Security.Claims;

namespace ManejoPresupuesto.Services;

public class UserService : IUserService
{
    private readonly HttpContext httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor.HttpContext!;
    }

    public int GetUserId()
    {
        if (httpContextAccessor.User.Identity is { IsAuthenticated: true })
        {
            var idClaim = httpContextAccessor.User.Claims
                .ToList()
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value
                .ToString();
            if (!int.TryParse(idClaim, out var id))
                throw new ApplicationException("El usuario no está autenticado");
            return id;
        }
        else
        {
            throw new ApplicationException("El usuario no está autenticado");
        }
    }
}