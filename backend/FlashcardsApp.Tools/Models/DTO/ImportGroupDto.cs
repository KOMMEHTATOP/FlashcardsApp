namespace FlashcardsApp.Tools.Models.DTO;

class ImportGroupDto
{
    public string GroupName { get; set; }
    public string? GroupColor { get; set; }
    public string? GroupIcon { get; set; }
    public List<string>? Tags { get; set; }
    public List<ImportCardDto>? Cards { get; set; }
}
