using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNet8Authentication.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class LogoutTestController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly UserManager<IdentityUser> _userManager;
        

        public LogoutTestController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost("/logout"), Authorize]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            if (empty != null)
            {
                await _signInManager.SignOutAsync();
                return Ok(); // Equivalent to Results.Ok()
            }

            return Unauthorized(); // Equivalent to Results.Unauthorized()
        }
    }
}
