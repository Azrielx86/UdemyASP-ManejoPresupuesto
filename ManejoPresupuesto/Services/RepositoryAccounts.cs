﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services;

public class RepositoryAccounts : IRepositoryAccounts
{
    private readonly string connectionString;

    public RepositoryAccounts(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Create(Cuenta cuenta)
    {
        await using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas(Nombre, TipoCuentaId, Descripcion, Balance)
                                                    VALUES(@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                                                    SELECT SCOPE_IDENTITY();", cuenta);
        cuenta.Id = id;
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE Cuentas WHERE Id = @Id", new { id });
    }

    public async Task<Cuenta?> GetById(int id, int usuarioId)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Cuenta>
                                                 (@"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, Descripcion, TipoCuentaId
                                                    FROM Cuentas
                                                    INNER JOIN TiposCuentas tc
                                                    ON tc.Id = Cuentas.TipoCuentaId
                                                    WHERE tc.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", new { usuarioId, id });
    }

    public async Task<IEnumerable<Cuenta>?> Search(int usuarioId)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, tc.Nombre AS TipoCuenta
                                                    FROM Cuentas
                                                    INNER JOIN TiposCuentas tc
                                                    ON tc.Id = Cuentas.TipoCuentaId
                                                    WHERE tc.UsuarioId = @UsuarioId
                                                    ORDER BY tc.Orden", new { usuarioId });
    }

    public async Task Update(CuentasCreacionViewModel cuentaEditar)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(@"
                                        UPDATE Cuentas
                                        SET Nombre = @Nombre,
                                        Balance = @Balance,
                                        Descripcion = @Descripcion,
                                        TipoCuentaId = @TipoCuentaId
                                        WHERE Id = @Id;
                                        ",
                                        new
                                        {
                                            cuentaEditar.Nombre,
                                            cuentaEditar.Balance,
                                            cuentaEditar.Descripcion,
                                            cuentaEditar.TipoCuentaId,
                                            cuentaEditar.Id
                                        });
    }
}