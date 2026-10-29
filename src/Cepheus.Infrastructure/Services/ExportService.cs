using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Models;
using ClosedXML.Excel;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace Cepheus.Infrastructure.Services
{
    /// <summary>
    /// Excel con ClosedXML y PDF con PDFsharp/MigraDoc.
    /// Nota de despliegue: en Windows, PDFsharp usa las fuentes instaladas en el sistema.
    /// Si algún día se despliega en Linux o en un contenedor, hay que registrar un
    /// IFontResolver propio (o instalar fuentes), de lo contrario el PDF fallará.
    /// </summary>
    public class ExportService : IExportService
    {
        private const string FormatoFecha = "dd/MM/yyyy HH:mm";

        static ExportService()
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        public byte[] ToExcel(string title, IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(NombreHoja(title));

            for (var c = 0; c < headers.Count; c++)
            {
                sheet.Cell(1, c + 1).Value = headers[c];
            }

            var encabezado = sheet.Range(1, 1, 1, headers.Count);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8EDF5");

            for (var r = 0; r < rows.Count; r++)
            {
                for (var c = 0; c < headers.Count; c++)
                {
                    EscribirCelda(sheet.Cell(r + 2, c + 1), rows[r][c]);
                }
            }

            sheet.SheetView.FreezeRows(1);

            if (rows.Count > 0)
            {
                sheet.Range(1, 1, rows.Count + 1, headers.Count).SetAutoFilter();
            }

            // El ajuste de ancho solo mide las primeras filas, para no hacerlo lento con muchos datos.
            sheet.Columns().AdjustToContents(1, Math.Min(rows.Count + 1, 200));

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ToPdf(ReporteInfo info, IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows)
        {
            const double anchoUtil = 26.7; // A4 apaisada (29.7 cm) con márgenes laterales de 1.5 cm

            var document = new Document();
            document.Info.Title = info.Titulo;

            var normal = document.Styles[StyleNames.Normal]!;
            normal.Font.Name = "Arial";
            normal.Font.Size = 8;

            var section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.Orientation = Orientation.Landscape;
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.TopMargin = Unit.FromCentimeter(2.4);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);
            section.PageSetup.HeaderDistance = Unit.FromCentimeter(0.9);
            section.PageSetup.FooterDistance = Unit.FromCentimeter(0.9);

            // ---- Encabezado de cada hoja: empresa (izq.) | título en negrita (centro) | página X de Y (der.) ----
            var cabecera = section.Headers.Primary.AddTable();
            var tercio = Unit.FromCentimeter(anchoUtil / 3);
            cabecera.AddColumn(tercio);
            cabecera.AddColumn(tercio);
            cabecera.AddColumn(tercio);

            var filaCabecera = cabecera.AddRow();
            filaCabecera.VerticalAlignment = VerticalAlignment.Center;
            filaCabecera.Borders.Bottom.Width = 0.5;
            filaCabecera.Borders.Bottom.Color = Colors.Gray;

            var empresa = filaCabecera.Cells[0].AddParagraph(info.Empresa);
            empresa.Format.Font.Size = 9;
            empresa.Format.Font.Bold = true;

            var titulo = filaCabecera.Cells[1].AddParagraph(info.Titulo);
            titulo.Format.Alignment = ParagraphAlignment.Center;
            titulo.Format.Font.Size = 13;
            titulo.Format.Font.Bold = true;

            var paginacion = filaCabecera.Cells[2].AddParagraph();
            paginacion.Format.Alignment = ParagraphAlignment.Right;
            paginacion.Format.Font.Size = 8;
            paginacion.AddText("Página ");
            paginacion.AddPageField();
            paginacion.AddText(" de ");
            paginacion.AddNumPagesField();

            // ---- Pie de cada hoja: usuario (centro) | fecha actual (der.) ----
            var pie = section.Footers.Primary.AddTable();
            pie.AddColumn(tercio);
            pie.AddColumn(tercio);
            pie.AddColumn(tercio);

            var filaPie = pie.AddRow();
            filaPie.VerticalAlignment = VerticalAlignment.Center;
            filaPie.Borders.Top.Width = 0.5;
            filaPie.Borders.Top.Color = Colors.Gray;

            var usuario = filaPie.Cells[1].AddParagraph($"Generado por: {info.GeneradoPor}");
            usuario.Format.Alignment = ParagraphAlignment.Center;
            usuario.Format.Font.Size = 7;

            var fecha = filaPie.Cells[2].AddParagraph(info.GeneradoEn.ToString(FormatoFecha));
            fecha.Format.Alignment = ParagraphAlignment.Right;
            fecha.Format.Font.Size = 7;

            // ---- Cuerpo ----
            if (!string.IsNullOrWhiteSpace(info.Subtitulo))
            {
                var sub = section.AddParagraph(info.Subtitulo);
                sub.Format.Font.Size = 8;
                sub.Format.Font.Color = Colors.Gray;
                sub.Format.SpaceAfter = Unit.FromCentimeter(0.3);
            }

            var table = section.AddTable();
            table.Borders.Width = 0.25;
            table.Borders.Color = Colors.Gray;
            table.Rows.LeftIndent = 0;
            table.LeftPadding = 3;
            table.RightPadding = 3;
            table.TopPadding = 2;
            table.BottomPadding = 2;

            // El ancho útil se reparte según el largo del contenido de cada columna.
            var anchos = CalcularAnchos(headers, rows, anchoUtil);
            foreach (var ancho in anchos)
            {
                table.AddColumn(Unit.FromCentimeter(ancho));
            }

            var filaEncabezado = table.AddRow();
            filaEncabezado.HeadingFormat = true;
            filaEncabezado.Format.Font.Bold = true;
            filaEncabezado.Shading.Color = Color.FromRgb(232, 237, 245);
            for (var c = 0; c < headers.Count; c++)
            {
                filaEncabezado.Cells[c].AddParagraph(headers[c]);
            }

            foreach (var fila in rows)
            {
                var fPdf = table.AddRow();
                for (var c = 0; c < headers.Count; c++)
                {
                    fPdf.Cells[c].AddParagraph(ATexto(fila[c]));
                }
            }

            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);
            return stream.ToArray();
        }

        private static void EscribirCelda(IXLCell celda, object? valor)
        {
            switch (valor)
            {
                case null:
                    break;
                case DateTime fecha:
                    celda.Value = fecha;
                    celda.Style.DateFormat.Format = FormatoFecha;
                    break;
                case bool booleano:
                    celda.Value = booleano;
                    break;
                case int entero:
                    celda.Value = entero;
                    break;
                default:
                    // Siempre como texto: un valor que empiece con "=" no se interpreta como fórmula.
                    celda.Value = valor.ToString() ?? string.Empty;
                    break;
            }
        }

        private static string ATexto(object? valor)
        {
            return valor switch
            {
                null => string.Empty,
                DateTime fecha => fecha.ToString(FormatoFecha),
                bool booleano => booleano ? "Sí" : "No",
                _ => valor.ToString() ?? string.Empty
            };
        }

        private static List<double> CalcularAnchos(IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows, double anchoTotal)
        {
            var pesos = new List<double>();

            for (var c = 0; c < headers.Count; c++)
            {
                var muestra = rows.Take(50).Select(f => ATexto(f[c]).Length).ToList();
                var promedio = muestra.Count > 0 ? muestra.Average() : 0;
                pesos.Add(Math.Clamp(Math.Max(headers[c].Length, promedio), 6, 40));
            }

            var suma = pesos.Sum();
            return pesos.Select(p => anchoTotal * p / suma).ToList();
        }

        private static string NombreHoja(string titulo)
        {
            var limpio = new string(titulo.Where(ch => !"[]:*?/\\".Contains(ch)).ToArray());
            return string.IsNullOrWhiteSpace(limpio) ? "Hoja1" : limpio[..Math.Min(limpio.Length, 31)];
        }
    }
}
