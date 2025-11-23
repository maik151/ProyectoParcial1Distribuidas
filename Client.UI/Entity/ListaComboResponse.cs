using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ListaComboResponse : RespuestaBase
    {
        public List<ItemComboDTO> Items { get; set; }
    }
}
