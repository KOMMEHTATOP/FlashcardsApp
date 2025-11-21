using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Study.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudyController : BaseController
{
    private readonly IStudyBL _studyBl;

    public StudyController(UserManager<User> userManager, IStudyBL studyBl) : base(userManager)
    {
        _studyBl = studyBl;
    }

    /// <summary>
    /// Записать результаты учебной сессии
    /// </summary>
    /// <param name="dto">Данные сессии</param>
    [HttpPost("record")]
    public async Task<IActionResult> RecordStudy([FromBody] RecordStudyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studyBl.RecordStudySessionAsync(userId, dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить историю учебных сессий
    /// </summary>
    /// <param name="limit">Ограничение количества записей (по умолчанию: 50, максимум: 1000)</param>
    [HttpGet("history")]
    public async Task<IActionResult> GetStudyHistory([FromQuery] int? limit = 50)
    {
        if (limit.HasValue && (limit.Value <= 0 || limit.Value > 1000))
        {
            return BadRequest(new
            {
                errors = new[] { "Параметр limit должен быть от 1 до 1000" }
            });
        }

        var userId = GetCurrentUserId();
        var result = await _studyBl.GetStudyHistoryAsync(userId, limit);

        return OkOrBadRequest(result);
    }
}
