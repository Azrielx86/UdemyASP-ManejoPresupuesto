using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services;

public class RepositoryTiposCuentas : IRepositoryTiposCuentas
{
    private readonly string connectionString;

    public RepositoryTiposCuentas(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Create(TipoCuenta tipoCuenta)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>(@"TiposCuentasInsertar", new
        {
            usuarioId = tipoCuenta.UsuarioId,
            nombre = tipoCuenta.Nombre
        }, commandType: System.Data.CommandType.StoredProcedure);

        tipoCuenta.Id = id;
    }

    public async Task Delete(int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE TiposCuentas WHERE Id = @Id", new { id });
    }

    public async Task<bool> Exists(string nombre, int usuarioid, int id = 0)
    {
        using var connection = new SqlConnection(connectionString);
        var exists = await connection.QueryFirstOrDefaultAsync<int>(
                                                            @"SELECT 1
                                                            FROM TiposCuentas
                                                            WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId
                                                            AND Id <> @id;",
                                                            new { nombre, usuarioid, id }
                                                            );
        return exists == 1;
    }

    public async Task<IEnumerable<TipoCuenta>> GetAll(int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        var tiposCuenta = await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                    FROM TiposCuentas
                                                                    WHERE UsuarioId = @UsuarioId
                                                                    ORDER BY Orden;",
                                                                    new { usuarioId });
        return tiposCuenta;
    }

    public async Task<TipoCuenta> GetById(int id, int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                       FROM TiposCuentas
                                                                       WHERE Id = @Id AND UsuarioId = @UsuarioId;",
                                                                       new { id, usuarioId });
    }

    public async Task Order(IEnumerable<TipoCuenta> tiposCuentaOrdenados)
    {
        using var connection = new SqlConnection(connectionString);
        var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
        await connection.ExecuteAsync(query, tiposCuentaOrdenados);
    }

    public async Task Update(TipoCuenta tipoCuenta)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                        SET Nombre = @Nombre
                                        WHERE Id = @Id",
                                        new { tipoCuenta.Nombre, tipoCuenta.Id });
    }
}