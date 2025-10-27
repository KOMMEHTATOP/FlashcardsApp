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
public class StudyController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IStudyService _studyService;

    public StudyController(UserManager<User> userManager, IStudyService studyService)
    {
        _userManager = userManager;
        _studyService = studyService;
    }

    [HttpPost("record")]
    public async Task<IActionResult> RecordStudy([FromBody] RecordStudyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _studyService.RecordStudySessionAsync(userId, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetStudyHistory([FromQuery] int? limit)
    {
        var userId = GetCurrentUserId();
        var result = await _studyService.GetStudyHistoryAsync(userId, limit);

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
