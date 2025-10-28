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

    [HttpPost("record")]
    public async Task<IActionResult> RecordStudy([FromBody] RecordStudyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studyBl.RecordStudySessionAsync(userId, dto);

        return OkOrBadRequest(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetStudyHistory([FromQuery] int? limit)
    {
        var userId = GetCurrentUserId();
        var result = await _studyBl.GetStudyHistoryAsync(userId, limit);

        return OkOrBadRequest(result);
    }
}
