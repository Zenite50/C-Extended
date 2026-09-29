using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Extended.Core
{
    // Alfabeto y estados auxiliares del reconocedor manual; no usa Regex.
    public static class AutomataAFD
    {
        private static readonly HashSet<string> Reservadas = new HashSet<string>(
            new[] {
                "namespace", "class", "enum", "func", "var", "nil",
                "if", "else", "for", "while", "return", "break", "continue",
                "int", "float", "double", "char", "bool", "string", "void",
                "static", "true", "false", "switch", "case", "default",
                "public", "private", "using", "new"
            }, StringComparer.Ordinal);

        public static bool EsInicioIdentificador(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        public static bool EsParteIdentificador(char c)
        {
            return EsInicioIdentificador(c) || char.IsDigit(c);
        }

        public static bool EsReservada(string lexema)
        {
            return Reservadas.Contains(lexema);
        }

        public static bool EsEscapeValido(char c)
        {
            return c == 'n' || c == 't' || c == 'r' || c == '0' ||
                   c == '\\' || c == '"' || c == '\'';
        }
    }
}
