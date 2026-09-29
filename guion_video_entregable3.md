# Guion de demostración (3-5 minutos)

1. **0:00-0:35.** Presentar el objetivo: motor léxico en C#, separado del formulario; mostrar `C Extended/Core` y las clases principales.
2. **0:35-1:25.** Enseñar cómo `AnalizadorLexico` avanza carácter por carácter, actualiza línea/columna y distingue identificadores, números, literales, operadores y comentarios. Señalar que no usa `Regex`.
3. **1:25-2:15.** Ejecutar la suite desde la carpeta `C Extended` con `dotnet run --project "PruebasConsola\PruebasConsola.csproj" -- "$PWD\Pruebas"`. Mostrar las tres pruebas originales y sus conteos.
4. **2:15-3:10.** Explicar `prueba3_errores.txt`: se detectan `12variable`, `45.12.3`, cadena sin cerrar y comentario sin cerrar. `@` está dentro del comentario abierto; mostrar `prueba4_recuperacion.txt`, donde sí se detecta y luego aparece el token `final`.
5. **3:10-3:50.** Mostrar la tabla de símbolos sin duplicados y `prueba5_categorias.txt` con caracteres y operadores lógicos.
6. **3:50-4:20.** Mostrar `evidencia_pruebas_entregable3.txt` y una compilación correcta de la solución. Cerrar con el reparto real de revisión y aprendizaje de cada integrante.

Grabar la pantalla y la voz de los integrantes. Verificar que el texto de consola sea legible y que el enlace del video sea accesible al docente antes de enviar.
