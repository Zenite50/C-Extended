using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Extended.Core
{
    public sealed class ErrorLexico
    {
        public string CaracterInvalido { get; private set; }
        public int Linea { get; private set; }
        public int Columna { get; private set; }
        public string Descripcion { get; private set; }

        public ErrorLexico(string caracterInvalido, int linea, int columna, string descripcion)
        {
            CaracterInvalido = caracterInvalido;
            Linea = linea;
            Columna = columna;
            Descripcion = descripcion;
        }
    }
}
