using Microsoft.AspNetCore.Hosting;
using ProyectoPrograAvanzada.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace ProyectoPrograAvanzada.Services
{
    public class PdfGeneratorService
    {
        private readonly IWebHostEnvironment _env;

        public PdfGeneratorService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerarRecibo(TAlquilere alquiler)
        {
            // Ruta física al logo dentro de wwwroot
            var logoPath = Path.Combine(_env.WebRootPath, "images", "logo.png");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // ===========================================================
                    // ENCABEZADO
                    // ===========================================================
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("RECIBO DE ALQUILER")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            col.Item().Text($"Fecha del recibo: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken2);
                        });

                        // Imagen con ruta física, NO ruta virtual
                        if (File.Exists(logoPath))
                        {
                            row.ConstantItem(70).Height(70)
                                .Image(logoPath, ImageScaling.FitWidth);
                        }
                        else
                        {
                            row.ConstantItem(70).Height(70)
                                .Text("Sin logo").FontColor(Colors.Grey.Medium);
                        }
                    });

                    // ===========================================================
                    // CONTENIDO
                    // ===========================================================
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Info del cliente
                        col.Item().Text($"Cliente: {alquiler.IdClienteNavigation.Nombre}")
                            .FontSize(14).Bold();
                        col.Item().Text($"Factura N°: {alquiler.IdAlquiler}").FontSize(12);
                        col.Item().Text("\n");


                        // -----------------------------
                        // DETALLE DEL ALQUILER
                        // -----------------------------
                        col.Item().Text("DETALLE DEL ALQUILER")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Grey.Darken1);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Tabla con detalles
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Encabezados
                            table.Header(header =>
                            {
                                header.Cell().Text("Vehículo").Bold();
                                header.Cell().Text("Fechas").Bold();
                                header.Cell().Text("Tarifa diaria").Bold();
                                header.Cell().Text("Subtotal").Bold();
                            });

                            // Filas dinámicas
                            foreach (var det in alquiler.TAlquileresDetalles)
                            {
                                table.Cell().Text($"#{det.IdVehiculo}").FontSize(12);
                                table.Cell().Text($"{det.FechaInicio:dd/MM/yyyy} → {det.FechaFin:dd/MM/yyyy}");
                                table.Cell().Text($"₡{det.TarifaDiaria:0.00}");
                                table.Cell().Text($"₡{det.Subtotal:0.00}");
                            }
                        });

                        col.Item().PaddingTop(15);


                        // -----------------------------
                        // TOTALES
                        // -----------------------------
                        decimal subtotal = alquiler.TAlquileresDetalles.Sum(x => x.Subtotal);
                        decimal iva = subtotal * (alquiler.Iva / 100m);
                        decimal total = subtotal + iva;

                        col.Item().Text($"Subtotal: ₡{subtotal:0.00}")
                            .FontSize(12);

                        col.Item().Text($"IVA ({alquiler.Iva}%): ₡{iva:0.00}")
                            .FontSize(12);

                        col.Item().AlignRight().Text($"TOTAL: ₡{total:0.00}")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);


                        // Separador y pie
                        col.Item().PaddingTop(20);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().AlignCenter().PaddingTop(5)
                            .Text("Gracias por utilizar nuestros servicios.")
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer().AlignCenter()
                        .Text("Documento generado automáticamente — Sistema de Alquileres")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken2);
                });
            });

            return document.GeneratePdf();
        }
    }
}
