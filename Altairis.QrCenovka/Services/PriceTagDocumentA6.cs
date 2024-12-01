﻿using Net.Codecrete.QrCodeGenerator;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Altairis.QrCenovka.Services;

public class PriceTagDocumentA6(IEnumerable<PriceTagInfo> priceTags) : IDocument {
    private const int Padding = 5;

    public void Compose(IDocumentContainer container) {
        container.Page(p => {
            // Set page options
            p.Size(PageSizes.A4.Landscape());
            p.PageColor("#fff");
            p.Margin(0);
            p.DefaultTextStyle(s => s.FontFamily("Arial").FontColor("#000"));

            // Create content table
            p.Content().Table(t => {
                // Define two columns
                t.ColumnsDefinition(c => {
                    c.ConstantColumn(PageSizes.A6.Landscape().Width);
                    c.ConstantColumn(PageSizes.A6.Landscape().Width);
                });

                // Add cells
                foreach (var item in priceTags) ComposePriceTag(t.Cell().Height(PageSizes.A6.Landscape().Height - .5f).Padding(Padding, Unit.Millimetre), item);
            });
        });
    }

    private static void ComposePriceTag(IContainer container, PriceTagInfo info) {
        // Generate QR code
        var qrData = QrCode.EncodeText(info.QrCodeData, QrCode.Ecc.Medium);
        var qrSvg = qrData.ToSvgString(0);

        // Compose price tag
        container.Row(row => {
            row.RelativeItem().Column(col => {
                col.Item().PaddingRight(5, Unit.Millimetre).Text(info.Name).FontSize(16).Bold();
                col.Item().PaddingTop(5, Unit.Millimetre).Text($"{info.Price:N2} Kč").FontSize(20);
            });
            row.ConstantItem(50, Unit.Millimetre).Svg(qrSvg).FitArea();
        });
    }

}
