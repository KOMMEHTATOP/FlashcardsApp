using FlashcardsApp.Models;
using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudySettingsController : ControllerBase
    {
        private readonly StudySettingsService _studySettingsService;
        private readonly UserManager<User> _userManager;

        public StudySettingsController(StudySettingsService studySettingsService, UserManager<User> userManager)
        {
            _studySettingsService = studySettingsService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<ResultSettingsDto>> GetSettings(Guid? groupId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _studySettingsService.GetStudySettingsAsync(currentUserId, groupId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ResultSettingsDto>> SaveSettings(CreateSettingsDto dto)
        {
            if (dto.MinRating > dto.MaxRating)
            {
                return BadRequest("MinRating cannot be greater than MaxRating");
            }

            var currentUserId = GetCurrentUserId();
            var result = await _studySettingsService.SaveStudySettingsAsync(currentUserId, dto);

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
