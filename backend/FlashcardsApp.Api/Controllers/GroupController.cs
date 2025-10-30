using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : BaseController
    {
        private readonly IGroupBL _groupBl;

        public GroupController(UserManager<User> userManager, IGroupBL groupBl) : base(userManager)
        {
            _groupBl = groupBl;
        }

        /// <summary>
        /// Получить все группы пользователя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var userId = GetCurrentUserId();
            var result = await _groupBl.GetGroupsAsync(userId);

            return OkOrBadRequest(result);
        }

        /// <summary>
        /// Получить группу по ID
        /// </summary>
        /// <param name="groupId"></param>
        [HttpGet("{groupId:guid}")]
        public async Task<IActionResult> GetGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var result = await _groupBl.GetGroupByIdAsync(groupId, userId);

            return OkOrNotFound(result);
        }

        /// <summary>
        /// Создать новую группу
        /// </summary>
        /// <param name="dto">Данные группы</param>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var userId = GetCurrentUserId();
            var newGroup = await _groupBl.CreateGroupAsync(dto, userId);

            return OkOrBadRequest(newGroup);
        }

        /// <summary>
        /// Обновить группу
        /// </summary>
        /// <param name="groupId">ID группы</param>
        /// <param name="dto">Данные для обновления</param>
        [HttpPut("{groupId:guid}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] CreateGroupDto dto)
        {
            var userId = GetCurrentUserId();
            var updateResult = await _groupBl.UpdateGroupAsync(groupId, userId, dto);

            return OkOrNotFound(updateResult);
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        /// <param name="groupId"></param>
        [HttpDelete("{groupId:guid}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            var deleteResult = await _groupBl.DeleteGroupAsync(groupId, userId);

            return NoContentOrBadRequest(deleteResult);
        }

        /// <summary>
        /// Изменить порядок групп
        /// </summary>
        /// <param name="groupOrders">Список с новым порядком групп</param>
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderGroups([FromBody] List<ReorderGroupDto>? groupOrders)
        {
            if (groupOrders == null || groupOrders.Count == 0)
            {
                return BadRequest(new
                {
                    errors = new[] { "Список групп для переупорядочивания не может быть пустым" }
                });
            }

            var userId = GetCurrentUserId();
            var result = await _groupBl.UpdateGroupsOrderAsync(groupOrders, userId);

            return OkOrBadRequest(result);
        }
    }
}