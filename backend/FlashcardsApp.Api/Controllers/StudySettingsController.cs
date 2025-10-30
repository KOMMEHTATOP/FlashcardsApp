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
    private readonly IStudySettingsBL _studySettingsBl;

    public StudySettingsController(IStudySettingsBL studySettingsBl, UserManager<User> userManager) :
        base(userManager)
    {
        _studySettingsBl = studySettingsBl;
    }

    /// <summary>
    /// Получить глобальные настройки
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsBl.GetStudySettingsAsync(userId);

        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Сохранить глобальные настройки
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SaveSettings([FromBody] CreateSettingsDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsBl.SaveStudySettingsAsync(userId, dto);
        
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Сбросить к дефолтным настройкам
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> ResetSettings()
    {
        var userId = GetCurrentUserId();
        var result = await _studySettingsBl.ResetToDefaultAsync(userId);

        return NoContentOrBadRequest(result);
    }
}
