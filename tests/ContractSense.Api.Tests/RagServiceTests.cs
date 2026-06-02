using ContractSense.Api.Services;

namespace ContractSense.Api.Tests;

public class RagServiceTests
{
    [Fact]
    public async Task AskAsync_RejectsQuestionLongerThanLimit()
    {
        var ragService = new RagService(null!, null!);
        var veryLongQuestion = new string('a', 1001);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ragService.AskAsync(Guid.NewGuid(), veryLongQuestion, CancellationToken.None));

        Assert.Contains("under 1000 characters", exception.Message);
    }
}
