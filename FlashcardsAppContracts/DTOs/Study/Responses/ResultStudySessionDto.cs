namespace FlashcardsAppContracts.DTOs.Responses;

public class ResultStudySessionDto
{
    public required IEnumerable<StudyCardDto> Cards { get; set; }
}

