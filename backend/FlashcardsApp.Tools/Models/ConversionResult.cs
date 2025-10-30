namespace FlashcardsApp.Tools.Models;

public class ConversionResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Json { get; set; }
    public int TotalCards { get; set; }
    public string? GroupName { get; set; }
    public List<int> SkippedRows { get; set; } = new();
}
