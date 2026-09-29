using System;
using System.Collections.Generic;
using System.IO;

namespace C_Extended.Core
{
    // Recorre el archivo una sola vez. Cada rama representa transiciones de un AFD.
    public sealed class AnalizadorLexico
    {
        private string codigoFuente = "";
        private int indiceActual;
        private int lineaActual;
        private int columnaActual;
        private readonly List<Token> listaTokens = new List<Token>();
        private readonly List<ErrorLexico> listaErrores = new List<ErrorLexico>();
        private TablaDeSimbolos tablaSimbolos = new TablaDeSimbolos();

        public int TotalLineas { get { return lineaActual; } }

        public void AnalizarArchivo(string ruta)
        {
            Analizar(File.ReadAllText(ruta));
        }

        public void Analizar(string codigo)
        {
            codigoFuente = codigo ?? "";
            indiceActual = 0;
            lineaActual = 1;
            columnaActual = 1;
            listaTokens.Clear();
            listaErrores.Clear();
            tablaSimbolos = new TablaDeSimbolos();

            while (!Fin)
            {
                char actual = Actual;
                if (char.IsWhiteSpace(actual))
                {
                    Avanzar();
                }
                else if (AutomataAFD.EsInicioIdentificador(actual))
                {
                    LeerIdentificador();
                }
                else if (char.IsDigit(actual))
                {
                    LeerNumero();
                }
                else if (actual == '"')
                {
                    LeerLiteral('"', "TK_CADENA", "Cadena de texto sin cerrar");
                }
                else if (actual == '\'')
                {
                    LeerLiteral('\'', "TK_CARACTER", "Carácter literal sin cerrar");
                }
                else if (actual == '/' && Siguiente == '/')
                {
                    LeerComentarioLinea();
                }
                else if (actual == '/' && Siguiente == '*')
                {
                    LeerComentarioBloque();
                }
                else
                {
                    LeerOperadorODelimitador();
                }
            }
        }

        public List<Token> ObtenerTokens() { return new List<Token>(listaTokens); }
        public List<ErrorLexico> ObtenerErrores() { return new List<ErrorLexico>(listaErrores); }
        public List<Simbolo> ObtenerSimbolos() { return tablaSimbolos.ObtenerSimbolos(); }

        private bool Fin { get { return indiceActual >= codigoFuente.Length; } }
        private char Actual { get { return Fin ? '\0' : codigoFuente[indiceActual]; } }
        private char Siguiente
        {
            get { return indiceActual + 1 < codigoFuente.Length ? codigoFuente[indiceActual + 1] : '\0'; }
        }

        // CRLF se cuenta como un solo salto; las columnas son de base 1.
        private void Avanzar()
        {
            if (Fin) return;
            char c = codigoFuente[indiceActual++];
            if (c == '\r')
            {
                if (!Fin && codigoFuente[indiceActual] == '\n') indiceActual++;
                lineaActual++;
                columnaActual = 1;
            }
            else if (c == '\n')
            {
                lineaActual++;
                columnaActual = 1;
            }
            else
            {
                columnaActual++;
            }
        }

        private string LexemaDesde(int inicio)
        {
            return codigoFuente.Substring(inicio, indiceActual - inicio);
        }

        private void TokenDesde(int inicio, int linea, int columna, string tipo)
        {
            listaTokens.Add(new Token(LexemaDesde(inicio), tipo, linea, columna));
        }

        private void ErrorDesde(int inicio, int linea, int columna, string descripcion)
        {
            listaErrores.Add(new ErrorLexico(LexemaDesde(inicio), linea, columna, descripcion));
        }

        private void LeerIdentificador()
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            // Estado S1: (letra | _) (letra | dígito | _)*.
            do { Avanzar(); } while (!Fin && AutomataAFD.EsParteIdentificador(Actual));
            string lexema = LexemaDesde(inicio);
            if (AutomataAFD.EsReservada(lexema))
                TokenDesde(inicio, linea, columna, "TK_RESERVADA");
            else
            {
                TokenDesde(inicio, linea, columna, "TK_IDENTIFICADOR");
                tablaSimbolos.AgregarSimbolo(lexema, linea, columna);
            }
        }

        private void LeerNumero()
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            // S2: dígitos; S3: punto exige un dígito; S4: más dígitos.
            while (!Fin && char.IsDigit(Actual)) Avanzar();
            bool decimalValido = false;
            if (!Fin && Actual == '.')
            {
                Avanzar();
                if (Fin || !char.IsDigit(Actual))
                {
                    while (!Fin && (Actual == '.' || char.IsDigit(Actual))) Avanzar();
                    ErrorDesde(inicio, linea, columna, "Número decimal mal formado: falta un dígito después del punto");
                    return;
                }
                decimalValido = true;
                while (!Fin && char.IsDigit(Actual)) Avanzar();
            }

            if (!Fin && (Actual == '.' || AutomataAFD.EsInicioIdentificador(Actual)))
            {
                while (!Fin && (char.IsDigit(Actual) || Actual == '.' ||
                                AutomataAFD.EsInicioIdentificador(Actual))) Avanzar();
                ErrorDesde(inicio, linea, columna,
                    decimalValido || LexemaDesde(inicio).Contains(".")
                    ? "Número decimal mal formado" : "Identificador con inicio numérico");
                return;
            }
            TokenDesde(inicio, linea, columna, decimalValido ? "TK_NUM_DECIMAL" : "TK_NUM_ENTERO");
        }

        private void LeerLiteral(char comilla, string tipo, string sinCerrar)
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            bool escapeInvalido = false;
            int caracteres = 0;
            Avanzar(); // comilla inicial
            while (!Fin && Actual != '\r' && Actual != '\n')
            {
                if (Actual == comilla)
                {
                    Avanzar();
                    if (escapeInvalido)
                        ErrorDesde(inicio, linea, columna, "Secuencia de escape no válida");
                    else if (tipo == "TK_CARACTER" && caracteres != 1)
                        ErrorDesde(inicio, linea, columna, "Un carácter literal debe contener exactamente un carácter");
                    else
                        TokenDesde(inicio, linea, columna, tipo);
                    return;
                }
                if (Actual == '\\')
                {
                    Avanzar();
                    if (Fin || Actual == '\r' || Actual == '\n') break;
                    if (!AutomataAFD.EsEscapeValido(Actual)) escapeInvalido = true;
                    Avanzar();
                }
                else
                {
                    Avanzar();
                }
                caracteres++;
            }
            ErrorDesde(inicio, linea, columna, sinCerrar);
        }

        private void LeerComentarioLinea()
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            Avanzar(); Avanzar();
            while (!Fin && Actual != '\r' && Actual != '\n') Avanzar();
            TokenDesde(inicio, linea, columna, "TK_COMENTARIO_LINEA");
        }

        private void LeerComentarioBloque()
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            Avanzar(); Avanzar();
            while (!Fin)
            {
                if (Actual == '*' && Siguiente == '/')
                {
                    Avanzar(); Avanzar();
                    TokenDesde(inicio, linea, columna, "TK_COMENTARIO_BLOQUE");
                    return;
                }
                Avanzar();
            }
            ErrorDesde(inicio, linea, columna, "Comentario multilínea sin cerrar");
        }

        private void LeerOperadorODelimitador()
        {
            int inicio = indiceActual, linea = lineaActual, columna = columnaActual;
            char c = Actual, siguiente = Siguiente;
            string tipo = null;

            // Se prueba primero el operador de dos caracteres (máximo avance).
            if ((c == '+' && (siguiente == '+' || siguiente == '=')) ||
                (c == '-' && (siguiente == '-' || siguiente == '=')) ||
                ((c == '*' || c == '/' || c == '%') && siguiente == '=') ||
                ((c == '=' || c == '!' || c == '<' || c == '>') && siguiente == '=') ||
                (c == '&' && siguiente == '&') || (c == '|' && siguiente == '|'))
            {
                Avanzar(); Avanzar();
                if (siguiente == '=' && (c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '='))
                    tipo = c == '=' ? "TK_OP_RELACIONAL" : "TK_OP_ASIGNACION";
                else if (c == '&' || c == '|') tipo = "TK_OP_LOGICO";
                else if (c == '!' || c == '<' || c == '>') tipo = "TK_OP_RELACIONAL";
                else tipo = "TK_OP_ARITMETICO";
            }
            else
            {
                Avanzar();
                switch (c)
                {
                    case '+': case '-': case '*': case '/': case '%': tipo = "TK_OP_ARITMETICO"; break;
                    case '<': case '>': tipo = "TK_OP_RELACIONAL"; break;
                    case '=': tipo = "TK_OP_ASIGNACION"; break;
                    case '!': tipo = "TK_OP_LOGICO"; break;
                    case '(': case ')': case '{': case '}': case '[': case ']':
                    case ';': case ',': case '.': case ':': tipo = "TK_DELIMITADOR"; break;
                }
            }
            if (tipo == null) ErrorDesde(inicio, linea, columna, "Carácter no permitido");
            else TokenDesde(inicio, linea, columna, tipo);
        }
    }
}
