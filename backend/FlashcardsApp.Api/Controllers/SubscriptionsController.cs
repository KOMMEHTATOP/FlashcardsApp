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
        /// Получить список публичных групп для подписки
        /// </summary>
        /// <param name="search">Поисковый запрос (необязательный)</param>
        /// <param name="sortBy">Сортировка: date (новые), popular (популярные), name (по алфавиту)</param>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 20, макс 100)</param>
        /// <response code="200">Возвращает список публичных групп</response>
        /// <response code="400">Ошибка валидации или выполнения запроса</response>
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicGroups(
            [FromQuery] string? search = null,
            [FromQuery] string sortBy = "date",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _subscriptionBL.GetPublicGroupsAsync(search, sortBy, page, pageSize);
            return OkOrBadRequest(result);
        }

        /// <summary>
        /// Получить группы, на которые подписан текущий пользователь
        /// </summary>
        /// <response code="200">Возвращает список подписок пользователя</response>
        /// <response code="400">Ошибка выполнения запроса</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.GetSubscribedGroupsAsync(userId);
            return OkOrBadRequest(result);
        }

        /// <summary>
        /// Подписаться на группу
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <response code="200">Подписка успешно создана</response>
        /// <response code="400">Ошибка валидации (например, уже подписан)</response>
        /// <response code="404">Группа не найдена</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("{groupId:guid}/subscribe")]
        public async Task<IActionResult> SubscribeToGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.SubscribeToGroupAsync(groupId, userId);
            return OkOrNotFound(result);
        }

        /// <summary>
        /// Отписаться от группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <response code="204">Отписка выполнена успешно</response>
        /// <response code="400">Ошибка валидации</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpDelete("{groupId:guid}/subscribe")]
        public async Task<IActionResult> UnsubscribeFromGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.UnsubscribeFromGroupAsync(groupId, userId);
            return NoContentOrBadRequest(result);
        }

        /// <summary>
        /// Проверить, подписан ли текущий пользователь на группу
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <response code="200">Возвращает статус подписки (true/false)</response>
        /// <response code="400">Ошибка выполнения запроса</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("{groupId:guid}/is-subscribed")]
        public async Task<IActionResult> IsSubscribed(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _subscriptionBL.IsSubscribedAsync(groupId, userId);
            return OkOrBadRequest(result);
        }

        /// <summary>
        /// Получить рейтинг автора
        /// </summary>
        /// <param name="authorId">Идентификатор автора</param>
        /// <response code="200">Возвращает рейтинг автора</response>
        /// <response code="400">Ошибка выполнения запроса</response>
        /// <response code="404">Автор не найден</response>
        [HttpGet("authors/{authorId:guid}/rating")]
        public async Task<IActionResult> GetAuthorRating(Guid authorId)
        {
            var result = await _subscriptionBL.GetAuthorRatingAsync(authorId);
            return OkOrNotFound(result);
        }
        
        /// <summary>
        /// Получить карточки публичной группы для предпросмотра
        /// </summary>
        [HttpGet("public/{groupId:guid}/cards")]
        public async Task<IActionResult> GetPublicGroupCards(Guid groupId)
        {
            var result = await _subscriptionBL.GetPublicGroupCardsAsync(groupId);
            return OkOrBadRequest(result);
        }
    }
}