using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.Models.DTOs.Cards.Requests;
using FlashcardsApp.Models.DTOs.Groups.Requests;
using FlashcardsApp.Models.DTOs.Import.Requests;
using FlashcardsApp.Models.DTOs.Import.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ImportBL : IImportBL
{
    private readonly ApplicationDbContext _context;
    private readonly IGroupBL _groupBL;
    private readonly ICardBL _cardBL;
    private readonly ILogger<ImportBL> _logger;

    public ImportBL(
        ApplicationDbContext context,
        IGroupBL groupBL,
        ICardBL cardBL,
        ILogger<ImportBL> logger)
    {
        _context = context;
        _groupBL = groupBL;
        _cardBL = cardBL;
        _logger = logger;
    }

    public async Task<ServiceResult<ImportResultDto>> ImportGroupWithCardsAsync(
        Guid userId, 
        ImportGroupWithCardsDto dto)
    {
        // ШАГ 1: Валидация уникальности имени группы
        var groupExists = await _context.Groups
            .AsNoTracking()
            .AnyAsync(g => g.UserId == userId && g.GroupName == dto.GroupName);

        if (groupExists)
        {
            _logger.LogWarning(
                "User {UserId} attempted to import group with duplicate name: {GroupName}",
                userId,
                dto.GroupName);
            
            return ServiceResult<ImportResultDto>.Failure(
                $"You already have a group named '{dto.GroupName}'");
        }

        // ШАГ 2: Начать транзакцию
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // ШАГ 3: Создать группу
            var createGroupDto = new CreateGroupDto
            {
                Name = dto.GroupName,
                Color = dto.GroupColor,
                GroupIcon = dto.GroupIcon,
                Order = dto.Order
            };

            var groupResult = await _groupBL.CreateGroupAsync(createGroupDto, userId);
            
            if (!groupResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                
                _logger.LogError(
                    "Failed to create group '{GroupName}' for user {UserId}: {Errors}",
                    dto.GroupName,
                    userId,
                    string.Join(", ", groupResult.Errors));
                
                return ServiceResult<ImportResultDto>.Failure(groupResult.Errors.ToArray());
            }

            var createdGroup = groupResult.Data;
            
            _logger.LogInformation(
                "Group {GroupId} created for import by user {UserId}",
                createdGroup.Id,
                userId);

            // ШАГ 4: Создать карточки с обработкой ошибок
            var cardResults = new List<ImportedCardResultDto>();
            int successfulCount = 0;
            int failedCount = 0;
            int duplicateCount = 0;

            foreach (var cardDto in dto.Cards)
            {
                try
                {
                    var createCardDto = new CreateCardDto
                    {
                        Question = cardDto.Question,
                        Answer = cardDto.Answer
                    };

                    var cardResult = await _cardBL.CreateCardAsync(
                        userId, 
                        createdGroup.Id, 
                        createCardDto);

                    if (cardResult.IsSuccess)
                    {
                        successfulCount++;
                        cardResults.Add(new ImportedCardResultDto
                        {
                            IsSuccess = true,
                            CardId = cardResult.Data.CardId,
                            Question = cardDto.Question,
                            ErrorMessage = null
                        });
                    }
                    else
                    {
                        failedCount++;
                        
                        // Объединяем все ошибки в одну строку
                        var errorMessage = string.Join("; ", cardResult.Errors);
                        
                        // Проверяем на дубликат
                        bool isDuplicate = cardResult.Errors.Any(e => 
                            e.Contains("already have a card with this question", 
                                      StringComparison.OrdinalIgnoreCase));
                        
                        if (isDuplicate)
                        {
                            duplicateCount++;
                        }

                        cardResults.Add(new ImportedCardResultDto
                        {
                            IsSuccess = false,
                            CardId = null,
                            Question = cardDto.Question,
                            ErrorMessage = errorMessage
                        });

                        _logger.LogWarning(
                            "Failed to import card '{Question}' for user {UserId}: {Errors}",
                            cardDto.Question,
                            userId,
                            errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    
                    cardResults.Add(new ImportedCardResultDto
                    {
                        IsSuccess = false,
                        CardId = null,
                        Question = cardDto.Question,
                        ErrorMessage = "Unexpected error occurred"
                    });

                    _logger.LogError(ex,
                        "Unexpected error importing card '{Question}' for user {UserId}",
                        cardDto.Question,
                        userId);
                }
            }

            // ШАГ 5: Коммит транзакции
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Import completed for user {UserId}: group '{GroupName}', {Successful}/{Total} cards created, {Failed} failed",
                userId,
                dto.GroupName,
                successfulCount,
                dto.Cards.Count,
                failedCount);

            // ШАГ 6: Формирование результата
            var result = new ImportResultDto
            {
                GroupId = createdGroup.Id,
                GroupName = createdGroup.GroupName,
                GroupColor = createdGroup.GroupColor,
                GroupIcon = createdGroup.GroupIcon,
                Order = createdGroup.Order,
                Statistics = new ImportStatisticsDto
                {
                    TotalCards = dto.Cards.Count,
                    SuccessfulCards = successfulCount,
                    FailedCards = failedCount,
                    DuplicateCards = duplicateCount
                },
                CardResults = cardResults
            };

            return ServiceResult<ImportResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            
            _logger.LogError(ex,
                "Critical error during import for user {UserId}, group '{GroupName}'",
                userId,
                dto.GroupName);
            
            return ServiceResult<ImportResultDto>.Failure(
                "An unexpected error occurred during import");
        }
    }
}