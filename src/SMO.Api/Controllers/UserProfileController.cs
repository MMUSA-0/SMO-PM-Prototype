using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Identity.Data.Dtos;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// User Profile Management API Controller
    /// Handles user profile operations and role switching
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserAppService _userService;
        private readonly IRoleAppService _roleService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserAppService userService,
            IRoleAppService roleService,
            ILogger<UserProfileController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user profile with all assigned roles
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = _userService.CurrentUserId;
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId.Value.ToString());
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var roleDetails = new List<object>();

                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        roleDetails.Add(new
                        {
                            role.Id,
                            role.Name,
                            DisplayNameAr = role.DisplayNameAr,
                            DisplayNameEn = role.DisplayNameEn,
                            DescriptionAr = role.DescriptionAr,
                            DescriptionEn = role.DescriptionEn,
                            RoleGroup = role.RoleGroup,
                            IsActive = _userService.CurrentUserRole == roleName // Current active role
                        });
                    }
                }

                var profile = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    FullNameAr = user.FullNameAr,
                    FullNameEn = user.FullNameEn,
                    TitleAr = user.TitleAr,
                    TitleEn = user.TitleEn,
                    user.PhoneNumber,
                    user.DateOfBirth,
                    user.IdentityNo,
                    user.IsActive,
                    CurrentRole = _userService.CurrentUserRole,
                    Roles = roleDetails,
                    Programs = await GetUserPrograms(userId.Value), // Programs user is responsible for
                    Initiatives = await GetUserInitiatives(userId.Value) // Initiatives user manages
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { error = "Failed to retrieve user profile" });
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            try
            {
                var userId = _userService.CurrentUserId;
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId.Value.ToString());
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Update user information
                user.FullNameAr = model.FullNameAr ?? user.FullNameAr;
                user.FullNameEn = model.FullNameEn ?? user.FullNameEn;
                user.TitleAr = model.TitleAr ?? user.TitleAr;
                user.TitleEn = model.TitleEn ?? user.TitleEn;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
                user.UpdatedBy = user.UserName;
                user.UpdatedOn = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { error = "Failed to update profile" });
            }
        }

        /// <summary>
        /// Switch user's active role
        /// </summary>
        [HttpPost("switch-role")]
        public async Task<IActionResult> SwitchRole([FromBody] SwitchRoleDto model)
        {
            try
            {
                var userId = _userService.CurrentUserId;
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId.Value.ToString());
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if user has the requested role
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!userRoles.Contains(model.RoleName))
                {
                    return BadRequest(new { message = "User does not have access to this role" });
                }

                // Get role details
                var role = await _roleManager.FindByNameAsync(model.RoleName);
                if (role == null)
                {
                    return NotFound(new { message = "Role not found" });
                }

                // In a production system, you would typically:
                // 1. Invalidate current JWT token
                // 2. Generate new JWT with the new role claim
                // 3. Return the new token to the client
                // For now, we'll return success and let the client handle re-authentication

                _logger.LogInformation($"User {user.UserName} switched to role {model.RoleName}");

                return Ok(new
                {
                    message = "Role switched successfully",
                    newRole = new
                    {
                        role.Name,
                        DisplayNameAr = role.DisplayNameAr,
                        DisplayNameEn = role.DisplayNameEn,
                        role.RoleGroup
                    },
                    requiresReauth = true // Client should re-authenticate to get new token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error switching user role");
                return StatusCode(500, new { error = "Failed to switch role" });
            }
        }

        /// <summary>
        /// Get all available roles for assignment
        /// </summary>
        [HttpGet("available-roles")]
        [Authorize(Roles = "SuperAdmin,ExecutiveOffice")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var roles = _roleManager.Roles
                    .OrderBy(r => r.RoleGroup)
                    .ThenBy(r => r.DisplayNameEn)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        DisplayNameAr = r.DisplayNameAr,
                        DisplayNameEn = r.DisplayNameEn,
                        DescriptionAr = r.DescriptionAr,
                        DescriptionEn = r.DescriptionEn,
                        r.RoleGroup,
                        r.Code
                    });

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                return StatusCode(500, new { error = "Failed to retrieve roles" });
            }
        }

        /// <summary>
        /// Assign roles to a user (Admin only)
        /// </summary>
        [HttpPost("{userId}/assign-roles")]
        [Authorize(Roles = "SuperAdmin,ExecutiveOffice")]
        public async Task<IActionResult> AssignRoles(Guid userId, [FromBody] AssignRolesDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Remove all current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Add new roles
                if (model.RoleNames?.Any() == true)
                {
                    var result = await _userManager.AddToRolesAsync(user, model.RoleNames);
                    if (!result.Succeeded)
                    {
                        return BadRequest(new { errors = result.Errors });
                    }
                }

                _logger.LogInformation($"Roles updated for user {user.UserName}");
                return Ok(new { message = "Roles assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning roles");
                return StatusCode(500, new { error = "Failed to assign roles" });
            }
        }

        #region Private Helper Methods

        private async Task<List<object>> GetUserPrograms(Guid userId)
        {
            // In a real implementation, this would query the database for programs
            // that the user is responsible for based on their roles and assignments
            return await Task.FromResult(new List<object>
            {
                new { Id = 1, Name = "برنامج تطوير القطاع المالي", Status = "Active" },
                new { Id = 2, Name = "برنامج التحول الرقمي", Status = "Active" },
                new { Id = 3, Name = "برنامج جودة الحياة", Status = "Under Review" }
            });
        }

        private async Task<List<object>> GetUserInitiatives(Guid userId)
        {
            // In a real implementation, this would query the database for initiatives
            // that the user manages
            return await Task.FromResult(new List<object>
            {
                new { Id = 1, Name = "مبادرة التحول الرقمي للخدمات", Status = "In Progress" },
                new { Id = 2, Name = "مبادرة تطوير البنية التحتية", Status = "Planning" }
            });
        }

        #endregion
    }

    #region DTOs

    public class UpdateProfileDto
    {
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SwitchRoleDto
    {
        public string RoleName { get; set; }
    }

    public class AssignRolesDto
    {
        public List<string> RoleNames { get; set; }
    }

    #endregion
}


