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

        public GroupController(UserManager<User> userManager, IGroupBL groupBl):base(userManager)
        {
            _groupBl = groupBl;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var userId = GetCurrentUserId();
            var result = await _groupBl.GetGroupsAsync(userId);

            return OkOrBadRequest(result);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGroup(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _groupBl.GetGroupByIdAsync(id, userId);

            return OkOrBadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto dto)
        {
            var userId = GetCurrentUserId();
            var newGroup = await _groupBl.CreateNewGroupAsync(dto, userId);

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
            var currentGroup = await _groupBl.GetGroupByIdAsync(groupId, userId);

            if (!currentGroup.IsSuccess)
            {
                return BadRequest(currentGroup.Errors);
            }

            var updateResult = await _groupBl.UpdateGroupAsync(groupId, userId, dto);

            return OkOrBadRequest(updateResult);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var userId = GetCurrentUserId();
            var currentGroup = await _groupBl.DeleteGroupAsync(id, userId);
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
            var result = await _groupBl.UpdateGroupsOrderAsync(groupOrders, userId);
    
            return OkOrBadRequest(result);
        }
    }
}
