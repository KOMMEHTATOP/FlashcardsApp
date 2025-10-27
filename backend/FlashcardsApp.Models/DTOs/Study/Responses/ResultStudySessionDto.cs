namespace FlashcardsApp.Models.DTOs.Study.Responses;

public class ResultStudySessionDto
{
    public required IEnumerable<StudyCardDto> Cards { get; set; }
}

