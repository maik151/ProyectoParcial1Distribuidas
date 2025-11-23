using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class MantenimientoRequest : PeticionBase
    {
        public string Numero { get; set; } // Se puede generar auto o enviar
        public DateTime Fecha { get; set; }
        public string Responsable { get; set; }
        public List<DetalleMantDTO> Detalles { get; set; }
    }
}
