using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services;

public interface IRepositoryCategory
{
    Task Create(Categoria categoria);

    Task Delete(int id);

    Task<IEnumerable<Categoria>?> Get(int userId, TipoOperacion tipoOperacion);

    Task<IEnumerable<Categoria>?> GetAll(int usuarioId, PaginacionViewModel paginacion);

    Task<Categoria?> GetById(int id, int usuarioId);

    Task Update(Categoria categoria);

    Task<int> Count(int usuarioId);
}