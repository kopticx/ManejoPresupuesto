using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", new {nombre = tipoCuenta.Nombre, usuarioId = tipoCuenta.UsuarioId}, 
                                                             commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0 )
        {
            using var con = new SqlConnection(connectionString);
            var existe = await con.QueryFirstOrDefaultAsync<int>(@$"SELECT 1 FROM TiposCuentas 
                                                                    WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId AND Id <> @id;", new {nombre, usuarioId, id});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas 
                                                      WHERE UsuarioId = @UsuarioId
                                                      ORDER BY Orden;", new {usuarioId});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @Nombre
                                     WHERE Id = @Id;", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas
                                                                    WHERE Id = @Id AND UsuarioId = @UsuarioId;", new {id, usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"DELET TiposCuentas WHERE Id = @Id;", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
