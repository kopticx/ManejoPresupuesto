﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly ISqlServerProvider sqlServerProvider;

        public RepositorioCuentas(ISqlServerProvider sqlServerProvider)
        {
            this.sqlServerProvider = sqlServerProvider;
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var con = sqlServerProvider.GetDbConnection();
            var id = await con.QuerySingleAsync<int>(@"INSERT INTO Cuentas(Nombre, TipoCuentaId, Balance, Descripcion)
                                                         VALUES(@Nombre, @TipoCuentaId, @Balance, @Descripcion)
                                                         SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var con = sqlServerProvider.GetDbConnection();

            return await con.QueryAsync<Cuenta>(@"SELECT cuentas.id, cuentas.Nombre, Balance, TC.Nombre TipoCuenta 
                                                  FROM Cuentas
                                                  INNER JOIN TiposCuentas TC
                                                  ON TC.Id = Cuentas.TipoCuentaId
                                                  WHERE TC.UsuarioId = @UsuarioId
                                                  ORDER BY TC.Orden;", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var con = sqlServerProvider.GetDbConnection();

            return await con.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT cuentas.id, cuentas.Nombre, Balance, Descripcion, TipoCuentaId
                                                                FROM Cuentas
                                                                INNER JOIN TiposCuentas TC
                                                                ON TC.Id = Cuentas.TipoCuentaId
                                                                WHERE TC.UsuarioId = @UsuarioId
                                                                AND Cuentas.Id = @Id", new { id, usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var con = sqlServerProvider.GetDbConnection();

            await con.ExecuteAsync(@"UPDATE Cuentas SET Nombre = @Nombre, Balance = @Balance, 
                                     Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                                     WHERE Id = @Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var con = sqlServerProvider.GetDbConnection();

            await con.ExecuteAsync(@"DELETE Cuentas WHERE Id = @Id", new { id });
        }
    }
}
