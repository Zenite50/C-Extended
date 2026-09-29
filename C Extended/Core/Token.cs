using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Extended.Core
{
    public sealed class Token
    {
        public string Lexema { get; private set; }
        public string TipoToken { get; private set; }
        public int Linea { get; private set; }
        public int Columna { get; private set; }

        public Token(string lexema, string tipoToken, int linea, int columna)
        {
            Lexema = lexema;
            TipoToken = tipoToken;
            Linea = linea;
            Columna = columna;
        }
    }
}
