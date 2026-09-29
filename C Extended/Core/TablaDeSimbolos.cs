using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Extended.Core
{
    public sealed class Simbolo
    {
        public string NombreIdentificador { get; private set; }
        public string TipoToken { get; private set; }
        public int LineaPrimeraAparicion { get; private set; }
        public int ColumnaInicio { get; private set; }

        public Simbolo(string nombre, int linea, int columna)
        {
            NombreIdentificador = nombre;
            TipoToken = "TK_IDENTIFICADOR";
            LineaPrimeraAparicion = linea;
            ColumnaInicio = columna;
        }
    }

    public sealed class TablaDeSimbolos
    {
        private readonly Dictionary<string, Simbolo> tabla =
            new Dictionary<string, Simbolo>(StringComparer.Ordinal);
        private readonly List<Simbolo> ordenAparicion = new List<Simbolo>();

        public void AgregarSimbolo(string lexema, int linea, int columna)
        {
            if (tabla.ContainsKey(lexema)) return;
            var simbolo = new Simbolo(lexema, linea, columna);
            tabla.Add(lexema, simbolo);
            ordenAparicion.Add(simbolo);
        }

        public List<Simbolo> ObtenerSimbolos()
        {
            return new List<Simbolo>(ordenAparicion);
        }
    }
}
