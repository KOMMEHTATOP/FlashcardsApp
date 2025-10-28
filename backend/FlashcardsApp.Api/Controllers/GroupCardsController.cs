using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Cards.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/groups/{groupId:guid}/cards")]
    [ApiController]
    [Authorize]
    public class GroupCardsController : BaseController
    {
        private readonly ICardBL _cardBl;

        public GroupCardsController(UserManager<User> userManager, ICardBL cardBl):base(userManager)
        {
            _cardBl = cardBl;
        }


        [HttpGet]
        public async Task<IActionResult> GetCardsByGroupId(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _cardBl.GetCardsByGroupAsync(groupId, userId);

            return OkOrBadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Guid groupId,[FromBody] CreateCardDto dto)
        {
            var userId = GetCurrentUserId();
            var newCard = await _cardBl.CreateCardAsync(userId, groupId, dto);

            if (!newCard.IsSuccess)
            {
                return BadRequest(newCard.Errors);
            }

            return Created($"/api/cards/{newCard.Data!.CardId}", newCard.Data);
        }
    }
}
