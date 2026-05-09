using NuGet.Packaging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class CertificadoService : ICertificadoService
    {
        static CertificadoService()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        public byte[] Generar(DatosCertificadosDto datos)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14).FontFamily("Helvetica"));

                    page.Content()
                        .Border(3)
                        .BorderColor(Colors.Blue.Darken3)
                        .Padding(30)
                        .Column(col =>
                        {
                            col.Spacing(20);

                            col.Item().AlignCenter()
                                .Text("CERTIFICADO DE PARTICIPACIÓN")
                                .FontSize(32).Bold().FontColor(Colors.Blue.Darken3);

                            col.Item().AlignCenter()
                                .Text("Votify otorga el presente certificado a")
                                .FontSize(14).Italic();

                            col.Item().AlignCenter()
                                .Text(datos.NombreParticipante)
                                .FontSize(28).Bold();

                            col.Item().AlignCenter().Text(txt =>
                            {
                                txt.AlignCenter();
                                txt.Span("por su participación en el evento ");
                                txt.Span($"\"{datos.NombreEvento}\"").Bold();
                                txt.Span(", dentro de la categoría ");
                                txt.Span(datos.NombreCategoria).Bold();

                                if (!string.IsNullOrWhiteSpace(datos.NombreGrupo))
                                {
                                    txt.Span(", como integrante del grupo ");
                                    txt.Span(datos.NombreGrupo).Bold();
                                }

                                txt.Span(", obteniendo la posición ");
                                txt.Span($"#{datos.Posicion}").Bold().FontColor(Colors.Red.Darken2);
                                txt.Span(".");
                            });

                            col.Item().PaddingTop(40).AlignCenter()
                                .Text($"Finalizado el {datos.FechaFinalizacion:dd/MM/yyyy}")
                                .FontSize(12);

                            col.Item().AlignCenter()
                                .Text("— Equipo Votify —").Italic();
                        });
                });
            });

            return documento.GeneratePdf();
        }
    }
}
