using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services;

public interface IRepositoryTiposCuentas
{
    Task Create(TipoCuenta tipoCuenta);

    Task Delete(int id);

    Task<bool> Exists(string nombre, int usuarioid);

    Task<IEnumerable<TipoCuenta>> GetAll(int usuarioId);

    Task<TipoCuenta> GetById(int id, int usuarioId);

    Task Order(IEnumerable<TipoCuenta> tiposCuentaOrdenados);

    Task Update(TipoCuenta tipoCuenta);
}