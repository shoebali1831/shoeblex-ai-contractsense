using System.Text.RegularExpressions;
using ContractSense.Api.Models.Entities;
using ContractSense.Api.Models.Internal;
using UglyToad.PdfPig;

namespace ContractSense.Api.Services;

public partial class PdfExtractionService : IPdfExtractionService
{
    public Task<List<PageText>> ExtractPagesAsync(Document document, CancellationToken cancellationToken)
    {
        var result = new List<PageText>();

        using var pdf = PdfDocument.Open(document.StoredFilePath);
        foreach (var page in pdf.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cleaned = CleanText(page.Text);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                continue;
            }

            result.Add(new PageText
            {
                PageNumber = page.Number,
                Text = cleaned
            });
        }

        if (result.Count == 0)
        {
            throw new InvalidOperationException("No extractable text was found in the PDF.");
        }

        return Task.FromResult(result);
    }

    private static string CleanText(string input)
    {
        var noCarriage = input.Replace("\r", " ");
        var squashedSpaces = MultiSpaceRegex().Replace(noCarriage, " ");
        var squashedLines = MultiLineRegex().Replace(squashedSpaces, Environment.NewLine);
        return squashedLines.Trim();
    }

    [GeneratedRegex("[\\t ]+")]
    private static partial Regex MultiSpaceRegex();

    [GeneratedRegex("\\n{2,}")]
    private static partial Regex MultiLineRegex();
}
