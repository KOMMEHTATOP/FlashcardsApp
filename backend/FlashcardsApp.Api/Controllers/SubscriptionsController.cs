using FlashcardsApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
     
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionBL _subscriptionBL;

        public SubscriptionsController(ISubscriptionBL subscriptionBL)
        {
            _subscriptionBL = subscriptionBL;
        }

        /// <summary>
        /// Получить список публичных групп для подписки
        /// </summary>
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicGroups([FromQuery] string? search = null)
        {
            var result = await _subscriptionBL.GetPublicGroupsAsync(search);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

        /// <summary>
        /// Получить группы, на которые подписан текущий пользователь
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.GetSubscribedGroupsAsync(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

        /// <summary>
        /// Подписаться на группу
        /// </summary>
        [HttpPost("{groupId:guid}/subscribe")]
        public async Task<IActionResult> SubscribeToGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.SubscribeToGroupAsync(groupId, userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Отписаться от группы
        /// </summary>
        [HttpPost("{groupId:guid}/unsubscribe")]
        public async Task<IActionResult> UnsubscribeFromGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.UnsubscribeFromGroupAsync(groupId, userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Проверить, подписан ли текущий пользователь на группу
        /// </summary>
        [HttpGet("{groupId:guid}/is-subscribed")]
        public async Task<IActionResult> IsSubscribed(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.IsSubscribedAsync(groupId, userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

        /// <summary>
        /// Получить рейтинг автора
        /// </summary>
        [HttpGet("authors/{authorId:guid}/rating")]
        public async Task<IActionResult> GetAuthorRating(Guid authorId)
        {
            var result = await _subscriptionBL.GetAuthorRatingAsync(authorId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        }
    }
}