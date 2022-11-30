using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{
    public class SqlServerProvider : ISqlServerProvider
    {
        private readonly string connectionString;

        public SqlServerProvider(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
