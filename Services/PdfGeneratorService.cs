using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ProyectoPrograAvanzada.Models;
using System;
using System.IO;
using System.Linq;

namespace ProyectoPrograAvanzada.Services
{
    public static class PdfGeneratorService
    {
        public static byte[] GenerarRecibo(TAlquilere alquiler)
        {
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            // Título
            var titulo = new Paragraph("RECIBO DE ALQUILER")
                .SetFontSize(20);
            doc.Add(titulo);

            doc.Add(new Paragraph($"Cliente: {alquiler.IdClienteNavigation.Nombre}"));
            doc.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}"));

            doc.Add(new Paragraph("\nDETALLE\n"));

            foreach (var det in alquiler.TAlquileresDetalles)
            {
                doc.Add(new Paragraph(
                    $"Vehículo {det.IdVehiculo} | ₡{det.TarifaDiaria} por día " +
                    $"({det.FechaInicio} → {det.FechaFin})  =  ₡{det.Subtotal:0.00}"
                ));
            }

            decimal subtotal = alquiler.TAlquileresDetalles.Sum(x => x.Subtotal);
            decimal iva = subtotal * 0.13m;
            decimal total = subtotal + iva;

            doc.Add(new Paragraph($"\nSubtotal: ₡{subtotal:0.00}"));
            doc.Add(new Paragraph($"IVA 13%: ₡{iva:0.00}"));

            var totalParrafo = new Paragraph($"TOTAL: ₡{total:0.00}")
                .SetFontSize(14);
            doc.Add(totalParrafo);

            doc.Close();
            return ms.ToArray();
        }
    }
}
