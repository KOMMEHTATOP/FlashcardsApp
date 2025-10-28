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
        private readonly IGroupService _groupService;

        public GroupController(UserManager<User> userManager, IGroupService groupService):base(userManager)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.GetGroupsAsync(userId);

            return OkOrBadRequest(result);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGroup(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.GetGroupByIdAsync(id, userId);

            return OkOrBadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto dto)
        {
            var userId = GetCurrentUserId();
            var newGroup = await _groupService.CreateNewGroupAsync(dto, userId);

            if (!newGroup.IsSuccess)
            {
                return BadRequest(newGroup.Errors);
            }

            return CreatedAtAction(nameof(GetGroup), new
            {
                id = newGroup.Data!.Id
            }, newGroup.Data);
        }

        [HttpPut("{groupId:guid}")]
        public async Task<IActionResult> UpdateGroup(CreateGroupDto dto, Guid groupId)
        {
            var userId = GetCurrentUserId();
            var currentGroup = await _groupService.GetGroupByIdAsync(groupId, userId);

            if (!currentGroup.IsSuccess)
            {
                return BadRequest(currentGroup.Errors);
            }

            var updateResult = await _groupService.UpdateGroupAsync(groupId, userId, dto);

            return OkOrBadRequest(updateResult);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var userId = GetCurrentUserId();
            var currentGroup = await _groupService.DeleteGroupAsync(id, userId);
            if (!currentGroup.IsSuccess)
            {
                return BadRequest(currentGroup.Errors);
            }
            
            return NoContent();
        }
        
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderGroups([FromBody] List<ReorderGroupDto> groupOrders)
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.UpdateGroupsOrderAsync(groupOrders, userId);
    
            return OkOrBadRequest(result);
        }
    }
}
