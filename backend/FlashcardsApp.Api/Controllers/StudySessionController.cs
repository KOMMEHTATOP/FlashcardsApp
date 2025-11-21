using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
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

        /// <summary>
        /// Начать новую учебную сессию
        /// </summary>
        /// <param name="groupId">ID группы для изучения</param>
        /// <param name="useDefaultSettings">Параметр не используется (deprecated, оставлен для совместимости с фронтендом)</param>
        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromQuery] Guid groupId, [FromQuery] bool useDefaultSettings = false)
        {
            var userId = GetCurrentUserId();
            var result = await _studySessionBl.StartSessionAsync(userId, groupId);

            return OkOrBadRequest(result); 
        }
    }
}
