# Entregable 3 - núcleo del analizador léxico

Este proyecto parte del esqueleto Windows Forms del Entregable 2. Las clases en `C Extended/Core/` implementan el análisis léxico manual; el formulario sigue siendo el boceto de la próxima etapa.

## Abrir y compilar

Abra `C Extended.slnx` en Visual Studio con las herramientas de escritorio de .NET Framework 4.8 y compile la solución. El proyecto principal no necesita la interfaz para ejecutar las pruebas del núcleo.

En un equipo con el SDK de .NET 10, ejecute desde la carpeta `C Extended`:

```powershell
dotnet run --project "PruebasConsola\PruebasConsola.csproj" -- "$PWD\Pruebas"
```

El programa lee las copias de los tres archivos originales y `prueba4_recuperacion.txt` y `prueba5_categorias.txt` de la carpeta `Pruebas`. Escribe el resultado en `Pruebas/evidencia_pruebas_entregable3.txt`. Devuelve código de salida 1 si falla una comprobación.

## Diseño

- `AnalizadorLexico.AnalizarArchivo(ruta)` lee el texto y `Analizar(codigo)` recorre cada carácter con un índice, línea y columna explícitos.
- `AutomataAFD` define el alfabeto de identificadores, escapes y reservadas. Las transiciones se implementan en los métodos `Leer...` y en el `switch` de operadores y delimitadores.
- `Token`, `ErrorLexico` y `Simbolo` guardan los resultados; `TablaDeSimbolos` conserva solo la primera aparición de cada identificador.
- El reconocedor no usa `Regex` de .NET. Los comentarios se registran como tokens. `\r\n` cuenta como un salto de línea. Las posiciones empiezan en 1.

## Convenciones del subconjunto

Se conservan las reservadas del Entregable 1 (`func`, `var`, `nil`, etc.) y se incluyen palabras y categorías mínimas de la consigna general (`int`, `bool`, caracteres literales y operadores lógicos, entre otras). `:` se reconoce como delimitador para `case`. Las cadenas y caracteres aceptan escapes comunes (`\n`, `\t`, `\r`, `\0`, `\\`, `\"`, `\'`). No se implementan cadenas interpoladas, números hexadecimales ni comentarios de bloque anidados.

En `prueba3_errores.txt`, el símbolo `@` queda dentro de un comentario `/*` sin cierre. Por eso el archivo produce cuatro errores, no cinco. `prueba4_recuperacion.txt` comprueba `@` fuera de comentarios y la continuación del análisis.

## Antes de la entrega académica

El grupo debe revisar el código y el informe, registrar sus propios commits en el repositorio, crear la etiqueta o release del Entregable 3, grabar el video de 3 a 5 minutos y citar el apoyo de IA según las normas del curso. Ningún commit, etiqueta ni video está incluido en esta copia local.
