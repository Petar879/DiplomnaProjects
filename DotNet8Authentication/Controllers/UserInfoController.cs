using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using NuGet.Protocol;
using System.Security.Claims;
using System.Text.Json;

namespace DotNet8Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserInfoController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet(Name = "GetAuthorizedUserIdAndRoles"), Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            //var user = await _userManager.FindByIdAsync(userId);

            //var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (userId == null)
            {
                return Unauthorized();
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                // Now 'roles' contains a list of role names associated with the user
                return Ok(new {UserId = userId, UserRole = roles.FirstOrDefault()});
            }
        }


        //[HttpPost("UpdateUserData"), Authorize(Roles = "Admin")]
        [HttpPost("UpdateUserData")]
        public async Task<IActionResult> GetUpdateUserData([FromBody] JsonElement jsonElement)
        {

            string userId = jsonElement.GetProperty("userId").ToString();
            string userName = jsonElement.GetProperty("userName").ToString();
            string email = jsonElement.GetProperty("email").ToString();
            string password = jsonElement.GetProperty("password").ToString();
            string role = jsonElement.GetProperty("role").ToString();

            var user = await _userManager.FindByIdAsync(userId);


            if (userName != string.Empty)
            {
                user.UserName = userName;
            }
            else if (email != string.Empty) 
            {
                user.Email = email;
            }
            else if (password != string.Empty)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, password);

                await _userManager.UpdateAsync(user);
            }
            else if (role != string.Empty)
            {
                var oldRole = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRoleAsync(user, oldRole.FirstOrDefault());
                var result = await _userManager.AddToRoleAsync(user, role);
                await _userManager.UpdateAsync(user);

            }
            await _userManager.UpdateAsync(user);

            return Ok($"User updated.");
        }


        //[HttpPost("DeleteUser"), Authorize(Roles = "Admin")]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] JsonElement jsonElement)
        {
            string userId = jsonElement.GetProperty("userId").ToString();

            var user = await _userManager.FindByIdAsync(userId);

            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deletion succeeded");
            }
            else 
            {
                return BadRequest("Something went wrong with user deletion");
            }
        }
    }
}
