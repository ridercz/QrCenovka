using System.Globalization;
using IbanNet.Builders;
using IbanNet.Registry;

namespace Altairis.QrCenovka.Services;

public record struct PriceTagInfo(string Name, decimal Price, string QrCodeData) {

    public static IEnumerable<PriceTagInfo> Generate(string accountNumber, string? defaultVarSymbol, string items) {
        if (string.IsNullOrWhiteSpace(items)) yield break;

        // Convert account number to IBAN
        var parts = accountNumber.Split(['-', '/'], StringSplitOptions.RemoveEmptyEntries);
        var iban = new IbanBuilder()
                .WithCountry("CZ", IbanRegistry.Default)
                .WithBankAccountNumber(parts.Length == 2 ? parts[0] : parts[0].PadLeft(6, '0') + parts[1].PadLeft(10, '0'))
                .WithBankIdentifier(parts[^1])
                .Build();

        // Split items by newline
        var lines = items.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        // Process line by line
        foreach (var itemLine in lines) {
            // Split line by semicolon or tab
            parts = itemLine.Split([';', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;

            // Get item name, price and variable symbol
            var name = parts[0].Trim();
            if (!decimal.TryParse(parts[1], out var price)) continue;
            var varSymbol = defaultVarSymbol;
            if (parts.Length > 2) {
                varSymbol = parts[2].Trim();
                if (varSymbol.Length < 1 || varSymbol.Length > 10 || !varSymbol.All(char.IsDigit)) varSymbol = defaultVarSymbol;
            }

            // Generate QR code data
            var qrCodeData = $"SPD*1.0*ACC:{iban}*AM:{price.ToString("0.00", CultureInfo.InvariantCulture)}*CC:CZK*PT:IP";
            if (!string.IsNullOrWhiteSpace(varSymbol)) qrCodeData += $"*X-VS:{varSymbol}";

            // Return result
            yield return new PriceTagInfo(name, price, qrCodeData);
        }
    }

}
