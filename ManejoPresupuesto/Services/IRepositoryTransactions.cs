using ManejoPresupuesto.Models.Transacciones;

namespace ManejoPresupuesto.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaccion transaccion);

        Task Delete(int id);

        Task<IEnumerable<Transaccion>> GetByDate(TransaccionesPorFecha modelo);

        Task<Transaccion> GetById(int id, int usuarioId);

        Task<IEnumerable<Transaccion>> GetByUser(TransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> GetByUser(int UsuarioId);
        Task<IEnumerable<TransaccionesMensual>> GetMonthly(int usuarioId, int year);

        Task<IEnumerable<TransaccionesSemanal>> GetWeekly(TransaccionesPorUsuario modelo);

        Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
    }
}