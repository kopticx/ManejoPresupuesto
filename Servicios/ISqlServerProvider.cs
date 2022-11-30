using System.Data;

namespace ManejoPresupuesto.Servicios
{
    public interface ISqlServerProvider
    {
        IDbConnection GetDbConnection();
    }
}
