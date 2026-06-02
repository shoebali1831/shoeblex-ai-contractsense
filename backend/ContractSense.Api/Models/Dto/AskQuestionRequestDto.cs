using System.ComponentModel.DataAnnotations;

namespace ContractSense.Api.Models.Dto;

public class AskQuestionRequestDto
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required]
    [MinLength(3)]
    public string Question { get; set; } = string.Empty;
}
