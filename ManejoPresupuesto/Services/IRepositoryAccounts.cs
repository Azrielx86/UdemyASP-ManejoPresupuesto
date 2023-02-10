using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services;

public interface IRepositoryAccounts
{
    Task Create(Cuenta cuenta);
    Task Delete(int id);
    Task<Cuenta> GetById(int id, int usuarioId);
    Task<IEnumerable<Cuenta>> Search(int usuarioId);
    Task Update(CuentasCreacionViewModel cuentaEditar);
}