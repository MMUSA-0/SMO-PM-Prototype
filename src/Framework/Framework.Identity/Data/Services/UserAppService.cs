using Framework.Core;
using Framework.Core.Angular;
using Framework.Core.AutoMapper;
using Framework.Core.Data.Repositories;
using Framework.Core.Extensions;
using Framework.Core.Globalization;
using Framework.Core.SharedServices.Services;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Repositories;
using Framework.Identity.Data.Services.Interfaces;
using Framework.Identity.Data.Services.Nafath.Dto;
using Framework.Identity.Recources;
using Framework.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PagedList.Core;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Framework.Identity.Data.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataProtectorTokenProvider<ApplicationUser> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppSettingsService _appSettingsService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserRoleAppService _userRoleAppService;
        private readonly RoleAppService _roleAppService;
        private readonly ActiveDirectoryHelperAppService _activeDirectoryAppService;
        private readonly NotificationService _notificationService;
        private readonly UserOtpAppService _userOtpAppService;
        private readonly JwtAuthAppService _jwtAuthAppService;
        private readonly UserTokensAppService _userTokensAppService;
        private readonly RoleRepository _roleRepository;
        private readonly UserRepository _userRepository;
        private readonly UserRolesRepository _userRolesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IRepositoryBase<AppIdentityDbContext, ApplicationUser> _applicationUserRepository;

        public UserAppService(UserManager<ApplicationUser> userManager,
            DataProtectorTokenProvider<ApplicationUser> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            RoleManager<ApplicationRole> roleManager,
            RoleRepository roleRepository,
            UserRolesRepository userRolesRepository,
            AppSettingsService appSettingsService,
            UserRepository userRepository,
            SignInManager<ApplicationUser> signInManager,
            UserRoleAppService userRoleAppService,
            NotificationService notificationService,
            JwtAuthAppService jwtAuthAppService,
            RoleAppService roleAppService,
            ActiveDirectoryHelperAppService activeDirectoryAppService, IHttpContextAccessor httpContextAccessor,
            IRepositoryBase<AppIdentityDbContext, ApplicationUser> applicationUserRepository,
            UserTokensAppService userTokensAppService, UserOtpAppService userOtpAppService, IConfiguration configuration)
        {
            _userManager = userManager;
            _dataProtectorTokenProvider = dataProtectorTokenProvider;
            _phoneNumberTokenProvider = phoneNumberTokenProvider;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _signInManager = signInManager;
            _roleRepository = roleRepository;
            _userRolesRepository = userRolesRepository;
            _appSettingsService = appSettingsService;
            _userRoleAppService = userRoleAppService;
            _activeDirectoryAppService = activeDirectoryAppService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _jwtAuthAppService = jwtAuthAppService;
            _roleAppService = roleAppService;
            _applicationUserRepository = applicationUserRepository;
            _userTokensAppService = userTokensAppService;
            _userOtpAppService = userOtpAppService;
            _configuration = configuration;
        }

        public string CurrentUserName => _httpContextAccessor?.HttpContext?.User?.Claims.Where(c => c.Type == "username").Select(c => c.Value).SingleOrDefault();
        public string CurrentUserRole => ((ClaimsIdentity)_httpContextAccessor?.HttpContext?.User?.Identity)?.FindAll(x => x.Type.Contains("role")).Select(x => x.Value).FirstOrDefault();        
        public List<string> CurrentUserRoles => ((ClaimsIdentity)_httpContextAccessor?.HttpContext?.User?.Identity)?.FindAll(x => x.Type.Contains("role")).Select(x => x.Value).ToList();
        public string CurrentUserFullname => _httpContextAccessor?.HttpContext?.User?.Claims.Where(c => c.Type == "fullname").Select(c => c.Value).SingleOrDefault();
        public string CurrentUserJTI => _httpContextAccessor?.HttpContext?.User?.Claims.Where(c => c.Type == "jti").Select(c => c.Value).SingleOrDefault();
        public string CurrentUserEmail => GetClaimValueByKey("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        public bool IsAdmin => CurrentUserRoleName == SystemUserRole.SuperAdmin.ToString();

        
        public bool IsCurrentUserInRole(params string[] roles) => CurrentUserRoles != null &&
            this.CurrentUserRoles.Any(r => roles.Contains(r));
        public bool IsCurrentUserRoleMatch(params string[] roles) => !string.IsNullOrEmpty(CurrentUserRoleName) &&
                    roles.Any(r => r.Contains(CurrentUserRoleName));
        
        public ApplicationUser CurrentUser
        {
            get
            {
                if (this._httpContextAccessor.HttpContext.Items["CurrentUser"] == null && CurrentUserName != null)
                {
                    var user = _userManager.FindByNameAsync(CurrentUserName).Result;
                    this._httpContextAccessor.HttpContext.Items["CurrentUser"] = user;
                }

                return this._httpContextAccessor.HttpContext.Items["CurrentUser"] as ApplicationUser;
            }
        }
        public Guid? CurrentUserId =>
            _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault()?.Value?.To<Guid?>();
        public Guid? CurrentUserRoleId =>
            _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(s => s.Type == "RoleId")?.Value?.To<Guid?>();
        public string CurrentUserRoleName =>
           _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(s => s.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value?.To<string>();


        public string GetCurrentUserWatermarkText()
        {
            return (CurrentUserName ?? _appSettingsService.DefaultPDFWatermarkText);
        }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetApplicationRoles() => _roleRepository.TableNoTracking.Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(CultureHelper.IsArabic ? r.DisplayNameAr : r.DisplayNameEn, r.Id.ToString())).ToList();
        public string GetClaimValueByKey(string Key)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                return _httpContextAccessor?.HttpContext?.User?.Claims?.Where(q => q.Type == Key).Select(q => q.Value).FirstOrDefault();
            }
            return null;
        }

        public async Task<ApplicationUser> GetUserBasicAsync(Guid id)
        {
            var result = await _userManager.FindByIdAsync(id.ToString());
            return result;
        }


        public async Task<ApiResponse<UserDto>> GetAsync(Guid id)
        {
            ApiResponse<UserDto> ApiResponse = new();

            var User = await _userManager.FindByIdAsync(id.ToString());
            var UserMapped = User.MapTo<UserDto>();
            if (UserMapped != null)
            {

                var UserRoles = await _userRoleAppService.GetRolesByUserIdAsync(UserMapped.Id);
                UserMapped.RoleNames = UserRoles.Select(x => x.Name).ToList();
                UserMapped.DisplayRoleNames = UserRoles.Select(x => CultureHelper.IsArabic ? x.DisplayNameAr : x.DisplayNameEn).ToList();
                ApiResponse.Success = true;
                ApiResponse.Value = UserMapped;
            }

            return ApiResponse;
        }
        public async Task<UserDto> FindByIdAsync(Guid? id)
        {
            var result = await _userManager.FindByIdAsync(id.ToString());
            return result.MapTo<UserDto>();
        }
        public async Task<UserSearchResultDto> GetList(UserSearchDto model)
        {
            UserSearchResultDto identityUserSearch = new();
            var filters = new List<Expression<Func<ApplicationUser, bool>>>();


            if (!model.ActivationStatus.IsNullOrEmpty())
            {
                filters.Add(u => u.IsActive.ToString().ToLower() == model.ActivationStatus.ToString().ToLower());
            }

            if (!model.UserName.IsNullOrEmpty())
            {
                filters.Add(u => u.NormalizedUserName.Contains(model.UserName.ToUpper()));
            }


            if (!model.FullName.IsNullOrEmpty())
            {
                filters.Add(u => u.FullNameAr.ToUpper().Contains(model.FullName.ToUpper())
                                || u.FullNameEn.Contains(model.FullName.ToUpper()));
            }

            if (!model.PhoneNumber.IsNullOrEmpty())
            {
                filters.Add(u => u.PhoneNumber.Contains(model.PhoneNumber));
            }


            if (!model.Email.IsNullOrEmpty())
            {
                filters.Add(u => u.NormalizedEmail.Contains(model.Email.ToUpper()));
            }

            if (model.RoleId != null)
            {
                filters.Add(a => a.UserRoles.Any(x => model.RoleId == x.RoleId));
            }
            else
            {
                var EmployeesRoles = await _roleAppService.GetEmployeesRoleList();
                var EmployeesRolesIds = EmployeesRoles.Select(x => x.Id);
                filters.Add(a => a.UserRoles.Any(x => EmployeesRolesIds.Contains(x.RoleId)));
            }

            //Export... 
            if (model.IsExport.HasValue && model.IsExport.Value)
            {
                identityUserSearch.ExportedItems = _userRepository.SearchAndSelectWithFilters(
                (b => b.OrderByDescending(a => a.CreatedOn)),
                a => new UsersListExcelDto()
                {
                    Id = a.Id,
                    FullName = CultureHelper.IsArabic ? a.FullNameAr : a.FullNameEn,
                    Email = a.Email,
                    Status = a.IsActive ? SharedResources.Active : SharedResources.Inactive,
                },
                filters
                );
                foreach (var item in identityUserSearch.ExportedItems)
                {
                    var UserRoles = await _userRoleAppService.GetRolesByUserIdAsync(item.Id.Value);
                    item.RoleName = UserRoles.Select(x => x.DisplayNameEn).FirstOrDefault();
                }
                return identityUserSearch;
            }
            model.PageSize ??= _appSettingsService.DefaultPagerPageSize;

            var result = _userRepository.SearchAndSelectWithFilters
                (
                model.PageNumber,
                model.IsExport.HasValue && model.IsExport.Value ? _appSettingsService.ExportNoOfItems : model.PageSize.Value,
                (b => b.OrderByDescending(a => a.CreatedOn)),
                 a => new UsersListDto()
                 {
                     Id = a.Id,
                     FullName = CultureHelper.IsArabic ? a.FullNameAr : a.FullNameEn,
                     UserName = a.UserName,
                     Email = a.Email,
                     PhoneNumber = a.PhoneNumber,
                     IsActive = a.IsActive
                 },
                filters
                );

            identityUserSearch.Items =
                new StaticPagedList<UsersListDto>(
                    result,
                    result.PageNumber,
                    result.PageSize,
                    result.TotalItemCount);

            foreach (var item in identityUserSearch.Items)
            {
                var UserRoles = await _userRoleAppService.GetRolesByUserIdAsync(item.Id);
                item.RoleNames = UserRoles.Select(x => CultureHelper.IsArabic ? x.DisplayNameAr : x.DisplayNameEn).ToList();
            }

            return await Task.FromResult(identityUserSearch);
        }

        public async Task<List<RoleDto>> GetRolesAsync(Guid id)
        {
            var roles = await _userRepository.GetRolesAsync(id);
            return roles.MapTo<List<RoleDto>>();
        }
        public async Task<IList<string>> GetRolesNamesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }
        public async Task<string> GetRoleDisplayName(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                return CultureHelper.IsArabic ? role.DisplayNameAr : role.DisplayNameEn;
            }
            else
            {
                await _signInManager.SignOutAsync();
            }
            return string.Empty;
        }
        public async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetAvailableUserRoles(Guid id)
        {
            var roles = await _userRepository.GetRolesAsync(id);
            return roles.Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = r.Id.ToString(),
                Text = CultureHelper.IsArabic ? r.DisplayNameAr : r.DisplayNameEn
            }).ToList();
        }

        public async Task<List<string>> GetRolesDisplayNameAsync(Guid id)
        {
            var roles = await _userRepository.GetRolesAsync(id);
            var rolesDto = roles.MapTo<List<RoleDto>>();
            return rolesDto.Select(r => r.DisplayName).ToList();
        }
        public async Task<ApiResponse<ApplicationUser>> CreateAsync(UserAddEditDto input)
        {
            ApiResponse<ApplicationUser> ApiResponse = new();
            var UserValidate = new UserCreateUpdateDto() { Email = input.Email, PhoneNumber = input.PhoneNumber, IdentityNo = input.IdentityNo };

            var domainUserData = _activeDirectoryAppService.GetUserFromActiveDirectory(input.UserName);
            if (domainUserData == null)
            {
                ApiResponse.Message = IdentityResources.HasNoAccount;
                return ApiResponse;
            }
            var ValidateUserResult = await ValidateUserIfExist(UserValidate);

            if (ValidateUserResult.Message.IsNotNullOrEmpty())
            {
                ApiResponse.Message = ValidateUserResult.Message;
                return ApiResponse;
            }

            ApplicationUser user = new(input.UserName.Trim().ToLower(), input.FullNameEn.Trim(),
                input.FullNameAr.Trim(), input.TitleEn.Trim(), input.TitleAr.Trim(),
                input.Email.Trim().ToLower(), input.DateOfBirth)
            {
                UserName = input.UserName,//(input.UserName == input.Email) ? input.UserName : input.Email,
                IdentityNo = input.IdentityNo,
                PhoneNumber = input.PhoneNumber,
                CreatedBy = CurrentUserName ?? input.UserName.ToPascalCase().Trim(),
                EmailConfirmed = true,
                FullNameEn = input.FullNameEn,
                FullNameAr = input.FullNameAr,
                TitleEn = input.TitleEn,
                TitleAr = input.TitleAr,
                DateOfBirth = input.DateOfBirth,
                IsActive = input.IsActive
            };

            if (input.Password.IsNullOrEmpty())
            {
                string Password = PasswordGenerator.Generate();
                input.Password = Password;
            }
            var UserManagerResult = await _userManager.CreateAsync(user, input.Password);

            if (!UserManagerResult.Errors.Any())
            {
                await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);
                await _userManager.SetTwoFactorEnabledAsync(user, input.TwoFactorEnabled);
                await _userManager.SetLockoutEnabledAsync(user, input.LockoutEnabled);

                if (input.RoleNames != null && input.RoleNames.Count > 0)
                {
                    List<UserRolesDto> userRoles = new();
                    foreach (var roleName in input.RoleNames)
                    {
                        var Role = await _roleRepository.FindByNameAsync(roleName);
                        userRoles.Add(new UserRolesDto
                        {
                            RoleId = Role.Id,
                            UserId = user.Id,
                            CreatedBy = CurrentUserName ?? "Portal"
                        });
                    }
                    await _userRoleAppService.InsertRangeAsync(userRoles, true);
                }


                ApiResponse.Success = true;
                ApiResponse.Value = user;
            }
            return ApiResponse;
        }
        public async Task<ApiResponse<UserDto>> UpdateAsync(UserAddEditDto input)
        {
            ApiResponse<UserDto> ApiResponse = new();
            var UserInput = new UserCreateUpdateDto() { Email = input.Email, PhoneNumber = input.PhoneNumber, IdentityNo = input.IdentityNo };

            var ValidateUserResult = await ValidateUserIfExist(UserInput, input.Id);
            if (!ValidateUserResult.Success && ValidateUserResult.Message.IsNotNullOrEmpty())
            {
                ApiResponse.Message = ValidateUserResult.Message;
                return ApiResponse;
            }

            var user = await _userRepository.FindByUserIdAsync(input.Id);

            if (user != null)
            {
                user.FullNameEn = input.FullNameEn.Trim();
                user.FullNameAr = input.FullNameAr.Trim();
                user.TitleEn = input.TitleEn.Trim();
                user.TitleAr = input.TitleAr.Trim();
                user.PhoneNumber = input.PhoneNumber.Trim();
                user.IdentityNo = input.IdentityNo;
                user.DateOfBirth = input.DateOfBirth;
                user.Email = input.Email.Trim();
                user.UpdatedBy = CurrentUserName;
                user.UpdatedOn = DateTime.Now;

                _userRepository.Update(user, true);
                await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);

                if (input.RoleNames != null && input.RoleNames.Count > 0)
                {
                    var Roles = await _roleAppService.GetRoleByNames(input.RoleNames);
                    _userRoleAppService.DeleteUserRoleNotAssigned(user.Id, Roles);

                    foreach (var Role in Roles)
                    {
                        if (!await IsUserInRoleAsync(user.Id, Role.Id))
                        {
                            await AddRoleAsync(user, Role.Id);
                        }
                    }
                }

                var mapped = user.MapTo<UserDto>();
                ApiResponse.Success = true;
                ApiResponse.Value = mapped;
            }
            return ApiResponse;
        }

        //public async Task UpdateEmailAsync(string Email, string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user != null)
        //    {
        //        user.TempEmail = Email;
        //        await _userManager.UpdateAsync(user);
        //    }
        //}
        //public async Task ConfirmNewEmailAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user != null)
        //    {
        //        user.Email = user.TempEmail;
        //        user.TempEmail = null;
        //        await _userManager.UpdateAsync(user);
        //    }
        //}
        //public async Task UpdatePhoneNumberAsync(string PhoneNumber, string Code, Guid? userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId.ToString());
        //    if (user != null)
        //    {
        //        user.TempPhoneNumber = PhoneNumber;
        //        user.MobileVerificationNumber = Convert.ToInt32(Code);
        //        await _userManager.UpdateAsync(user);
        //    }
        //}

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeStatusAsync(Guid UserId)
        {

            var oldData = await _userRepository.GetByIdAsync(UserId);

            if (oldData != null)
            {
                oldData.IsActive = !oldData.IsActive;
                await _userRepository.UpdateAsync(oldData, true);
                return true;
            }
            return false;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequestDto model)
        {
            if (model.NewPassword != model.ConfirmPassword) throw new InvalidOperationException("Not Found Or Unauthorized");

            var user = await FindByEmailIdOrUsernameAsync(CurrentUserName) ?? throw new InvalidOperationException("Not Found Or Unauthorized");

            var isValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isValid) throw new InvalidOperationException("Invalid Password");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return true;
            }
            else throw new InvalidOperationException("Invalid Password");
        }

        public async Task AddRolesAsync(Guid id, string[] roleNames)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            List<UserRolesDto> userRoles = new();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                userRoles.Add(new UserRolesDto
                {
                    RoleId = role.Id,
                    UserId = user.Id,
                });
            }
            await _userRoleAppService.InsertRangeAsync(userRoles, true);
            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task AddRolesAsync(Guid id, List<Guid> UserRoles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            List<UserRolesDto> userRoles = new();
            foreach (var roleId in UserRoles)
            {
                userRoles.Add(new UserRolesDto
                {
                    RoleId = roleId,
                    UserId = user.Id,
                });

            }
            await _userRoleAppService.InsertRangeAsync(userRoles, true);
            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task AddRoleAsync(ApplicationUser User, Guid RoleId)
        {
            var role = await _roleRepository.GetByIdAsync(RoleId);
            if (role != null && role.Name.ToLower() != SystemUserRole.SuperAdmin.ToString().ToLower())
            {
                await _userRoleAppService.InsertAsync(new UserRolesDto
                {
                    CreatedBy = CurrentUserName,
                    RoleId = RoleId,
                    UserId = User.Id
                }, true);

                await _userManager.UpdateSecurityStampAsync(User);
            }
        }

        public async Task<UserDto> FindByUsernameAsync(string username)
        {
            if (username != null)
            {
                var user = await _userManager.FindByNameAsync(username);
                return user.MapTo<UserDto>();
            }
            return null;
        }

        public async Task<ApplicationUser> FindByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            return user;
        }

        public async Task<List<ApplicationUser>> FindByUsernames(List<string> usernames)
        {
            var users = await _userRepository.TableNoTracking.Where(a => usernames.Contains(a.UserName)).ToListAsync();
            return users;
        }

        public async Task<ApplicationUser> FindByUsernameAsNoTracking(string username)
        {
            var users = await _userRepository.TableNoTracking
                .SingleOrDefaultAsync(a => a.UserName.ToLower().Trim().Equals(username.ToLower().Trim()));
            return users;
        }

        public async Task<UserDto> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.MapTo<UserDto>();
        }

        public async Task<ApplicationUser> FindByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user;
        }

        public async Task<ApplicationUser> FindByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user;
        }

        public async Task<UserDto> FindByEmailOrUsernameAsync(string username)
        {
            var user = await _userRepository.TableNoTracking.FirstOrDefaultAsync(c => c.Email.ToLower().Trim() == username.ToLower().Trim() || c.UserName.ToLower().Trim() == username.ToLower().Trim());
            return user?.MapTo<UserDto>();
        }

        public async Task<ApplicationUser> FindByEmailIdOrUsernameAsync(string username)
        {
            return await _userRepository.TableNoTracking.FirstOrDefaultAsync(c => c.Email.ToLower().Trim() == username.ToLower().Trim() || c.UserName.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<UserDto> FindByPhoneAsync(string phone)
        {
            var user = await _userRepository.TableNoTracking.FirstOrDefaultAsync(a => a.PhoneNumber == phone);
            return user?.MapTo<UserDto>();
        }

        public async Task<ApplicationUser> FindByIdentityNoAsync(string identityNo)
        {
            var user = await _userRepository.TableNoTracking.FirstOrDefaultAsync(a => a.IdentityNo == identityNo);
            return user;
        }

        public async Task<List<UserDto>> FindAllByRoleNameAsync(string roleName, bool activeOnly = false)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            var userList = activeOnly ? users.Where(u => u.IsActive).ToList() : users.ToList();
            return userList.MapTo<List<UserDto>>();
        }

        public async Task<List<UserDto>> FindAllByRoleIdAsync(Guid roleId, bool activeOnly = false)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            var userList = activeOnly ? users.Where(u => u.IsActive).ToList() : users.ToList();
            return userList.MapTo<List<UserDto>>();
        }

        public async Task<List<Core.Angular.SelectListItem<Guid>>> GetUserListByRoleName(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            var UserList = users.Select(p => new Core.Angular.SelectListItem<Guid>
            {
                Value = p.Id,
                NameAr = p.FullNameAr,
                NameEn = p.FullNameEn,
            }).ToList();
            return UserList;
        }

        public async Task<List<Core.Angular.SelectListItem<Guid>>> GetUserList()
        {
            var users = await _userRepository.TableNoTracking.Select(p => new Core.Angular.SelectListItem<Guid>
            {
                Value = p.Id,
                NameAr = p.UserName,
                NameEn = p.UserName,
            }).ToListAsync();
            return users;
        }

        public string GeneratePasswordToken(Guid id)
        {
            var user = _userManager.FindByIdAsync(id.ToString()).Result;
            var token = _phoneNumberTokenProvider.GenerateAsync("Reset_Password", _userManager, user);
            return token.Result;
        }

        public async Task<string> GetUserNameByUserId(Guid userId)
        {
            return await _userRepository.TableNoTracking.Where(q => q.Id == userId).Select(q => q.UserName).FirstOrDefaultAsync();
        }

        public bool ValidateToken(Guid id, string token)
        {
            var user = _userManager.FindByIdAsync(id.ToString()).Result;
            var isValid = _phoneNumberTokenProvider.ValidateAsync("Reset_Password", token, _userManager, user);
            return isValid.Result;
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new();
            Random rnd = new();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var role = await _roleRepository.FindByNameAsync(roleName);
            return _userRolesRepository.TableNoTracking.Where(s => s.RoleId == role.Id && s.UserId == userId).Any();
        }

        public async Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return _userRolesRepository.TableNoTracking.Where(s => s.RoleId == roleId && s.UserId == userId).Any();
        }

        public async Task<List<UserDto>> GetUsersInRoles(List<string> roleNames)
        {
            var users = new List<UserDto>();
            foreach (var role in roleNames)
            {
                users.AddRange(await FindAllByRoleNameAsync(role, true));
            }
            users = users.Where(c => c.IsActive).ToList();
            return users;
        }

        public async Task<string> GetUserNamesInRole(string roleName)
        {
            var users = await FindAllByRoleNameAsync(roleName, true);
            var userNames = users.Select(s => s.UserName).JoinAsString(",");
            return userNames;
        }

        public async Task<ApiResponse<LoginResult>> UserLogin(LoginDto input)
        {
            var isValidCaptch = IsValidCaptcha(input.CaptchaText, input.CaptchaKey);
            if (!isValidCaptch)
            {
                return new ApiResponse<LoginResult>
                {
                    Value = new LoginResult
                    {
                        LoginStatus = LoginStatusEnum.EmailNotRegistered
                    },
                    Message = IdentityResources.InvalidCaptcha,
                    Success = false
                };
            }

            var isSimulation = _appSettingsService.IsSimulation;
            var simulationPassKey = _appSettingsService.SimulationPassKey;
            input.Password = DecryptUserInputPassword(input.Password);

            var userObj = await FindByIdentityOrEmailOrUsernameAsync(input.Email);
            var apiResponse = await UserLoginChecks(userObj, input, false);
            if (!apiResponse.Success) return apiResponse;

            var passHash = new PasswordHasher<string>();
            var verificationResult = passHash.VerifyHashedPassword(userObj.UserName, userObj.PasswordHash, input.Password);
            if (verificationResult != PasswordVerificationResult.Success && !(isSimulation && input.Password == simulationPassKey))
            {
                await InvalidAccessOfUser(userObj);
                apiResponse.Value.LoginStatus = LoginStatusEnum.AccountInValidToLogin;
                apiResponse.Message = IdentityResources.InvalidLogin;
                return apiResponse;
            }

            await ResetLockoutOfUser(userObj);

            // Send OTP Code
            string token = await GenerateCallBackToken(userObj, GenerationTokenEnum.LoginOTP);
            apiResponse = await SendOTPCode(userObj.UserName, token);
            return apiResponse;
        }

        private async Task<ApiResponse<LoginResult>> UserLoginChecks
            (ApplicationUser user, LoginDto loginDto, bool sendActivation = true)
        {
            LoginResult loginResult = new LoginResult();
            ApiResponse<LoginResult> apiResponse = new ApiResponse<LoginResult>();
            apiResponse.Value = loginResult;
            apiResponse.Success = true;

            //var isValidCaptch = IsValidCaptcha(loginDto.CaptchaText, loginDto.CaptchaKey);
            //if (!isValidCaptch)
            //{
            //    apiResponse.Value.LoginStatus = LoginStatusEnum.EmailNotRegistered;
            //    apiResponse.Message = IdentityResources.InvalidCaptcha;
            //    apiResponse.Success = false;
            //}
            //else 
            if (user == null)
            {
                apiResponse.Value.LoginStatus = LoginStatusEnum.EmailNotRegistered;
                apiResponse.Message = IdentityResources.InvalidLogin;
                apiResponse.Success = false;
            }
            else if (user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now)
            {
                apiResponse.Value.LoginStatus = LoginStatusEnum.EmailTemporaryBlocked;
                apiResponse.Message = IdentityResources.EmailTemporaryBlocked;
                apiResponse.Success = false;
            }
            else if (!user.IsActive)
            {                
                apiResponse.Value.LoginStatus = LoginStatusEnum.EmailNotActive;
                apiResponse.Message = IdentityResources.UserNotActivated;
                apiResponse.Success = false;
                //Send Activation Link via email
                if (sendActivation)
                {
                    var isSent = await SendActivationLinkGeneral(user);
                    if (isSent.Success)
                    {
                        apiResponse.Value.UserName = user.UserName;
                        apiResponse.Value.LoginStatus = LoginStatusEnum.EmailActivationSent;
                        apiResponse.Message = IdentityResources.UserIsNotActive;
                    }
                }
            }            
            else
            {
                apiResponse.Value.UserName = user.UserName;
                apiResponse.Value.LoginStatus = LoginStatusEnum.AccountValidToLogin;
            }

            return apiResponse;
        }


        public async Task<ApplicationUser> FindByIdentityOrEmailOrUsernameAsync(string username)
        {
            var user = await _userRepository.TableNoTracking
                .FirstOrDefaultAsync(c =>
                c.IdentityNo.ToLower().Trim() == username.ToLower().Trim()
                || c.Email.ToLower().Trim() == username.ToLower().Trim()
                || c.UserName.ToLower().Trim() == username.ToLower().Trim());
            return user;
        }
        
        public async Task<bool> UserLogout(string jti)
        {
            var isDelete = false;
            if (jti != null)
            {
                isDelete = await _userTokensAppService.DeleteByJTIAsync(jti);
            }

            return isDelete;
        }


        public async Task<bool> VerifyUserCredentials(ApplicationUser userObj, LoginDto input, List<RoleDto> roles)
        {
            var isSimulation = _appSettingsService.IsSimulation;
            var simulationPassKey = _appSettingsService.SimulationPassKey;
            var isValid = false;

            if (roles.Any(x => x.Name != SystemUserRole.SuperAdmin.ToString())
            && userObj.Email.ToLower().Contains(_appSettingsService.ActiveDirectoryDomainName))
            {
                var isADLoginValid = await ValidateADUser(input.Email, input.Password);
                if (isADLoginValid || (isSimulation && input.Password == simulationPassKey))
                    isValid = true;

            }
            else
            {
                var passHash = new PasswordHasher<string>();
                var verificationResult = passHash.VerifyHashedPassword(userObj.UserName, userObj.PasswordHash, input.Password);
                if (verificationResult == PasswordVerificationResult.Success || (isSimulation && input.Password == simulationPassKey))
                    isValid = true;
            }

            return isValid;
        }

        public async Task<ApiResponse> ValidateUserIfExist(UserCreateOrUpdateDtoBase input, Guid? id = null)
        {
            var ApiResponse = new ApiResponse();
            //Validate Create
            if (id == null)
            {
                if (input.Email.IsNotNullOrEmpty())
                {
                    var userByEmail = await _userManager.FindByEmailAsync(input.Email);
                    if (userByEmail != null)
                    {
                        ApiResponse.Message = IdentityResources.EmailAlreadyRegistered;
                        return ApiResponse;
                    }


                }
                if (input.PhoneNumber.IsNotNullOrEmpty())
                {
                    var userByPhone = _userRepository.TableNoTracking.FirstOrDefault(a => a.PhoneNumber != null && a.PhoneNumber == input.PhoneNumber);
                    if (userByPhone != null)
                    {
                        ApiResponse.Message = IdentityResources.PhoneNumberAlreadyExist;
                        return ApiResponse;
                    }
                }
                if (input.IdentityNo.IsNotNullOrEmpty())
                {
                    var userByIdentity = _userRepository.TableNoTracking.FirstOrDefault(a => a.IdentityNo != null && a.IdentityNo == input.IdentityNo);
                    if (userByIdentity != null)
                    {
                        ApiResponse.Message = IdentityResources.IdentityNumberAlreadyExist;
                        return ApiResponse;
                    }
                }
            }
            else //Validate Edit
            {
                var usersByEmail = _userRepository.TableNoTracking.Where(u => u.Email != null && u.Email == input.Email && u.Id != id.Value).ToList();
                if (usersByEmail.Any())
                {
                    ApiResponse.Message = IdentityResources.EmailAlreadyRegistered;
                    return ApiResponse;
                }
                if (input.PhoneNumber.IsNotNullOrEmpty())
                {
                    var userByPhone = _userRepository.TableNoTracking.Where(u => u.PhoneNumber != null && u.PhoneNumber == input.PhoneNumber && u.Id != id.Value).ToList();
                    if (userByPhone.Any())
                    {
                        ApiResponse.Message = IdentityResources.PhoneNumberAlreadyExist;
                        return ApiResponse;
                    }
                }
                if (input.IdentityNo.IsNotNullOrEmpty())
                {
                    var userByIdentityNo = _userRepository.TableNoTracking.Where(u => u.IdentityNo != null && u.IdentityNo == input.IdentityNo && u.Id != id.Value).ToList();
                    if (userByIdentityNo.Any())
                    {
                        ApiResponse.Message = IdentityResources.IdentityNumberAlreadyExist;
                        return ApiResponse;
                    }
                }
            }

            return ApiResponse;
        }

        public async Task<ClaimDto> GetClaimByUsername(string username)
        {
            ClaimDto claimUser = null;
            if (!string.IsNullOrEmpty(username))
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    claimUser = new ClaimDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullNameEn, //CultureHelper.IsArabic ? user.FullNameAr : user.FullNameEn,
                        FullNameAr = user.FullNameAr,
                        UserRole = await _userRoleAppService.GetClaimUserRolesList(user.Id)
                    };
                }
            }
            return claimUser;
        }

        //Tokens
        public async Task<string> GenerateMobileToken(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var token = await _phoneNumberTokenProvider.GenerateAsync("Reset_Password", _userManager, user);
            return token;
        }

        public async Task<bool> ValidateMobileToken(Guid id, string token)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var isValid = await _phoneNumberTokenProvider.ValidateAsync("Reset_Password", token, _userManager, user);
            return isValid;
        }

        public async Task<string> GenerateEmailToken(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var token = await _dataProtectorTokenProvider.GenerateAsync("Reset_Password", _userManager, user);
            return token;
        }

        public async Task<bool> ValidateEmailToken(Guid id, string token)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var isValid = await _dataProtectorTokenProvider.ValidateAsync("Reset_Password", token, _userManager, user);
            return isValid;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(UserDto user)
        {
            var userObject = await _userManager.FindByIdAsync(user.Id.ToString());
            return await _userManager.GeneratePasswordResetTokenAsync(userObject);
        }

        public async Task<UserVM> GenerateUserToken(ApplicationUser userObj, List<RoleDto> Roles, CancellationToken cancellationToken = default)
        {
            var token = await _jwtAuthAppService.GenerateJWTToken(userObj.UserName);
            var refreshToken = await _jwtAuthAppService.GenerateJWTRefreshToken(userObj.UserName);

            return new UserVM()
            {
                Id = userObj.Id,
                UserName = userObj.UserName,
                FullNameEn = userObj.FullNameEn,
                FullNameAr = userObj.FullNameAr,
                IdentityNo = userObj.IdentityNo,
                PhoneNumber = userObj.PhoneNumber,
                Email = userObj.Email,
                UserRoles = Roles.Select(r => r.Name)?.ToList(),
                RolesCodes = Roles.Select(r => r.Code)?.ToList(),
                Token = token.token,
                TokenExpiresOn = token.expiresOn,
                RefreshToken = refreshToken.token,
                RefreshTokenExpiresOn = refreshToken.expiresOn,
            };
        }


        public async Task<IdentityResult> ResetPasswordAsync(UserDto user, string token, string password)
        {
            var userObject = await _userManager.FindByIdAsync(user.Id.ToString());

            return await _userManager.ResetPasswordAsync(userObject, token, password);
        }

        public async Task<bool> ValidateADUser(string userEmail, string password)
        {
            bool isValid;
            try
            {
                isValid = _activeDirectoryAppService.ValidateADUser(userEmail, password);

                if (isValid)
                {
                    isValid = await FindByEmail(userEmail) != null;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        public ADUserCreateDto FindUser(string userName)
        {
            return _activeDirectoryAppService.GetUserFromActiveDirectory(userName);
        }

        public List<ADUserCreateDto> AutoCompleteADUsers(string SearchText)
        {

            return _activeDirectoryAppService.GetAllUsersBySearch(SearchText);
        }

        public async Task<List<ApplicationUser>> FindByIds(List<Guid> ids)
        {
            var users = await _userRepository.TableNoTracking.Where(a => ids.Contains(a.Id)).ToListAsync();
            return users;
        }

        public async Task<bool> RemoveUserFromRole(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                _userRoleAppService.DeleteByUserIdRoleId(user.Id, roleId);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveUserFromRole(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var role = await _roleManager.FindByNameAsync(roleName);
            if (user != null && role != null)
            {
                _userRoleAppService.DeleteByUserIdRoleId(user.Id, role.Id);
                return true;
            }
            return false;
        }

        public async Task<IdentityResult> AddRoleClaim(Guid userId, Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var claims = await _userManager.GetClaimsAsync(user);
            var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, claims);
            if (removeClaimsResult.Succeeded)
            {
                var result = await _userManager.AddClaimsAsync(user, new List<Claim> {
                new Claim("RoleId", roleId.ToString()),
                new Claim("RoleName", role.Name)
                });

                return result;
            }
            return null;
        }

        public async Task<string> GenerateVerificationCode(Guid? UserId, EmailMobileVerificationDto VerificationObj = null)
        {
            int generatedCode = CommonHelper.GenerateRandomInteger(0, 1000000);
            string code = generatedCode.ToString("D6");

            var CodeToArr = code.ToCharArray();
            if (CodeToArr[0] == '0')
            {
                CodeToArr[0] = '1';
            }
            code = string.Join("", CodeToArr);

            var userOtpDto = new UserOtpDto()
            {
                UserId = UserId == Guid.Empty ? null : UserId,
                Email = (VerificationObj != null && VerificationObj.Email.IsNotNullOrEmpty()) ? VerificationObj.Email : null,
                Mobile = (VerificationObj != null && VerificationObj.Mobile.IsNotNullOrEmpty()) ? VerificationObj.Mobile : null,
                Otp = code,
                CreatedBy = "System",
                UpdatedBy = "System",
                UpdatedOn = DateTime.Now
            };
            var Inserted = await _userOtpAppService.SaveAsync(userOtpDto, true);

            return Inserted ? code : null;
        }        

        public async Task<Guid?> GetCurrentUserId()
        {
            var UserDetails = await _userRepository.FindByUserNameAsync(CurrentUserName);
            return UserDetails?.Id;
        }

        public async Task<ApiResponse<LoginResult>> SendOTPCode(string UserName, string token)
        {
            ApiResponse<LoginResult> OTPResponse = new ApiResponse<LoginResult>();
            OTPResponse.Success = true;
            var userObj = await FindByUsername(UserName);
            if (userObj != null)
            {
                var tokenIsValid = await ValidateUserToken(token, GenerationTokenEnum.LoginOTP, userObj);
                if (!tokenIsValid)
                {
                    OTPResponse.Success = false;
                    OTPResponse.Message = SharedResources.InvalidRequestParametersError;
                    return OTPResponse;
                }

                var isUserOTPLimitValidToGenerateNew = await _userOtpAppService.IsUserOTPLimitValidToGenerateNew(userObj.Id);
                var isUserOTPDurationValidToGenerateNew = await _userOtpAppService.IsUserOTPDurationValidToGenerateNew(userObj.Id);

                if (!isUserOTPLimitValidToGenerateNew || !isUserOTPDurationValidToGenerateNew)
                {
                    OTPResponse.Success = false;
                    OTPResponse.Message = SharedResources.GenerateOTPLimitationOrDurationNotValid;
                    return OTPResponse;
                }

                string Code = await _userOtpAppService.GenerateVerificationCode(userObj.Id);

                if (!_appSettingsService.IsSmsSimulation)
                {
                    //bool IsEmailSent = await _notificationService.SendEmailWithCallBack(UserName, string.Empty, Code, EmailTemplateNames.Users_LoginOTPEmail.ToString());
                    var SMSResponse = await _notificationService.SendSMS(userObj.PhoneNumber, Code, SmsTemplateNames.Users_LoginOTPSMS.ToString());
                    Code = string.Empty;
                    OTPResponse.Success = SMSResponse.Success; // || IsEmailSent;
                }

                OTPResponse.Value = new LoginResult
                {
                    LoginStatus = LoginStatusEnum.AccountOtpSent,
                    UserName = userObj.UserName,
                    NationalId = userObj.IdentityNo.HidePartOfText(),
                    PhoneNumber = userObj.PhoneNumber.HidePartOfText(),
                    VerificationCode = _appSettingsService.IsSmsSimulation ? Code : ""
                };
            }
            else OTPResponse.Success = false;

            return OTPResponse;
        }

        public async Task<ApiResponse<LoginResult>> ConfirmOTPCodeAsync(UserVerificationDto model)
        {
            ApiResponse<LoginResult> ApiResponse = new();

            if (model.Code.IsNullOrEmpty() && !model.Code.All(char.IsDigit))
            {
                ApiResponse.Message = SharedResources.ErrorActivationCode;
                return ApiResponse;
            }

            var user = await FindByEmailIdOrUsernameAsync(model.UserName);
            if (user == null)
            {
                ApiResponse.Message = IdentityResources.InvalidLogin; //IdentityResources.UserNotFound;
                return ApiResponse;
            }

            var tokenIsValid = await ValidateUserToken(model.Token, GenerationTokenEnum.LoginOTP, user);
            if (!tokenIsValid)
            {
                ApiResponse.Message = IdentityResources.InvalidLogin; //IdentityResources.InvalidToken;
                return ApiResponse;
            }

            var UserOtp = await _userOtpAppService.GetUserOtpByUserId(user.Id);
            if (UserOtp == null || UserOtp.Otp != model.Code)
            {
                ApiResponse.Message = IdentityResources.InvalidLogin; //SharedResources.ErrorActivationCode;
                return ApiResponse;
            }

            await _userOtpAppService.DeleteUserOtp(user.Id);

            var userRoles = await GetRolesAsync(user.Id);
            var userToken = await GenerateUserToken(user, userRoles);

            ApiResponse.Success = true;

            ApiResponse.Value = new LoginResult
            {
                LoginStatus = LoginStatusEnum.AccountValidToLogin,
                UserName = user.UserName,
                Token = userToken,
                NationalId = user.IdentityNo.HidePartOfText(),
                PhoneNumber = user.PhoneNumber.HidePartOfText()
            };

            
            return ApiResponse;
        }

        // To follow-up user login attemps
        public async Task InvalidAccessOfUser(ApplicationUser user)
        {

            user.AccessFailedCount += 1;
            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.Now.AddMinutes(30);
            }

            user = await _userRepository.UpdateAsync(user, true);

        }

        public async Task ResetLockoutOfUser(ApplicationUser user)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user = await _userRepository.UpdateAsync(user, true);
        }

        public async Task<ApiResponse<LoginResult>> SendActivationLink(string UserName)
        {
            ApiResponse<LoginResult> ActivationResponse = new();

            var userObj = await FindByEmail(UserName);
            if (userObj != null)
            {
                ActivationResponse = await SendActivationLinkGeneral(userObj);
            }
            return ActivationResponse;
        }

        public async Task<ApiResponse<LoginResult>> ConfirmActivationLink(UserVerificationDto UserVerification)
        {
            ApiResponse<LoginResult> ConfirmAccountResponse = new();
            var User = await FindByEmail(UserVerification.UserName);
            if (User != null)
            {
                var newToken = GetDecodedToken(UserVerification.Token);
                var ConfirmEmailResult = await _userManager.ConfirmEmailAsync(User, newToken);
                if (ConfirmEmailResult.Succeeded && ConfirmEmailResult.Errors.Count() == 0)
                {
                    var userObj = await FindByEmail(UserVerification.UserName);
                    //update user to be active;
                    userObj.IsActive = true;
                    await _userManager.UpdateAsync(userObj);

                    ConfirmAccountResponse.Success = true;

                    ConfirmAccountResponse.Value = new LoginResult
                    {
                        //LoginStatus = User.IsFirstLogin ? LoginStatusEnum.ResetPasswordSent : LoginStatusEnum.EmailRegisteredAndActiveExternal,
                        LoginStatus = LoginStatusEnum.EmailRegisteredAndActiveExternal,
                        UserName = userObj.UserName
                    };
                    //if (User.IsFirstLogin)
                    //{
                    //    string token = await _userManager.GeneratePasswordResetTokenAsync(userObj);
                    //    var encodedToken = HttpUtility.UrlEncode(token);
                    //    ConfirmAccountResponse.Value.Token = encodedToken;
                    //}
                }
            }
            return ConfirmAccountResponse;
        }

        public async Task<ApiResponse<LoginResult>> SendResetPasswordLink(string UserName)
        {
            ApiResponse<LoginResult> ActivationResponse = new();

            var userObj = await FindByEmail(UserName);
            if (userObj != null)
            {
                if (userObj.IsActive)
                    ActivationResponse = await SendResetPasswordLinkGeneral(userObj);
                else
                    ActivationResponse = await SendActivationLinkGeneral(userObj);
            }
            else
                ActivationResponse.Message = IdentityResources.UserNotFound;

            return ActivationResponse;
        }

        public async Task<ApiResponse<LoginResult>> ResetUserPassword(ResetPasswordDto ResetPasswordObj)
        {
            ApiResponse<LoginResult> OTPResponse = new();

            var userObj = await FindByEmail(ResetPasswordObj.UserName);
            if (userObj != null)
            {

                var newToken = GetDecodedToken(ResetPasswordObj.Token);
                var ChangePasswordResult = await _userManager.ResetPasswordAsync(userObj, newToken, ResetPasswordObj.Password);

                if (ChangePasswordResult.Succeeded && ChangePasswordResult.Errors.Count() == 0)
                {
                    //userObj.IsFirstLogin = false;
                    await _userManager.UpdateAsync(userObj);

                    OTPResponse.Success = true;
                    OTPResponse.Value = new LoginResult
                    {
                        LoginStatus = LoginStatusEnum.EmailRegisteredAndActiveExternal,
                        UserName = userObj.UserName
                    };
                }
            }

            return OTPResponse;
        }

        private async Task<ApiResponse<LoginResult>> SendActivationLinkGeneral(ApplicationUser userObj)
        {
            ApiResponse<LoginResult> ActivationResponse = new();

            string CallBackURL = await GetCallBackURL(userObj, "accountActivation", GenerationTokenEnum.ActivateAccount);
            bool IsEmailSent = await _notificationService.SendEmailWithCallBack(userObj.Email, CultureHelper.IsArabic ? userObj.FullNameAr : userObj.FullNameEn,
                CallBackURL, EmailTemplateNames.Users_AccountActivationEmail.ToString());

            ActivationResponse.Success = IsEmailSent;
            ActivationResponse.Value = new LoginResult
            {
                UserName = userObj.UserName,
                LoginStatus = LoginStatusEnum.EmailActivationSent
            };
            return ActivationResponse;
        }

        private async Task<ApiResponse<LoginResult>> SendResetPasswordLinkGeneral(ApplicationUser userObj, bool isSendLink = true)
        {
            ApiResponse<LoginResult> ActivationResponse = new();

            string CallBackURL = await GetCallBackURL(userObj, "confirmResetPassword", GenerationTokenEnum.ResetPassword);
            if (isSendLink)
            {
                bool IsEmailSent = await _notificationService.SendEmailWithCallBack(userObj.Email,
                    CultureHelper.IsArabic ? userObj.FullNameAr : userObj.FullNameEn, CallBackURL,
                    EmailTemplateNames.Users_ResetPasswordEmail.ToString());
                ActivationResponse.Success = IsEmailSent;
            }

            ActivationResponse.Value = new LoginResult
            {
                UserName = userObj.UserName,
                LoginStatus = LoginStatusEnum.ResetPasswordSent
            };

            return ActivationResponse;
        }

        public async Task<ApiResponse<EmailMobileVerificationDto>> SendEmailMobileOTP(EmailMobileVerificationDto VerificationObj)
        {
            ApiResponse<EmailMobileVerificationDto> OTPResponse = new();

            var emailIsExistAndVerified = await FindByUsernameAsync(VerificationObj.Email);
            if (emailIsExistAndVerified != null && emailIsExistAndVerified.EmailConfirmed)
            {
                OTPResponse.Success = false;
                OTPResponse.Message = SharedResources.EmailUsedBefore;
                return OTPResponse;
            }

            string Code = await GenerateVerificationCode(null, VerificationObj);

            OTPResponse.Success = true;
            if (VerificationObj.Mobile.IsNotNullOrEmpty())
            {
                if (!_appSettingsService.IsSmsSimulation)
                {
                    var SMSResponse = await _notificationService.SendSMS(VerificationObj.Mobile, Code, SmsTemplateNames.Users_PhoneVerificationSMS.ToString());
                    Code = string.Empty;
                    OTPResponse.Success = SMSResponse.Success;
                }
            }
            else //SendEmail
            {
                //bool IsEmailSent = true;
                bool IsEmailSent = await _notificationService.SendEmailWithCallBack(VerificationObj.Email, string.Empty, Code, EmailTemplateNames.Users_EmailVerification.ToString());
                Code = string.Empty;
                OTPResponse.Success = IsEmailSent;
            }

            OTPResponse.Value = new EmailMobileVerificationDto
            {
                Email = VerificationObj.Email,
                Mobile = VerificationObj.Mobile,
                Code = Code
            };
            return OTPResponse;
        }

        public async Task<ApiResponse<bool>> ConfirmEmailMobileOTP(EmailMobileVerificationDto VerificationObj)
        {
            ApiResponse<bool> ApiResponse = new();

            if (VerificationObj.Code.IsNullOrEmpty() && !VerificationObj.Code.All(char.IsDigit))
            {
                ApiResponse.Message = SharedResources.ErrorActivationCode;
                return ApiResponse;
            }

            var UserOtp = await _userOtpAppService.GetUserOtpByEmailOrMobile(VerificationObj);
            if (UserOtp != null && UserOtp.Otp != VerificationObj.Code)
            {
                ApiResponse.Message = SharedResources.ErrorActivationCode;
                return ApiResponse;
            }
            await _userOtpAppService.DeleteUserOtp(Guid.Empty, VerificationObj);
            ApiResponse.Success = true;
            ApiResponse.Value = true;
            return ApiResponse;
        }

        private string GetDecodedToken(string token)
        {
            var newToken = HttpUtility.UrlDecode(token);
            return newToken.Replace(" ", "+");
        }

        public async Task<string> GetCallBackURL(ApplicationUser userObj, string pageName, GenerationTokenEnum generationToken)
        {
            string token = await GenerateCallBackToken(userObj, generationToken);
            string activationLink = GenerateCallBackLink(userObj, generationToken, token, pageName);
            return activationLink;
        }

        private async Task<string> GenerateCallBackToken(ApplicationUser userObj, GenerationTokenEnum generationToken)
        {
            string token = string.Empty;
            switch (generationToken)
            {
                case GenerationTokenEnum.ResetPassword:
                    token = await _userManager.GeneratePasswordResetTokenAsync(userObj);
                    break;
                case GenerationTokenEnum.ActivateAccount:
                    token = await _userManager.GenerateEmailConfirmationTokenAsync(userObj);
                    break;
                case GenerationTokenEnum.LoginOTP:
                    token = await _dataProtectorTokenProvider.GenerateAsync(generationToken.ToString(), _userManager, userObj);
                    break;
                default:
                    token = await _userManager.GenerateEmailConfirmationTokenAsync(userObj);
                    break;
            }

            return token;
        }

        public async Task<bool> ValidateUserToken(string token, GenerationTokenEnum generationToken, ApplicationUser userObj)
        {
            return await _dataProtectorTokenProvider.ValidateAsync(generationToken.ToString(), token, _userManager, userObj);
        }

        private string GenerateCallBackLink(ApplicationUser userObj, GenerationTokenEnum generationToken, string token, string pageName)
        {
            string activationLink;
            string applicationURL = _appSettingsService.ApplicationUrl;
            var encodedToken = HttpUtility.UrlEncode(token);

            switch (generationToken)
            {
                case GenerationTokenEnum.ResetPassword:
                    activationLink = applicationURL + "authentication/" + pageName + "?token=" + encodedToken + "&userName=" + userObj.Email;
                    break;
                case GenerationTokenEnum.ActivateAccount:
                    activationLink = applicationURL + "authentication/" + pageName + "?token=" + encodedToken + "&userName=" + userObj.Email;
                    break;
                case GenerationTokenEnum.LoginOTP:
                    activationLink = applicationURL + "authentication/" + pageName + "?token=" + encodedToken + "&userName=" + userObj.Email;
                    break;
                default:
                    activationLink = applicationURL + "authentication/" + pageName + "?token=" + encodedToken + "&userName=" + userObj.Email;
                    break;
            }

            return activationLink;
        }

        public async Task<bool> SendTestEmail(string Email)
        {
            string CallBackURL = "Test Links";
            bool IsEmailSent = await _notificationService.SendEmailWithCallBack(Email, "Mahmoud Anwar", CallBackURL, EmailTemplateNames.Users_AccountActivationEmail.ToString());
            return IsEmailSent;
        }

        public async Task<ApiResponse<string>> SendTestSMS(string PhoneNumber)
        {
            var SMSResponse = await _notificationService.SendSMS(PhoneNumber, "1234", SmsTemplateNames.Users_LoginOTPSMS.ToString());
            return SMSResponse;
        }

        private static string GetLastName(string[] nameParts)
        {
            string text = string.Join(" ", nameParts.Select(s => s));
            return text;
        }

        //Hint: This method hit users table to seek matched users -> Don't use in casual request
        public async Task<List<Guid>> GetUserIdsByName(string name, bool useCulture = true)
        {
            var usersIds = await _userRepository.TableNoTracking
                .Select(u => new { u.Id, u.FullNameEn, u.FullNameAr })
                .Where(u => useCulture ?
                (CultureHelper.IsArabic ? u.FullNameAr.ToLower().Contains(name.Trim()) : u.FullNameEn.ToLower().Contains(name.Trim()))
                : (u.FullNameAr.ToLower().Contains(name.Trim()) || u.FullNameEn.ToLower().Contains(name.Trim()))
                ).Select(u => u.Id).ToListAsync();
            return usersIds;
        }

        public async Task<List<string>> GetUsernamesByName(string name, bool useCulture = true)
        {
            var usernames = await _userRepository.TableNoTracking
                .Select(u => new { u.UserName, u.FullNameEn, u.FullNameAr })
                .Where(u => useCulture ?
                (CultureHelper.IsArabic ? u.FullNameAr.ToLower().Contains(name.Trim()) : u.FullNameEn.ToLower().Contains(name.Trim()))
                : (u.FullNameAr.ToLower().Contains(name.Trim()) || u.FullNameEn.ToLower().Contains(name.Trim()))
                ).Select(u => u.UserName).ToListAsync();
            return usernames;
        }

        private string DecryptUserInputPassword(string password)
        {
            var key = _configuration["Encryption:Key"];
            var iv = _configuration["Encryption:iv"];
            return DecryptASE(password, key, iv);
        }

        // decryption function similar to angular cryptojs
        public static string DecryptASE(string encryptedText, string cipherKeyIVPhrase, string iv)
        {
            string plaintext = string.Empty;
            //var cipherTextArray = cipherKeyIVPhrase; //cipherKeyIVPhrase.Split("|");
            string cipherPhrase = cipherKeyIVPhrase; // cipherTextArray[0];
            string salt = iv; //cipherTextArray[1];

            byte[] ciphertext = Convert.FromBase64String(encryptedText);
            // create an rijndaelmanaged object  
            // with the specified key and iv.  
            using (var rijalg = new RijndaelManaged())
            {
                //settings  
                rijalg.Mode = CipherMode.CBC;
                rijalg.Padding = PaddingMode.PKCS7;
                rijalg.FeedbackSize = 128;
                rijalg.Key = Encoding.UTF8.GetBytes(cipherPhrase);
                rijalg.IV = Encoding.UTF8.GetBytes(salt);

                // create a decryptor to perform the stream transform.  
                var decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

                // create the streams used for decryption.  
                using (var msDecrypt = new MemoryStream(ciphertext))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {

                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // read the decrypted bytes from the decrypting stream  
                            // and place them in a string.  
                            plaintext = srDecrypt.ReadToEnd();

                        }

                    }
                }
                return plaintext;
            }
        }

        public CapthcaDto GetRandomCaptchaImg()
        {
            string captchaText = GenerateCaptcha(5);
            var captchaKey = _configuration["reCAPTCHA:Key"];

            CapthcaDto dto = new CapthcaDto()
            {
                Captcha = GenerateImgAsBase64String(captchaText),
                Key = EncryptionAppService.EncryptString(captchaText, captchaKey),
                Text = captchaText
            };
            return dto;
        }

        public bool IsValidCaptcha(string captchaText, string captchaEncryptedKey)
        {
            if (_appSettingsService.IsSimulation) return true;
            var captchaKey = _configuration["reCAPTCHA:Key"];
            var decryptedText = EncryptionAppService.DecryptString(captchaEncryptedKey, captchaKey);
            return decryptedText == captchaText;
        }

        private string GenerateCaptcha(int length)
        {
            const string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTwWvVyYzZ1234567890";
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = CommonHelper.GenerateRandomInteger(0, chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        private string GenerateImgAsBase64String(string captcha)
        {
            string res = "";
            string text = captcha;
            Font font = new Font("Arial", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brush = new SolidBrush(Color.Black);

            Bitmap bitmap = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(bitmap);
            SizeF textSize = graphics.MeasureString(text, font);
            int width = (int)textSize.Width;
            int height = (int)textSize.Height;
            bitmap = new Bitmap(bitmap, new Size(width, height));
            graphics = Graphics.FromImage(bitmap);
            //graphics.Clear(Color.Gray);  // For image background 
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.DrawString(text, font, brush, 0, 0);
            graphics.Flush();

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            res = base64String;

            ms.Dispose();
            graphics.Dispose();
            bitmap.Dispose();
            return res;
        }

        public async Task<ApiResponse<string>> SamlAuthenticateAsync(string UserEmail)
        {
            ApiResponse<string> result = new();
            var response = await SamlUserLogin(UserEmail);

            if (response.Success && response.Value != null)
            {
                result.Success = response.Success;
                result.Value = response.Value.Token;
            }
            else
            {
                result.Message = response.Message;
                result.ModelStateErrors = response.ModelStateErrors;
            }
            return result;
        }
        public async Task<ApiResponse<UserVM>> SamlUserLogin(string UserNameOrEmail)
        {
            ApiResponse<UserVM> ApiResponse = new();
            UserVM UserVM;

            var userObj = await _userRepository.TableNoTracking.FirstOrDefaultAsync(u => u.Email == UserNameOrEmail || u.UserName == UserNameOrEmail || u.NormalizedEmail == UserNameOrEmail.ToUpper());
            if (userObj == null)
            {
                ApiResponse.Message = "ErrorCode001";
                return ApiResponse;
            }
            else
            {
                if (!userObj.IsActive)
                {
                    ApiResponse.Message = "ErrorCode001";
                    return ApiResponse;
                }

                var rolesList = await GetRolesAsync(userObj.Id);

                if (rolesList == null || rolesList.Count == 0)
                {
                    ApiResponse.Message = "ErrorCode001";
                    return ApiResponse;
                }

                if (ApiResponse.Message.IsNullOrEmpty())
                {
                    UserVM = await GenerateUserToken(userObj, rolesList);
                    ApiResponse.Success = true;
                    ApiResponse.Value = UserVM;
                }
            }
            return ApiResponse;
        }

        #region Nafath
        public async Task<ApiResponse<UserVM>> NafathUserUpdate(string PersonId, string FullNameAr, string FullNameEn, DateTime BirthDate, char Gender, string NationalityEn, string NationalityAr)
        {
            ApiResponse<UserVM> ApiResponse = new ApiResponse<UserVM>();
            var NafathUser = await FindByIdentityNoAsync(PersonId);
            if (NafathUser == null)
            {
                ApiResponse.Message = IdentityResources.YouAreNotAuthorized;
                return ApiResponse;
            }
            else if (NafathUser != null && !NafathUser.IsActive)
            {
                ApiResponse.Message = IdentityResources.YouAreNotAuthorized;
                return ApiResponse;
            }

            var rolesList = await GetRolesAsync(NafathUser.Id);
            var token = await GenerateUserToken(NafathUser, rolesList);
            ApiResponse.Value = token;

            return ApiResponse;
        }
        public ApiResponse<IAMClaimDto> NafathGetUserDataFromToken(string Response)
        {
            ApiResponse<IAMClaimDto> ApiResponse = new ApiResponse<IAMClaimDto>();
            var token = Response;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            string status = tokenS.Claims.Where(claim => claim.Type == "status").Select(claim => claim.Value).FirstOrDefault();
            if (status != null)
            {
                if (status == "COMPLETE")
                {
                    string person = tokenS.Claims.Where(claim => claim.Type == "person").Select(claim => claim.Value).FirstOrDefault();
                    if (!string.IsNullOrEmpty(person))
                    {
                        var personObj = JsonConvert.DeserializeObject<IAMClaimDto>(person);
                        string transId = tokenS.Claims.Where(claim => claim.Type == "transId").Select(claim => claim.Value).FirstOrDefault();

                        if (!string.IsNullOrEmpty(transId))
                        {
                            personObj.TransId = transId;
                        }
                        //var UserCreateResponse = await NafathUserUpdate(personObj.Id.ToString(), personObj.ArFullName, personObj.EnFullName, personObj.DobG, personObj.Gender, personObj.EnNationality, personObj.ArNationality);

                        //if (UserCreateResponse.Success && UserCreateResponse.Value != null)
                        //    UserCreateResponse.Value.TransId = transId;
                        ApiResponse.Value ??= new IAMClaimDto();
                        ApiResponse.Value = personObj;
                        return ApiResponse;
                    }
                }
            }

            ApiResponse.Message = SharedResources.InvalidRequestParametersError;
            return ApiResponse;
        }

        public static int GetIdentityTypeByIdentityNumber(string IdnetityNumber)
        {
            if (IdnetityNumber.StartsWith("10"))
            {
                return (int)IdentityTypeEnum.National;
            }
            else if (IdnetityNumber.StartsWith("2"))
            {
                return (int)IdentityTypeEnum.Resident;
            }
            else if (IdnetityNumber.StartsWith("3") || IdnetityNumber.StartsWith("4"))
            {
                return (int)IdentityTypeEnum.Visitor;
            }
            else if (IdnetityNumber.StartsWith("5"))
            {
                return (int)IdentityTypeEnum.VisitorOmrahVisa;
            }
            else
            {
                return (int)IdentityTypeEnum.VisitorHajjVisa;
            }
        }        

        #endregion

    }
}

