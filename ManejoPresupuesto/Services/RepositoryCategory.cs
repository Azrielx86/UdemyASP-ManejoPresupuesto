using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services;

public class RepositoryCategory : IRepositoryCategory
{
    private readonly string connectionString;

    public RepositoryCategory(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Create(Categoria categoria)
    {
        await using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>("""
                                                        INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                        VALUES(@Nombre, @TipoOperacionId, @UsuarioId)
                                                        SELECT SCOPE_IDENTITY()
                                                        """, categoria);
        categoria.Id = id;
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE Categorias WHERE Id = @Id", new { id });
    }

    public async Task<IEnumerable<Categoria>?> Get(int usuarioId, TipoOperacion tipoOperacion)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Categoria>(@"SELECT Nombre, TipoOperacionId, UsuarioId, Id
                                                        FROM Categorias
                                                        WHERE UsuarioId = @UsuarioId AND TipoOperacionId = @tipoOperacion;",
            new { usuarioId, tipoOperacion });
    }

    public async Task<IEnumerable<Categoria>?> GetAll(int usuarioId, PaginacionViewModel paginacion)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Categoria>($"""
                                                       SELECT Nombre, TipoOperacionId, UsuarioId, Id
                                                       FROM Categorias
                                                       WHERE UsuarioId = @UsuarioId
                                                       ORDER BY Nombre
                                                       OFFSET {paginacion.RecordsIgnore}
                                                       ROWS FETCH NEXT {paginacion.RecordsPerPage}
                                                       ROWS ONLY;
                                                       """, new { usuarioId });
    }

    public async Task<Categoria?> GetById(int id, int usuarioId)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Categoria>(@"SELECT *
                                                                    FROM Categorias
                                                                    WHERE Id = @Id AND UsuarioId = @UsuarioId;",
            new
            {
                id,
                usuarioId
            });
    }

    public async Task Update(Categoria categoria)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(@"UPDATE Categorias
                                        SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                                        WHERE Id = @Id", categoria);
    }

    public async Task<int> Count(int usuarioId)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @usuarioId",
            new { usuarioId });
    }
}