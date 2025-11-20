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
    public class SubscriptionsController : BaseController
    {
        private readonly ISubscriptionBL _subscriptionBL;

        public SubscriptionsController(
            ISubscriptionBL subscriptionBL,
            UserManager<User> userManager) 
            : base(userManager)
        {
            _subscriptionBL = subscriptionBL;
        }

        /// <summary>
        /// Получить детали публичной группы по ID. Используется для просмотра содержимого.
        /// </summary>
        [HttpGet("{groupId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicGroupDetails(Guid groupId)
        {
            var userId = GetUserIdOrEmpty(); 
            var result = await _subscriptionBL.GetPublicGroupDetailsAsync(groupId, userId);
            return OkOrNotFound(result);
        }

        /// <summary>
        /// Получить список публичных групп (Каталог)
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetPublicGroups(
            [FromQuery] string? search = null,
            [FromQuery] string sortBy = "date",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? tagId = null)
        {
            var userId = GetUserIdOrEmpty(); 
            var result = await _subscriptionBL.GetPublicGroupsAsync(userId, search, sortBy, page, pageSize, tagId);
            return OkOrBadRequest(result);
        }

        /// <summary>
        /// Получить карточки публичной группы для предпросмотра
        /// </summary>
        [HttpGet("public/{groupId:guid}/cards")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetPublicGroupCards(Guid groupId)
        {
            var result = await _subscriptionBL.GetPublicGroupCardsAsync(groupId);
            return OkOrBadRequest(result);
        }
        
        /// <summary>
        /// Получить группы, на которые подписан текущий пользователь
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.GetSubscribedGroupsAsync(userId);
            return OkOrBadRequest(result);
        }

        [HttpPost("{groupId:guid}/subscribe")]
        public async Task<IActionResult> SubscribeToGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.SubscribeToGroupAsync(groupId, userId);
            return OkOrNotFound(result);
        }

        [HttpDelete("{groupId:guid}/subscribe")]
        public async Task<IActionResult> UnsubscribeFromGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.UnsubscribeFromGroupAsync(groupId, userId);
            return NoContentOrBadRequest(result);
        }

        [HttpGet("{groupId:guid}/is-subscribed")]
        public async Task<IActionResult> IsSubscribed(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.IsSubscribedAsync(groupId, userId);
            return OkOrBadRequest(result);
        }

        [HttpGet("authors/{authorId:guid}/rating")]
        public async Task<IActionResult> GetAuthorRating(Guid authorId)
        {
            var result = await _subscriptionBL.GetAuthorRatingAsync(authorId);
            return OkOrNotFound(result);
        }
        
        [HttpGet("tags")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTags()
        {
            var result = await _subscriptionBL.GetTagsAsync();
            return OkOrBadRequest(result);
        }
        
        // Вспомогательный метод для безопасного получения ID
        // Если пользователь вошел - возвращает его ID.
        // Если гость - возвращает Guid.Empty (0000-000...), чтобы логика BL работала корректно.
        private Guid GetUserIdOrEmpty()
        {
            return User.Identity?.IsAuthenticated == true 
                ? GetCurrentUserId() 
                : Guid.Empty;
        }
        

    }
}