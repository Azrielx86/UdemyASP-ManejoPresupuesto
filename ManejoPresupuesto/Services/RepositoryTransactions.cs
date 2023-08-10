using Dapper;
using ManejoPresupuesto.Models.Transacciones;
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

    public async Task<IEnumerable<Transaccion>> GetByDate(TransaccionesPorFecha modelo)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Transaccion>(@"
                        SELECT t.Id, t.FechaTransaccion, t.Monto, cu.Nombre AS Cuenta,
                        cat.Nombre AS Categoria, cat.TipoOperacionId
                        FROM Transacciones t
                        INNER JOIN Cuentas cu
                        ON t.CuentaId = cu.Id
                        INNER JOIN Categorias cat
                        ON t.CategoriaId = cat.Id
                        WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                        AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                        ", modelo);
    }

    public async Task<IEnumerable<TransaccionesSemanal>> GetWeekly(TransaccionesPorUsuario modelo)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<TransaccionesSemanal>(@"SELECT DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana,
                                                                SUM(Monto) as Monto, cat.TipoOperacionId
                                                                FROM Transacciones t
                                                                INNER JOIN Categorias cat
                                                                ON cat.Id = t.CategoriaId
                                                                WHERE t.UsuarioId = @usuarioId AND
                                                                FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                                                                GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId", modelo);
    }

    public async Task<IEnumerable<TransaccionesMensual>> GetMonthly(int usuarioId, int year)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<TransaccionesMensual>(@"SELECT MONTH(t.FechaTransaccion) AS Month,
                                                                SUM(t.Monto) AS Monto, cat.TipoOperacionId
                                                                FROM Transacciones t
                                                                INNER JOIN Categorias cat
                                                                ON cat.Id = t.CategoriaId
                                                                WHERE t.UsuarioId = @usuarioId AND YEAR(t.FechaTransaccion) = @year
                                                                GROUP BY t.FechaTransaccion, cat.TipoOperacionId;", new { usuarioId, year });
    }

    public async Task<IEnumerable<Transaccion>> GetByUser(TransaccionesPorUsuario modelo)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Transaccion>(@"
                        SELECT t.Id, t.FechaTransaccion, t.Monto, cu.Nombre AS Cuenta,
                        cat.Nombre AS Categoria, cat.TipoOperacionId
                        FROM Transacciones t
                        INNER JOIN Cuentas cu
                        ON t.CuentaId = cu.Id
                        INNER JOIN Categorias cat
                        ON t.CategoriaId = cat.Id
                        WHERE t.UsuarioId = @UsuarioId
                        AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                        ORDER BY t.FechaTransaccion DESC", modelo);
    }

    public async Task<IEnumerable<Transaccion>> GetByUser(int UsuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Transaccion>(@"
                        SELECT t.Id, t.FechaTransaccion, t.Monto, cu.Nombre AS Cuenta,
                        cat.Nombre AS Categoria, cat.TipoOperacionId
                        FROM Transacciones t
                        INNER JOIN Cuentas cu
                        ON t.CuentaId = cu.Id
                        INNER JOIN Categorias cat
                        ON t.CategoriaId = cat.Id
                        WHERE t.UsuarioId = @UsuarioId
                        ORDER BY t.FechaTransaccion DESC", new { UsuarioId });
    }
}