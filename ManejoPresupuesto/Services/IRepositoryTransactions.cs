using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaccion transaccion);

        Task Delete(int id);

        Task<IEnumerable<Transaccion>> GetByDate(TransaccionesPorFecha modelo);

        Task<Transaccion> GetById(int id, int usuarioId);
        Task<IEnumerable<Transaccion>> GetByUser(TransaccionesPorUsuario modelo);
        Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
    }
}