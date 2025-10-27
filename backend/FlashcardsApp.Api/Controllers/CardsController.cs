using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Cards.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardsController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly ICardService _cardService;

        public CardsController(UserManager<User> userManager, ICardService cardService)
        {
            _userManager = userManager;
            _cardService = cardService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCards(int? rating = null)
        {
            var userId = GetCurrentUserId();
            var cards = await _cardService.GetAllCardsAsync(userId, rating);

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


        [HttpPut("{cardId:guid}")]
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

        [HttpDelete("{cardId:guid}")]
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
