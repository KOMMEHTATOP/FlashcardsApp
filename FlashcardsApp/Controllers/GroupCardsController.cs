using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/groups/{groupId:guid}/cards")]
    [ApiController]
    [Authorize]
    public class GroupCardsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly CardService _cardService;

        public GroupCardsController(UserManager<User> userManager, CardService cardService)
        {
            _userManager = userManager;
            _cardService = cardService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCardsByGroupId(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _cardService.GetCardsByGroupAsync(groupId, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Guid groupId, CreateCardDto dto)
        {
            var userId = GetCurrentUserId();
            var newCard = await _cardService.CreateCardAsync(userId, groupId, dto);

            if (!newCard.IsSuccess)
            {
                return BadRequest(newCard.Errors);
            }

            return Created($"/api/cards/{newCard.Data!.CardId}", newCard.Data);
        }

        private Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            return Guid.Parse(userId);
        }
    }
}
