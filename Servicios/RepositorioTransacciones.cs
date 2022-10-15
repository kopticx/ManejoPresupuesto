using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var con = new SqlConnection(connectionString);

            var id = await con.QuerySingleAsync<int>("Transacciones_Insertar", new {transaccion.UsuarioId, transaccion.FechaTransaccion, 
                                                                                    transaccion.Monto, transaccion.CategoriaId, 
                                                                                    transaccion.CuentaId, transaccion.Nota}, commandType: CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Transaccion>(@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre Categoria, CU.Nombre Cuenta, C.TipoOperacionId, T.Nota
                                                       FROM Transacciones T 
                                                       INNER JOIN Categorias C
                                                       ON C.Id = T.CategoriaId
                                                       INNER JOIN Cuentas CU
                                                       on CU.Id = T.CuentaId
                                                       WHERE T.UsuarioId = @UsuarioId
                                                       AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                                       ORDER BY T.FechaTransaccion DESC", modelo);
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteReaderAsync(@"Transacciones_Actualizar", new {transaccion.Id, transaccion.FechaTransaccion, transaccion.Monto,
                                                                           transaccion.CategoriaId, transaccion.CuentaId, transaccion.Nota,
                                                                           montoAnterior, cuentaAnteriorId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT T.*, C.TipoOperacionId FROM Transacciones T
                                                                     INNER JOIN Categorias C
                                                                     ON C.Id = T.CategoriaId
                                                                     WHERE T.Id = @Id AND T.UsuarioId = @UsuarioId", new {id, usuarioId});
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Transaccion>(@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre Categoria, CU.Nombre Cuenta, C.TipoOperacionId
                                                       FROM Transacciones T 
                                                       INNER JOIN Categorias C
                                                       ON C.Id = T.CategoriaId
                                                       INNER JOIN Cuentas CU
                                                       on CU.Id = T.CuentaId
                                                       WHERE T.CuentaId = @CuentaId AND T.UsuarioId = @UsuarioId
                                                       AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var con = new SqlConnection(connectionString);

            return await con.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7 + 1 AS Semana,
                                                                     SUM(Monto) AS Monto, C.TipoOperacionId
                                                                     FROM Transacciones T
                                                                     INNER JOIN Categorias C
                                                                     ON C.Id = T.CategoriaId
                                                                     WHERE T.UsuarioId = @usuarioId AND
                                                                     FechaTransaccion BETWEEN @fechaInicio AND @fechafin
                                                                     GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7, C.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var con = new SqlConnection(connectionString);

            return await con.QueryAsync<ResultadoObtenerPorMes>(@"SELECT MONTH(FechaTransaccion) AS Mes,
                                                                  SUM(Monto) As Monto, C.TipoOperacionId
                                                                  FROM  Transacciones T
                                                                  INNER JOIN Categorias C
                                                                  ON C.Id = T.CategoriaId
                                                                  WHERE  T.UsuarioId = @usuarioId AND YEAR(FechaTransaccion) = @año
                                                                  GROUP BY MONTH(FechaTransaccion), C.TipoOperacionId", new {usuarioId, año});
        }

        public async Task Borrar(int id)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync("Transacciones_Borrar", new { id }, commandType: CommandType.StoredProcedure);
        }
    }
}
