namespace BussinesEntity
{
    public class ModelosMantenimiento
    {
        // RF-MAN-01: Tabla ActividadMantenimiento
        public class ActividadMantenimiento
        {
            public string Codigo { get; set; } // PK
            public string Nombre { get; set; }
        }

        // RF-MAN-02: Tabla ActivoMantenimiento (Copia local)
        public class ActivoMantenimiento
        {
            public long ActivoId { get; set; } // PK
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public DateTime FechaCompra { get; set; }
        }
    }
}
