using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using C_Extended.Core;

internal static class Program
{
    private static int aprobadas;
    private static int fallidas;
    private static readonly StringBuilder informe = new StringBuilder();

    private static void Verificar(string nombre, bool condicion)
    {
        if (condicion) aprobadas++; else fallidas++;
        informe.AppendLine((condicion ? "APROBADA" : "FALLIDA") + " | " + nombre);
    }

    private static AnalizadorLexico Archivo(string ruta)
    {
        var analizador = new AnalizadorLexico();
        analizador.AnalizarArchivo(ruta);
        informe.AppendLine("ARCHIVO | " + Path.GetFileName(ruta) +
            " | tokens=" + analizador.ObtenerTokens().Count +
            " | símbolos=" + analizador.ObtenerSimbolos().Count +
            " | errores=" + analizador.ObtenerErrores().Count);
        foreach (var error in analizador.ObtenerErrores())
            informe.AppendLine("  ERROR " + error.Linea + ":" + error.Columna +
                " | " + error.Descripcion + " | " + error.CaracterInvalido.Replace("\r", "\\r").Replace("\n", "\\n"));
        return analizador;
    }

    private static bool HayToken(AnalizadorLexico a, string lexema, string tipo)
    {
        return a.ObtenerTokens().Any(t => t.Lexema == lexema && t.TipoToken == tipo);
    }

    private static int Main(string[] args)
    {
        if (args.Length != 1 || !Directory.Exists(args[0]))
        {
            Console.Error.WriteLine("Uso: PruebasConsola <carpeta-de-archivos-de-prueba>");
            return 2;
        }

        var raiz = args[0];
        var a1 = Archivo(Path.Combine(raiz, "prueba1_sencillo.txt"));
        Verificar("Prueba 1: sin errores", a1.ObtenerErrores().Count == 0);
        Verificar("Prueba 1: reservada e identificador", HayToken(a1, "namespace", "TK_RESERVADA") && HayToken(a1, "contador", "TK_IDENTIFICADOR"));
        Verificar("Prueba 1: += es asignación", HayToken(a1, "+=", "TK_OP_ASIGNACION"));
        var contador = a1.ObtenerSimbolos().SingleOrDefault(s => s.NombreIdentificador == "contador");
        Verificar("Prueba 1: símbolo único y primera posición", contador != null && contador.LineaPrimeraAparicion == 5 && contador.ColumnaInicio == 13 && a1.ObtenerSimbolos().Count(s => s.NombreIdentificador == "contador") == 1);

        var a2 = Archivo(Path.Combine(raiz, "prueba2_medio.txt"));
        Verificar("Prueba 2: sin errores", a2.ObtenerErrores().Count == 0);
        Verificar("Prueba 2: decimal, for y continue", HayToken(a2, "2.5", "TK_NUM_DECIMAL") && HayToken(a2, "for", "TK_RESERVADA") && HayToken(a2, "continue", "TK_RESERVADA"));

        var a3 = Archivo(Path.Combine(raiz, "prueba3_errores.txt"));
        Verificar("Prueba 3: cuatro errores visibles", a3.ObtenerErrores().Count == 4);
        Verificar("Prueba 3: identificación de errores", a3.ObtenerErrores().Any(e => e.Descripcion.Contains("inicio numérico")) && a3.ObtenerErrores().Any(e => e.Descripcion.Contains("decimal mal formado")) && a3.ObtenerErrores().Any(e => e.Descripcion.Contains("Cadena de texto sin cerrar")) && a3.ObtenerErrores().Any(e => e.Descripcion.Contains("Comentario multilínea sin cerrar")));
        Verificar("Prueba 3: @ permanece dentro del comentario abierto", !a3.ObtenerErrores().Any(e => e.CaracterInvalido == "@"));

        var a4 = Archivo(Path.Combine(raiz, "prueba4_recuperacion.txt"));
        Verificar("Prueba 4: carácter prohibido y número mal formado", a4.ObtenerErrores().Count == 2 && a4.ObtenerErrores()[0].CaracterInvalido == "@" && a4.ObtenerErrores()[0].Linea == 2 && a4.ObtenerErrores()[0].Columna == 1);
        Verificar("Prueba 4: sigue hasta final tras dos errores", HayToken(a4, "final", "TK_IDENTIFICADOR") && HayToken(a4, "despues", "TK_IDENTIFICADOR"));

        var a5 = Archivo(Path.Combine(raiz, "prueba5_categorias.txt"));
        Verificar("Prueba 5: sin errores", a5.ObtenerErrores().Count == 0);
        foreach (string tipo in new[] { "TK_RESERVADA", "TK_IDENTIFICADOR", "TK_NUM_ENTERO", "TK_CADENA", "TK_CARACTER", "TK_OP_ARITMETICO", "TK_OP_RELACIONAL", "TK_OP_LOGICO", "TK_OP_ASIGNACION", "TK_DELIMITADOR", "TK_COMENTARIO_LINEA", "TK_COMENTARIO_BLOQUE" })
            Verificar("Prueba 5: categoría " + tipo, a5.ObtenerTokens().Any(t => t.TipoToken == tipo));

        var extra = new AnalizadorLexico();
        extra.Analizar("var 12abc = 1;\r\nvar x = 'ab';\r\nvar y = \"mal\\q\";\r\nvar z = 2;");
        Verificar("Extra: recuperación de tres errores", extra.ObtenerErrores().Count == 3 && HayToken(extra, "z", "TK_IDENTIFICADOR"));
        Verificar("Extra: columnas con CRLF", extra.ObtenerTokens().Any(t => t.Lexema == "z" && t.Linea == 4 && t.Columna == 5));
        extra.Analizar("var a = 1..2; var b = 3;");
        Verificar("Extra: decimal con dos puntos es un solo error y continúa", extra.ObtenerErrores().Count == 1 && extra.ObtenerErrores()[0].CaracterInvalido == "1..2" && HayToken(extra, "b", "TK_IDENTIFICADOR"));
        extra.Analizar("var nuevo = 0;");
        Verificar("Extra: reinicio limpia símbolos y errores", extra.ObtenerErrores().Count == 0 && extra.ObtenerSimbolos().Count == 1 && extra.ObtenerSimbolos()[0].NombreIdentificador == "nuevo");

        informe.AppendLine("TOTAL | aprobadas=" + aprobadas + " | fallidas=" + fallidas);
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write(informe.ToString());
        File.WriteAllText(Path.Combine(raiz, "evidencia_pruebas_entregable3.txt"), informe.ToString(), new UTF8Encoding(false));
        return fallidas == 0 ? 0 : 1;
    }
}
