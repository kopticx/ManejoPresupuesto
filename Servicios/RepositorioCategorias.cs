﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var con = new SqlConnection(connectionString);
            var id = await con.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                 VALUES (@Nombre, @TipoOperacionId, @UsuarioId)
                                                 SELECT SCOPE_IDENTITY()", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion)
        {
            using var con = new SqlConnection(connectionString);

            return await con.QueryAsync<Categoria>(@$"SELECT * FROM Categorias 
                                                     WHERE UsuarioId = @usuarioId 
                                                     ORDER BY Nombre
                                                     OFFSET {paginacion.RecordsASaltar} 
                                                     ROWS FETCH NEXT {paginacion.RecordsPorPagina} ROWS ONLY", new { usuarioId });
        }

        public async Task<int> Contar(int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM Categorias
                                                       WHERE UsuarioId = @usuarioId", new { usuarioId });
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var con = new SqlConnection(connectionString);

            return await con.QueryAsync<Categoria>(@"SELECT * FROM Categorias 
                                                     WHERE UsuarioId = @usuarioId 
                                                     AND TipoOperacionId = @tipoOperacionId", new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<Categoria>(@"SELECT * FROM Categorias 
                                                                   WHERE Id = @id AND UsuarioId = @usuarioId", new { id, usuarioId });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"UPDATE Categorias SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId 
                                     WHERE Id = @UsuarioId", categoria);
        }

        public async Task Borrar(int id)
        {
            var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"DELETE Categorias WHERE Id = @id", new { id });
        }
    }
}
