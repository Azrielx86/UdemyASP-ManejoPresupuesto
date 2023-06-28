using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services;

public class RepositoryTransactions : IRepositoryTransactions
{
    private readonly string connectionString;

    public RepositoryTransactions(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Create(Transaccion transaccion)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>(
            "Transacciones_Insertar",
            new
            {
                transaccion.UsuarioId,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota
            },
            commandType: System.Data.CommandType.StoredProcedure
            );

        transaccion.Id = id;
    }

    public async Task<Transaccion> GetById(int id, int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"
        SELECT * FROM Transacciones
        INNER JOIN Categorias cat
        ON cat.Id = Transacciones.CategoriaId
        WHERE Transacciones.Id = @Id
        AND Transacciones.UsuarioId = @UsuarioId;
        ", new { id, usuarioId });
    }

    public async Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("Transaccion_Actualizar", new
        {
            transaccion.Id,
            transaccion.FechaTransaccion,
            transaccion.Monto,
            montoAnterior,
            transaccion.CuentaId,
            cuentaAnteriorId,
            transaccion.CategoriaId,
            transaccion.Nota
        }, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task Delete(int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("Transaccion_Eliminar", new { id },
                    commandType: System.Data.CommandType.StoredProcedure);
    }
}