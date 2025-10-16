using Framework.Core;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Services.Interfaces
{
    public interface IUserAppService
    {
        public string CurrentUserName { get; }        
        public Guid? CurrentUserId { get; }        
        public string CurrentUserRole { get; }
        public List<string> CurrentUserRoles { get; }

        public string GetCurrentUserWatermarkText();
        public Task<ApiResponse<UserDto>> GetAsync(Guid id);

        public Task<ApplicationUser> GetUserBasicAsync(Guid id);

        public Task<UserSearchResultDto> GetList(UserSearchDto model);

        public Task<bool> ChangePassword(ChangePasswordRequestDto model);
        public List<ADUserCreateDto> AutoCompleteADUsers(string SearchText);
        public Task<ApiResponse<LoginResult>> UserLogin(LoginDto input);
        public Task<ApiResponse<LoginResult>> SendOTPCode(string UserName, string token);
        public Task<ApiResponse<LoginResult>> SendActivationLink(string UserName);
        public Task<ApiResponse<LoginResult>> ConfirmActivationLink(UserVerificationDto UserVerification);
        public Task<ApiResponse<LoginResult>> SendResetPasswordLink(string UserName);
        public Task<ApiResponse<LoginResult>> ResetUserPassword(ResetPasswordDto ResetPasswordObj);
        public Task<ApiResponse<EmailMobileVerificationDto>> SendEmailMobileOTP(EmailMobileVerificationDto VerificationObj);
        public Task<ApiResponse<bool>> ConfirmEmailMobileOTP(EmailMobileVerificationDto VerificationObj);
        public CapthcaDto GetRandomCaptchaImg();

        public Task<UserDto> FindByIdAsync(Guid? id);

        public Task<List<RoleDto>> GetRolesAsync(Guid id);

        public Task<string> GetRoleDisplayName(string roleName);

        Task<List<SelectListItem>> GetAvailableUserRoles(Guid id);

        public Task<List<string>> GetRolesDisplayNameAsync(Guid id);

        public Task<ApiResponse<UserDto>> UpdateAsync(UserAddEditDto input);

        //public Task UpdateEmailAsync(string Email, string UserId);

        //public Task ConfirmNewEmailAsync(string userId);

        //public Task UpdatePhoneNumberAsync(string PhoneNumber, string Code, Guid? userId);

        public Task<bool> DeleteAsync(Guid id);
        public Task<bool> ChangeStatusAsync(Guid UserId);

        public Task AddRolesAsync(Guid id, string[] roleNames);

        public Task AddRoleAsync(ApplicationUser User, Guid roleId);

        public Task<UserDto> FindByUsernameAsync(string username);

        public Task<ApplicationUser> FindByUsername(string username);

        public Task<List<ApplicationUser>> FindByUsernames(List<string> usernames);

        public Task<List<ApplicationUser>> FindByIds(List<Guid> ids);

        public Task<UserDto> FindByEmailAsync(string email);

        public Task<UserDto> FindByEmailOrUsernameAsync(string username);
        public Task<List<Guid>> GetUserIdsByName(string name, bool useCulture = true);
        public Task<List<string>> GetUsernamesByName(string name, bool useCulture = true);

        public Task<UserDto> FindByPhoneAsync(string phone);

        public Task<List<UserDto>> FindAllByRoleNameAsync(string roleName, bool activeOnly = false);

        public Task<List<UserDto>> FindAllByRoleIdAsync(Guid roleId, bool activeOnly = false);

        public string GeneratePasswordToken(Guid id);

        public bool ValidateToken(Guid id, string token);

        public Task<ApiResponse<ApplicationUser>> CreateAsync(UserAddEditDto input);

        public string CreatePassword(int length);

        public Task<bool> IsUserInRoleAsync(Guid userId, string roleName);

        public Task<List<UserDto>> GetUsersInRoles(List<string> roleNames);

        public Task<string> GetUserNamesInRole(string roleName);

        public Task<ApiResponse> ValidateUserIfExist(UserCreateOrUpdateDtoBase input, Guid? id = null);

        public Task<string> GenerateMobileToken(Guid id);

        public Task<bool> ValidateMobileToken(Guid id, string token);

        public Task<string> GenerateEmailToken(Guid id);

        public Task<bool> ValidateEmailToken(Guid id, string token);

        public Task<string> GeneratePasswordResetTokenAsync(UserDto user);

        public Task<IdentityResult> ResetPasswordAsync(UserDto user, string token, string password);

        public Task<ApiResponse<LoginResult>> ConfirmOTPCodeAsync(UserVerificationDto model);

        public Task<bool> ValidateADUser(string userName, string password);

        public ADUserCreateDto FindUser(string userName);

        public IEnumerable<SelectListItem> GetApplicationRoles();

        public Task<bool> RemoveUserFromRole(Guid userId, Guid roleId);

        public Task<bool> RemoveUserFromRole(Guid userId, string roleName);

        Task<IdentityResult> AddRoleClaim(Guid userId, Guid roleId);

        public string GetClaimValueByKey(string Key);

        Task<Guid?> GetCurrentUserId();
        Task<ApiResponse<UserVM>> SamlUserLogin(string UserNameOrEmail);
        Task<ApiResponse<string>> SamlAuthenticateAsync(string UserEmail);

    }
}
