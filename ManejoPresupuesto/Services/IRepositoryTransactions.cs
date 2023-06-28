using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaccion transaccion);

        Task Delete(int id);

        Task<Transaccion> GetById(int id, int usuarioId);

        Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
    }
}