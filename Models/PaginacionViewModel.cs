namespace ManejoPresupuesto.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina = 10;
        private readonly int CantidadMaximaRecordsPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > CantidadMaximaRecordsPorPagina) ? CantidadMaximaRecordsPorPagina : value;
            }
        }

        public int RecordsASaltar => recordsPorPagina * (Pagina - 1);
    }
}
