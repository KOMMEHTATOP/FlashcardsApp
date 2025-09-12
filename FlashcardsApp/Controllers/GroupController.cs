using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly GroupService _groupService;

        public GroupController(UserManager<User> userManager, GroupService groupService)
        {
            _userManager = userManager;
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.GetGroupsAsync(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGroup(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.GetGroupByIdAsync(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Data);
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

            if (!updateResult.IsSuccess)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok(updateResult.Data);
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
        
        
        private Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }

            return Guid.Parse(userId);
        }
    }
}
