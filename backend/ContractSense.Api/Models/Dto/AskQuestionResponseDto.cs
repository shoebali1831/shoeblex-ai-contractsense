namespace ContractSense.Api.Models.Dto;

public class AskQuestionResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public List<int> SourcePages { get; set; } = [];
    public string Disclaimer { get; set; } = string.Empty;
}
