using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardsController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly CardService _cardService;

        public CardsController(UserManager<User> userManager, CardService cardService)
        {
            _userManager = userManager;
            _cardService = cardService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var userId = GetCurrentUserId();
            var cards = await _cardService.GetAllCardsAsync(userId);

            if (!cards.IsSuccess)
            {
                return BadRequest(cards.Errors);
            }

            return Ok(cards.Data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCardById(Guid id)
        {
            var userId = GetCurrentUserId();
            var card = await _cardService.GetCardAsync(id, userId);

            if (!card.IsSuccess)
            {
                return BadRequest(card.Errors);
            }

            return Ok(card.Data);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCard(Guid cardId, CreateCardDto cardDto)
        {
            var userId = GetCurrentUserId();
            var updateResult = await _cardService.UpdateCardAsync(cardId, userId, cardDto);

            if (!updateResult.IsSuccess)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok(updateResult.Data);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCard(Guid cardId)
        {
            var userId = GetCurrentUserId();
            var deleteResult = await _cardService.DeleteCardAsync(cardId, userId);

            if (!deleteResult.IsSuccess)
            {
                return BadRequest(deleteResult.Errors);
            }

            return NoContent();
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
