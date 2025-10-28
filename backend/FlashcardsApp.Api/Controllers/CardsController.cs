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
    public class CardsController : BaseController
    {
        private readonly ICardService _cardService;

        public CardsController(UserManager<User> userManager, ICardService cardService) : base(userManager)
        {
            _cardService = cardService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCards(int? rating = null)
        {
            var userId = GetCurrentUserId();
            var cards = await _cardService.GetAllCardsAsync(userId, rating);

            return OkOrBadRequest(cards);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCardById(Guid id)
        {
            var userId = GetCurrentUserId();
            var card = await _cardService.GetCardAsync(id, userId);

            return OkOrBadRequest(card);
        }


        [HttpPut("{cardId:guid}")]
        public async Task<IActionResult> UpdateCard(Guid cardId, CreateCardDto cardDto)
        {
            var userId = GetCurrentUserId();
            var updateResult = await _cardService.UpdateCardAsync(cardId, userId, cardDto);

            return OkOrBadRequest(updateResult);
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
    }
}
