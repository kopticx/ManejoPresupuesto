using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var con = new SqlConnection(connectionString);

            var UsuarioId = await con.QuerySingleAsync<int>(@"INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                                                       VALUES (@Email, @EmailNormalizado, @PasswordHash)
                                                       SELECT SCOPE_IDENTITY()", usuario);

            await con.ExecuteAsync("CrearDatosUsuarioNuevo", new { UsuarioId }, commandType: CommandType.StoredProcedure);

            return UsuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QuerySingleOrDefaultAsync<Usuario>(@"SELECT * FROM Usuarios 
                                                                  WHERE EmailNormalizado = @emailNormalizado", new { emailNormalizado});
        }
    }
}
