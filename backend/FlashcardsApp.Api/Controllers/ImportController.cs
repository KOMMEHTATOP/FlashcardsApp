using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Import.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ImportController : BaseController
{
    private readonly IImportBL _importBL;
    private readonly ILogger<ImportController> _logger;

    public ImportController(
        UserManager<User> userManager,
        IImportBL importBL,
        ILogger<ImportController> logger) : base(userManager)
    {
        _importBL = importBL;
        _logger = logger;
    }

    /// <summary>
    /// Импортировать группу с карточками
    /// </summary>
    /// <param name="dto">Данные группы и карточек</param>
    /// <returns>Результат импорта с детальной статистикой</returns>
    [HttpPost("group-with-cards")]
    public async Task<IActionResult> ImportGroupWithCards(
        [FromBody] ImportGroupWithCardsDto dto)
    {
        var userId = GetCurrentUserId();
        
        _logger.LogInformation(
            "User {UserId} started import: group '{GroupName}' with {CardCount} cards",
            userId,
            dto.GroupName,
            dto.Cards.Count);

        var result = await _importBL.ImportGroupWithCardsAsync(userId, dto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Import failed for user {UserId}: {Errors}",
                userId,
                string.Join(", ", result.Errors));
            
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation(
            "Import completed for user {UserId}: {Successful}/{Total} cards created",
            userId,
            result.Data.Statistics.SuccessfulCards,
            result.Data.Statistics.TotalCards);

        return Ok(result.Data);
    }
}
