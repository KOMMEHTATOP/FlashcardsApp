using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Requests;
using FlashcardsApp.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudySettingsController : BaseController
{
    private readonly IStudySettingsService _studySettingsService;

    public StudySettingsController(IStudySettingsService studySettingsService, UserManager<User> userManager) :
        base(userManager)
    {
        _studySettingsService = studySettingsService;
    }

    /// <summary>
    /// Получить глобальные настройки
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.GetStudySettingsAsync(userId);

        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Сохранить глобальные настройки
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SaveSettings(CreateSettingsDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.SaveStudySettingsAsync(userId, dto);
        
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Сбросить к дефолтным настройкам
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> ResetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsService.ResetToDefaultAsync(userId);

        return OkOrBadRequest(result); 

    }
}
