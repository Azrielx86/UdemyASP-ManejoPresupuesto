using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaccion transaccion);
    }
}