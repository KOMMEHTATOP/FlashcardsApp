using FlashcardsApp.Models;
using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudySessionController : ControllerBase
    {
        private readonly StudySessionService _studySessionService;
        private readonly UserManager<User> _userManager;

        public StudySessionController(StudySessionService studySessionService, UserManager<User> userManager)
        {
            _studySessionService = studySessionService;
            _userManager = userManager;
        }

        [HttpPost("start")]
        public async Task<ActionResult<ResultStudySessionDto>> StartSession([FromQuery] Guid groupId, [FromQuery] bool useDefaultSettings = false)
        {
            var userId = GetCurrentUserId();
            var result = await _studySessionService.StartSessionAsync(userId, groupId, useDefaultSettings);

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
}
