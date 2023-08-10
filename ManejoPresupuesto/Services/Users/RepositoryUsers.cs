using System.Data;
using Dapper;
using ManejoPresupuesto.Models.Users;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services.Users;

public class RepositoryUsers : IRepositoryUsers
{
    private readonly string connectionString;

    public RepositoryUsers(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> CreateUserAsync(User user)
    {
        await using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>("""
                                                        INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                                                        VALUES (@Email, @EmailNormalizado, @PasswordHash);
                                                        SELECT SCOPE_IDENTITY();
                                                        """, user);
        await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new { usuarioId = id }, commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task<User> GetUserByEmail(string emailNormalizado)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Usuarios WHERE EmailNormalizado = @emailNormalizado", new { emailNormalizado });
    }
}