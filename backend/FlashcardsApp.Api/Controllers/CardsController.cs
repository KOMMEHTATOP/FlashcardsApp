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
        private readonly ICardBL _cardBl;

        public CardsController(UserManager<User> userManager, ICardBL cardBl) : base(userManager)
        {
            _cardBl = cardBl;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCards(int? rating = null)
        {
            var userId = GetCurrentUserId();
            var cards = await _cardBl.GetAllCardsAsync(userId, rating);

            return OkOrBadRequest(cards);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCardById(Guid id)
        {
            var userId = GetCurrentUserId();
            var card = await _cardBl.GetCardAsync(id, userId);

            return OkOrBadRequest(card);
        }


        [HttpPut("{cardId:guid}")]
        public async Task<IActionResult> UpdateCard(Guid cardId, CreateCardDto cardDto)
        {
            var userId = GetCurrentUserId();
            var updateResult = await _cardBl.UpdateCardAsync(cardId, userId, cardDto);

            return OkOrBadRequest(updateResult);
        }

        [HttpDelete("{cardId:guid}")]
        public async Task<IActionResult> DeleteCard(Guid cardId)
        {
            var userId = GetCurrentUserId();
            var deleteResult = await _cardBl.DeleteCardAsync(cardId, userId);

            if (!deleteResult.IsSuccess)
            {
                return BadRequest(deleteResult.Errors);
            }

            return NoContent();
        }
    }
}
