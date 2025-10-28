using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource");
            }

            return Guid.Parse(userId);
        }

        public IActionResult OkOrBadRequest<T>(ServiceResult<T> result)
        {
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }
    }
}
