using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost("CreateRole"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> PostCreateRole([FromBody] JsonElement jsonElement)
    {
        string roleName = jsonElement.GetProperty("roleName").ToString();

        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name cannot be empty.");

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
            return Conflict("Role already exists.");

        var newRole = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(newRole);

        if (result.Succeeded)
            return Ok($"Role '{roleName}' created successfully.");
        else
            return BadRequest($"Error creating role: {string.Join(", ", result.Errors)}");
    }

    //[HttpPost("AssignTestRole"), Authorize]
    //public async Task AssignTestRoles()
    //{
    //    //var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

    //    //var userIdk = _userManager.GetUsersForClaimAsync(userId).Result.FirstOrDefault();


    //    //await _userManager.AddToRoleAsync(userIdk, "testRole");
    //}

    [HttpPost("AssignUserRole"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> PostAssignRoleToUser([FromBody] JsonElement jsonElement)
    {
        string userName = jsonElement.GetProperty("userName").ToString();
        string userRole = jsonElement.GetProperty("userRole").ToString();

        var currentUser = await _userManager.FindByNameAsync(userName);

        if (currentUser == null)
            return Unauthorized("User not authenticated.");

        var result = await _userManager.AddToRoleAsync(currentUser, userRole);

        if (result.Succeeded)
            return Ok($"User assigned to role successfully.");
        else
            return BadRequest($"Error assigning role: {string.Join(", ", result.Errors)}");
    }


    [HttpGet("GetUsersAndTheirRoles"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new
            {
                user.Id,
                user.UserName,
                user.Email,
                Role = roles.FirstOrDefault()
            });
        }

        return Ok(userRoles);
    }

    //[HttpGet("GetAllRoles")]
    [HttpGet("GetAllRoles"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();

        return Ok(roles);
    }
}