using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
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
        private readonly SignInManager<User> _signInManager;

        public GroupController(UserManager<User> userManager,  SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
    }
}
