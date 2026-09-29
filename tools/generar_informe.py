"""Genera el informe breve del Entregable 3 a partir de resultados verificados."""
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas
from reportlab.platypus import Paragraph, Table, TableStyle


BASE = Path(__file__).resolve().parents[1]
SALIDA = BASE / "output" / "pdf" / "Entregable3_Ramirez_Sosa_Posadas.pdf"
SALIDA.parent.mkdir(parents=True, exist_ok=True)

pdfmetrics.registerFont(TTFont("Arial", r"C:\Windows\Fonts\arial.ttf"))
pdfmetrics.registerFont(TTFont("Arial-Bold", r"C:\Windows\Fonts\arialbd.ttf"))

AZUL = colors.HexColor("#17324D")
ACENTO = colors.HexColor("#147D82")
GRIS = colors.HexColor("#566575")
CLARO = colors.HexColor("#EDF3F5")
NEGRO = colors.HexColor("#17222D")
ANCHO, ALTO = A4
MARGEN = 54

estilos = {
    "normal": ParagraphStyle("normal", fontName="Arial", fontSize=9.3, leading=14.4,
                             textColor=NEGRO, spaceAfter=5),
    "small": ParagraphStyle("small", fontName="Arial", fontSize=8.2, leading=12.2,
                            textColor=NEGRO),
    "white": ParagraphStyle("white", fontName="Arial-Bold", fontSize=8.3, leading=11,
                            textColor=colors.white),
    "cell": ParagraphStyle("cell", fontName="Arial", fontSize=8.3, leading=11.5,
                           textColor=NEGRO),
}


def par(texto, estilo="normal"):
    return Paragraph(texto, estilos[estilo])


def bloque(c, texto, y, estilo="normal", ancho=ANCHO - 2 * MARGEN):
    p = par(texto, estilo)
    _, alto = p.wrap(ancho, ALTO)
    p.drawOn(c, MARGEN, y - alto)
    return y - alto - 8


def encabezado(c, numero, titulo):
    c.setFillColor(AZUL)
    c.rect(0, ALTO - 34, ANCHO, 34, fill=1, stroke=0)
    c.setFont("Arial-Bold", 9)
    c.setFillColor(colors.white)
    c.drawString(MARGEN, ALTO - 22, "LENGUAJES FORMALES Y DE PROGRAMACIÓN  /  PROYECTO FINAL")
    c.setFillColor(ACENTO)
    c.rect(MARGEN, ALTO - 85, 5, 33, fill=1, stroke=0)
    c.setFillColor(AZUL)
    c.setFont("Arial-Bold", 19)
    c.drawString(MARGEN + 15, ALTO - 73, titulo)
    c.setStrokeColor(colors.HexColor("#CED9DF"))
    c.line(MARGEN, 44, ANCHO - MARGEN, 44)
    c.setFillColor(GRIS)
    c.setFont("Arial", 8)
    c.drawString(MARGEN, 31, "Entregable 3  |  Implementación del motor léxico")
    c.drawRightString(ANCHO - MARGEN, 31, str(numero) + " / 4")
    return ALTO - 110


def portada(c):
    encabezado_cunor = BASE / "tools" / "assets" / "encabezado_cunor.png"
    alto_encabezado = 165
    c.drawImage(str(encabezado_cunor), 0, ALTO - alto_encabezado,
                width=ANCHO, height=alto_encabezado, preserveAspectRatio=False)

    c.setFillColor(colors.black)
    c.setFont("Times-Bold", 13)
    c.drawCentredString(ANCHO / 2, ALTO - 195, "Curso: Lenguajes formales de programación")
    c.drawCentredString(ANCHO / 2, ALTO - 217,
                        "Proyecto: Proyecto Final - Analizador Léxico")
    c.drawCentredString(ANCHO / 2, ALTO - 239,
                        "Entregable 3: Implementación del motor léxico")
    c.setFont("Times-Roman", 11)
    c.drawCentredString(ANCHO / 2, ALTO - 264, "Docente: Ingeniero José Alberto Veliz Cruz")

    c.setFont("Times-Bold", 13)
    c.drawString(72, ALTO - 300, "Integrantes")

    estilo_portada = ParagraphStyle(
        "portada", fontName="Times-Roman", fontSize=11, leading=13,
        textColor=colors.black
    )
    estilo_portada_bold = ParagraphStyle(
        "portada_bold", fontName="Times-Bold", fontSize=11, leading=13,
        textColor=colors.black
    )
    datos = [
        [Paragraph("Nombre", estilo_portada_bold),
         Paragraph("Carné", estilo_portada_bold),
         Paragraph("Carrera", estilo_portada_bold)],
        [Paragraph("Cristian Gabriel<br/>Ramírez Morales", estilo_portada),
         Paragraph("202540362", estilo_portada), Paragraph("Sistemas", estilo_portada)],
        [Paragraph("Enrique José Sosa<br/>Caal", estilo_portada),
         Paragraph("202544363", estilo_portada), Paragraph("Sistemas", estilo_portada)],
        [Paragraph("José Roberto<br/>Posadas Ascencio", estilo_portada),
         Paragraph("202544488", estilo_portada), Paragraph("Sistemas", estilo_portada)],
    ]
    tabla_portada = Table(datos, colWidths=[160, 105, 105],
                          rowHeights=[20, 43, 43, 43])
    tabla_portada.setStyle(TableStyle([
        ("GRID", (0, 0), (-1, -1), 0.7, colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 7),
        ("RIGHTPADDING", (0, 0), (-1, -1), 7),
        ("ALIGN", (0, 0), (-1, 0), "CENTER"),
    ]))
    tabla_portada.wrapOn(c, 370, 180)
    tabla_portada.drawOn(c, (ANCHO - 370) / 2, ALTO - 495)

    c.setFont("Times-Bold", 12)
    c.drawString(126, ALTO - 535, "Enlace de GitHub:")
    enlace = "https://github.com/Zenite50/C-Extended"
    c.setFont("Times-Roman", 12)
    c.setFillColor(colors.HexColor("#3E7E9E"))
    c.drawString(233, ALTO - 535, enlace)
    c.linkURL(enlace, (233, ALTO - 539, 466, ALTO - 522), relative=0)

    c.setFillColor(colors.black)
    c.setFont("Times-Roman", 12)
    c.drawCentredString(ANCHO / 2, 74, "Fecha de entrega: 28 de septiembre de 2026")
    c.showPage()


def subtitulo(c, texto, y):
    c.setFillColor(AZUL)
    c.setFont("Arial-Bold", 11)
    c.drawString(MARGEN, y, texto)
    return y - 16


def tabla(c, filas, anchos, y, alto_fila=29):
    data = [[par(str(valor), "white" if i == 0 else "cell") for valor in fila]
            for i, fila in enumerate(filas)]
    t = Table(data, colWidths=anchos, rowHeights=[alto_fila] * len(filas))
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), AZUL),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, CLARO]),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 8),
        ("RIGHTPADDING", (0, 0), (-1, -1), 7),
        ("TOPPADDING", (0, 0), (-1, -1), 3),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 3),
        ("LINEBELOW", (0, -1), (-1, -1), 0.5, colors.HexColor("#CED9DF")),
    ]))
    _, h = t.wrapOn(c, sum(anchos), ALTO)
    t.drawOn(c, MARGEN, y - h)
    return y - h - 13


c = canvas.Canvas(str(SALIDA), pagesize=A4)
c.setTitle("Entregable 3 - Motor léxico - Ramírez, Sosa y Posadas")
c.setAuthor("Cristian Gabriel Ramírez Morales; Enrique José Sosa Caal; José Roberto Posadas Ascencio")

portada(c)

# Página 2: alcance e implementación.
y = encabezado(c, 2, "Motor léxico en C#")
y = bloque(c, "<b>Docente:</b> Ing. José Alberto Veliz Cruz. <b>Integrantes:</b> Cristian Gabriel Ramírez Morales (202540362), Enrique José Sosa Caal (202544363) y José Roberto Posadas Ascencio (202544488). <b>Preparación:</b> 28 de septiembre de 2026.", y)
y = subtitulo(c, "Alcance de esta etapa", y - 3)
y = bloque(c, "Se implementó el núcleo del analizador a partir del esqueleto del Entregable 2. Procesa archivos de texto y entrega tokens con lexema, categoría, línea y columna; una tabla de símbolos sin duplicados; y errores léxicos con recuperación. El formulario gráfico conservado en la solución corresponde a la integración prevista para el Entregable 4.", y)
y = subtitulo(c, "Arquitectura", y - 5)
y = tabla(c, [
    ["Clase", "Responsabilidad"],
    ["AnalizadorLexico", "Recorrido carácter por carácter; estados, posiciones y recuperación."],
    ["AutomataAFD", "Alfabeto de identificadores, palabras reservadas y escapes."],
    ["Token / ErrorLexico", "Resultados con lexema o secuencia inválida y ubicación."],
    ["TablaDeSimbolos", "Identificadores únicos y primera aparición."],
], [140, ANCHO - 2 * MARGEN - 140], y, 35)
y = subtitulo(c, "Decisiones de implementación", y - 1)
for texto in [
    "Las ramas de reconocimiento aplican transiciones manuales; no se utiliza <i>Regex</i> de .NET como motor. Se priorizan operadores de dos caracteres y comentarios antes que división.",
    "Las posiciones empiezan en 1 y CRLF cuenta como un solo salto de línea. Un nuevo análisis reinicia tokens, errores y símbolos.",
    "Se conservan las palabras del subconjunto propio (por ejemplo, <i>func</i>, <i>var</i>, <i>nil</i>) y se cubren las categorías mínimas del proyecto general, incluidos caracteres literales y operadores lógicos.",
]:
    y = bloque(c, "• " + texto, y, "normal")
c.showPage()

# Página 3: evidencia y resultados.
y = encabezado(c, 3, "Pruebas y resultados")
y = bloque(c, "Se ejecutó un programa de consola con los tres archivos del Entregable 1 y dos archivos nuevos. Además de los conteos, las 28 verificaciones comprueban categorías, posiciones, unicidad de símbolos, reinicio y continuidad tras errores.", y)
y = tabla(c, [
    ["Archivo", "Tokens", "Símbolos", "Errores", "Resultado"],
    ["prueba1_sencillo.txt", "25", "3", "0", "Aprobado"],
    ["prueba2_medio.txt", "62", "9", "0", "Aprobado"],
    ["prueba3_errores.txt", "20", "4", "4", "Aprobado"],
    ["prueba4_recuperacion.txt", "21", "4", "2", "Aprobado"],
    ["prueba5_categorias.txt", "59", "7", "0", "Aprobado"],
], [196, 60, 68, 54, 109], y, 35)
y = subtitulo(c, "Errores observados en la prueba original", y - 1)
y = tabla(c, [
    ["Ubicación", "Secuencia", "Diagnóstico"],
    ["6:13", "12variable", "Identificador con inicio numérico"],
    ["9:22", "45.12.3", "Número decimal mal formado"],
    ["12:23", "Cadena sin cierre", "Cadena de texto sin cerrar"],
    ["14:9", "/* ... EOF", "Comentario multilínea sin cerrar"],
], [77, 122, ANCHO - 2 * MARGEN - 199], y, 34)
y = bloque(c, "<b>Interpretación:</b> el carácter <b>@</b> del archivo original aparece después de <b>/*</b> sin cierre. Pertenece al texto del comentario y no debe diagnosticarse por separado. En <i>prueba4_recuperacion.txt</i>, situado fuera de un comentario, se reporta en 2:1 y el análisis continúa hasta el identificador <i>final</i>.", y)
y = bloque(c, "<b>Resultado global:</b> 28 verificaciones aprobadas y 0 fallidas. La salida completa se conserva en <i>Pruebas/evidencia_pruebas_entregable3.txt</i>.", y)
c.showPage()

# Página 4: dificultades, reproducción y trazabilidad.
y = encabezado(c, 4, "Validación y cierre")
y = subtitulo(c, "Dificultades y soluciones", y)
for texto in [
    "<b>Esqueleto sin lógica:</b> el ZIP contenía las clases del núcleo vacías. Se implementó el reconocimiento en esas clases y se mantuvo separado del formulario.",
    "<b>Errores encadenados:</b> un lexema numérico inválido se consume completo para evitar tokens falsos; una cadena sin cerrar termina al fin de línea y permite continuar. Un comentario de bloque sin cierre consume hasta EOF, según su semántica.",
    "<b>Inconsistencia del archivo de errores:</b> el supuesto quinto error queda dentro del comentario abierto. Se agregó un caso independiente para verificar el carácter prohibido y la recuperación.",
]:
    y = bloque(c, "• " + texto, y)
y = subtitulo(c, "Compilación y ejecución reproducible", y - 4)
y = bloque(c, "La solución original de Windows Forms (objetivo .NET Framework 4.8) compiló sin advertencias ni errores en este equipo. La suite de consola se ejecutó con SDK .NET 10 y produjo 28/28 verificaciones aprobadas. Desde la raíz de <i>C Extended</i>:", y)
y = bloque(c, "<font name='Arial-Bold'>dotnet run --project PruebasConsola\\PruebasConsola.csproj -- .\\Pruebas</font>", y, "small")
y = bloque(c, "El comando de compilación del proyecto antiguo desde el SDK instalado requirió la propiedad <i>GenerateResourceMSBuildArchitecture=CurrentArchitecture</i> por la tarea de recursos de MSBuild. En Visual Studio con las herramientas de escritorio de .NET Framework 4.8 se puede compilar la solución de forma habitual.", y)
y = subtitulo(c, "Trazabilidad y alcance pendiente", y - 4)
y = bloque(c, "Se usaron como referencia la consigna <i>Proyecto_Analizador_Lexico_CSharp_WinForms.pdf</i> (Entregable 3, páginas 10-11), la especificación léxica del grupo y el documento del Entregable 2. El repositorio del grupo es <link href='https://github.com/Zenite50/C-Extended' color='#147D82'>github.com/Zenite50/C-Extended</link>. El <link href='https://1drv.ms/v/c/1d3c238191e93468/IQDIPjdBdGY1S7qOaWde8RWDAV8ziTD5rBrfi_h5XLoNF24?e=hWsJN0' color='#147D82'>video de demostración en OneDrive</link> dura 3 minutos y 48 segundos y no se almacena en el repositorio.", y)
y = bloque(c, "<b>Uso de IA:</b> Codex asistió en la implementación, la elaboración de pruebas y la redacción de este informe. El grupo debe revisar, comprender y declarar este apoyo al realizar sus propios commits y la entrega, conforme a las normas del proyecto.", y)
y = bloque(c, "<b>Siguiente etapa:</b> conectar el núcleo probado con los controles Windows Forms y la exportación de resultados, actividades del Entregable 4.", y)
c.showPage()
c.save()
print(SALIDA)
