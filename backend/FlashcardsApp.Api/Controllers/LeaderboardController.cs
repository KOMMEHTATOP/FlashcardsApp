using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderboardController : BaseController  
    {
        private readonly ILeaderboardBL _leaderboardBL;

        public LeaderboardController(UserManager<User> userManager, ILeaderboardBL leaderboardBL) : base(userManager)
        {
            _leaderboardBL = leaderboardBL;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard()  
        {
            var userId = GetCurrentUserId();
            var result = await _leaderboardBL.GetLeaderboardAsync(userId);
            return OkOrBadRequest(result);
        }
    }
}
