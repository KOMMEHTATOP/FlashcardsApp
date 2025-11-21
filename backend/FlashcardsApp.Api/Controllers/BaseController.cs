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

        /// <summary>
        /// Получает ID текущего авторизованного пользователя
        /// </summary>
        /// <returns>GUID пользователя</returns>
        /// <exception cref="UnauthorizedAccessException">Если пользователь не авторизован или ID имеет неверный формат</exception>
        protected Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new UnauthorizedAccessException("Invalid user ID format in token");
            }

            return parsedUserId;
        }

        /// <summary>
        /// Возвращает Ok если операция успешна, иначе BadRequest с ошибками
        /// </summary>
        protected IActionResult OkOrBadRequest<T>(ServiceResult<T> result)
        {
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Возвращает Ok если операция успешна, иначе NotFound если данные не найдены, иначе BadRequest
        /// </summary>
        protected IActionResult OkOrNotFound<T>(ServiceResult<T> result)
        {
            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => 
                    e.Contains("не найдено", StringComparison.OrdinalIgnoreCase) || 
                    e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound(new
                    {
                        errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Возвращает NoContent (204) если операция успешна, иначе BadRequest
        /// Используется для DELETE операций
        /// </summary>
        protected IActionResult NoContentOrBadRequest<T>(ServiceResult<T> result)
        {
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            return NoContent();
        }
        
        /// <summary>
        /// Возвращает Created (201) если операция успешна, иначе NotFound если ресурс не найден, иначе BadRequest
        /// Используется для POST операций, где может быть ошибка "не найдено" (например, родительский ресурс)
        /// </summary>
        /// <param name="result">Результат операции</param>
        /// <param name="locationUri">URI созданного ресурса (для заголовка Location)</param>
        protected IActionResult CreatedOrNotFound<T>(ServiceResult<T> result, string locationUri)
        {
            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e =>
                    e.Contains("не найдено", StringComparison.OrdinalIgnoreCase) ||
                    e.Contains("не найдена", StringComparison.OrdinalIgnoreCase) ||
                    e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound(new
                    {
                        errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            return Created(locationUri, result.Data);
        }
    }
}