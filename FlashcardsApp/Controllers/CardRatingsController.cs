using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/cards/{cardId:guid}/ratings")]
    [ApiController]
    [Authorize]
    public class CardRatingsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly CardRatingService _cardRatingService;

        public CardRatingsController(UserManager<User> userManager, CardRatingService cardRatingService)
        {
            _userManager = userManager;
            _cardRatingService = cardRatingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRatings(Guid cardId)
        {
            var user = GetCurrentUserId();
            var cardRatings = await _cardRatingService.GetCardRatingsAsync(user, cardId);

            if (!cardRatings.IsSuccess)
            {
                return BadRequest(cardRatings.Errors);
            }

            return Ok(cardRatings.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating(Guid cardId, CreateCardRatingDto dto)
        {
            var user = GetCurrentUserId();
            var result = await _cardRatingService.CreateCardRatingAsync(cardId, user, dto);

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
