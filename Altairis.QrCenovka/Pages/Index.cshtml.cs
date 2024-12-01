using System.ComponentModel.DataAnnotations;
using Altairis.QrCenovka.Services;
using Altairis.ValidationToolkit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Altairis.QrCenovka.Pages;

public class IndexModel : PageModel {

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel {
        [Required(ErrorMessage = "Musíte zadat èíslo úètu"), CzechBankAccount(ErrorMessage = "Zadané èíslo úètu není platné")]
        public string AccountNumber { get; set; } = string.Empty;

        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Variabilní symbol musí být 1-10 èíslic")]
        public string? VarSymbol { get; set; }

        [Required(ErrorMessage = "Musíte zadat alespoò jednu položku")]
        public string Items { get; set; } = string.Empty;

        public string Format { get; set; } = "A6";

    }

    public IActionResult OnPost() {
        // Validate input
        if (!this.ModelState.IsValid) return this.Page();

        // Generate price tag data
        var items = PriceTagInfo.Generate(this.Input.AccountNumber, this.Input.VarSymbol, this.Input.Items);

        // Prepare the right document type
        var doc = this.Input.Format switch {
            "A6" => new PriceTagDocumentA6(items) as IDocument,
            "A7" => new PriceTagDocumentA7(items),
            _ => throw new ArgumentOutOfRangeException(),
        };

        // Generate PDF
        var pdfBytes = doc.GeneratePdf();

        // Return result as file
        return this.File(pdfBytes, "application/pdf");
    }

}
