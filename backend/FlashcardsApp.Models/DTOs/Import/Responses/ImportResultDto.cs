namespace FlashcardsApp.Models.DTOs.Import.Responses;

public class ImportResultDto
{
    // Информация о созданной группе
    public Guid GroupId { get; set; }
    public required string GroupName { get; set; }
    public required string GroupColor { get; set; }
    public required string GroupIcon { get; set; }
    public int Order { get; set; }
    
    // Статистика импорта
    public required ImportStatisticsDto Statistics { get; set; }
    
    // Детали по карточкам
    public required List<ImportedCardResultDto> CardResults { get; set; } = new();
}

public class ImportStatisticsDto
{
    public int TotalCards { get; set; }        // Сколько было в запросе
    public int SuccessfulCards { get; set; }   // Сколько создано
    public int FailedCards { get; set; }       // Сколько пропущено
    public int DuplicateCards { get; set; }    // Сколько было дубликатов
}

public class ImportedCardResultDto
{
    public bool IsSuccess { get; set; }
    public Guid? CardId { get; set; }          // Null если не создана
    public required string Question { get; set; }  // Для идентификации
    public string? ErrorMessage { get; set; }  // Причина ошибки
}
