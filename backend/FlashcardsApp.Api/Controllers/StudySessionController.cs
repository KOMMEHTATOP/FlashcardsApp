using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Study.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudySessionController : BaseController
    {
        private readonly IStudySessionBL _studySessionBl;

        public StudySessionController(IStudySessionBL studySessionBl, UserManager<User> userManager) : base(userManager)
        {
            _studySessionBl = studySessionBl;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromQuery] Guid groupId, [FromQuery] bool useDefaultSettings = false)
        {
            var userId = GetCurrentUserId();
            var result = await _studySessionBl.StartSessionAsync(userId, groupId);

            return OkOrBadRequest(result); 
        }
    }
}
