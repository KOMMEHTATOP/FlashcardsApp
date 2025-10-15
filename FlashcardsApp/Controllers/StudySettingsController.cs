using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudySettingsController : ControllerBase
{
    private readonly IStudySettingsService _studySettingsService;
    private readonly UserManager<User> _userManager;

    public StudySettingsController(IStudySettingsService studySettingsService, UserManager<User> userManager)
    {
        _studySettingsService = studySettingsService;
        _userManager = userManager;
    }

    /// <summary>
    /// Получить глобальные настройки
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ResultSettingsDto>> GetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.GetStudySettingsAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Сохранить глобальные настройки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResultSettingsDto>> SaveSettings(CreateSettingsDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.SaveStudySettingsAsync(userId, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Сбросить к дефолтным настройкам
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ResultSettingsDto>> ResetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.ResetToDefaultAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    private Guid GetCurrentUserId()
    {
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("You are not authorized to access this resource.");
        }

        return Guid.Parse(userId);
    }
}