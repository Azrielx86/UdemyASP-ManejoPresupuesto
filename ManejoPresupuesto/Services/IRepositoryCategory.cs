using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services;

public interface IRepositoryCategory
{
    Task Create(Categoria categoria);

    Task Delete(int id);

    Task<IEnumerable<Categoria>> GetAll(int usuarioId);

    Task<Categoria> GetById(int id, int usuarioId);

    Task Update(Categoria categoria);
}