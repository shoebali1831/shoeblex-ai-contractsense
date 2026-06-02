using ContractSense.Api.Models.Entities;
using ContractSense.Api.Services;

namespace ContractSense.Api.Tests;

public class RagValueExtractionTests
{
    [Theory]
    [InlineData("what is the monthly rent", true)]
    [InlineData("how much is the payment per month", true)]
    [InlineData("summarize termination clause", false)]
    public void IsValueQuestion_DetectsValueIntent(string question, bool expected)
    {
        var actual = RagService.IsValueQuestion(question);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryExtractValueAnswer_ReturnsRentSentence_WhenAmountExists()
    {
        var chunks = new List<ContractChunk>
        {
            new()
            {
                PageNumber = 2,
                ChunkIndex = 0,
                Content = "The monthly rent is $2,500 and is due on the first day of each month."
            },
            new()
            {
                PageNumber = 3,
                ChunkIndex = 1,
                Content = "Tenant shall maintain the premises in good condition."
            }
        };

        var found = RagService.TryExtractValueAnswer(
            "what is the monthly rent",
            chunks,
            out var answer,
            out var pages);

        Assert.True(found);
        Assert.Contains("$2,500", answer);
        Assert.Single(pages);
        Assert.Equal(2, pages[0]);
    }

    [Fact]
    public void TryExtractValueAnswer_ReturnsFalse_WhenNoUsefulValueText()
    {
        var chunks = new List<ContractChunk>
        {
            new()
            {
                PageNumber = 5,
                ChunkIndex = 0,
                Content = "The lease commences on January 1 and ends on December 31."
            }
        };

        var found = RagService.TryExtractValueAnswer(
            "what is the monthly rent",
            chunks,
            out var answer,
            out var pages);

        Assert.False(found);
        Assert.Equal(string.Empty, answer);
        Assert.Empty(pages);
    }
}
