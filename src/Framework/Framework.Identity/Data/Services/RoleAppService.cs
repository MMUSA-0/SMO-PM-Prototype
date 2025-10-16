using Framework.Core.AutoMapper;
using Framework.Core.Extensions;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PagedList.Core;
using Framework.Identity.Data.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Framework.Core.Globalization;
using Framework.Core.SharedServices.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ZXing;

namespace Framework.Identity.Data.Services
{
    public class RoleAppService : IRoleAppService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly RoleRepository _roleRepository;
        private readonly AppSettingsService _appSettingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleAppService(
            RoleManager<ApplicationRole> roleManager,
           RoleRepository roleRepository, AppSettingsService appSettingsService,
           IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _appSettingsService = appSettingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> CurrentUserRoles => ((ClaimsIdentity)_httpContextAccessor?.HttpContext?.User?.Identity)?.FindAll(x => x.Type.Contains("role")).Select(x => x.Value).ToList();

        public async Task<IEnumerable<SelectListItem>> List()
        {
            return await _roleRepository.TableNoTracking
              .Select(s => new SelectListItem
              {
                  Text = CultureHelper.IsArabic ? s.DisplayNameAr : s.DisplayNameEn,
                  Value = s.Name
              }).ToListAsync();
        }
        

        public async Task<IEnumerable<RoleDto>> GetEmployeesRoleList()
        {
            var entity = _roleRepository.TableNoTracking;

            entity = entity.Where(r => r.Code != (int)SystemUserRole.SuperAdmin); //SuperAdmin             

            var result = await entity.ToListAsync();
            return result.MapTo<List<RoleDto>>();
        }


        public async Task<RoleDto> GetAsync(Guid id)
        {
            var result = await _roleManager.FindByIdAsync(id.ToString());
            return result.MapTo<RoleDto>();

        }
        public async Task<List<RoleDto>> GetAllAsync()
        {
            var result = await _roleRepository.GetListAsync();
            return result.MapTo<List<RoleDto>>();
        }

        public async Task<RoleDto> FindByRoleNameAsync(string roleName)
        {
            var result = await _roleManager.FindByNameAsync(roleName);
            return result.MapTo<RoleDto>();
        }

        public RoleSearchDto SearchList(RoleSearchDto model)
        {

            var filters = new List<Expression<Func<ApplicationRole, bool>>>();


            if (!model.Name.IsNullOrEmpty())
            {
                Expression<Func<ApplicationRole, bool>>
                    filter = r => r.DisplayNameAr.ToLower().Contains(model.Name) || r.DisplayNameEn.ToLower().Contains(model.Name);
                filters.Add(filter);
            }

            if (!model.Group.IsNullOrEmpty())
            {
                Expression<Func<ApplicationRole, bool>>
                    filter = r => r.RoleGroup == model.Group;
                filters.Add(filter);
            }

            Func<IQueryable<ApplicationRole>, IOrderedQueryable<ApplicationRole>> orderBy;
            if (model.IsDescending.HasValue && model.IsDescending.Value)
            {
                orderBy = a => a.OrderByDescending(b => b.Name);
            }
            else
            {
                orderBy = a => a.OrderBy(b => b.Name);
            }

            model.PageSize = _appSettingsService.DefaultPagerPageSize;

            var result = _roleRepository.SearchWithFilters
                (
                model.PageNumber,
                model.IsExport.HasValue && model.IsExport.Value ? _appSettingsService.ExportNoOfItems : model.PageSize.Value,
                orderBy,
                filters
                );

            model.Items =
                new StaticPagedList<ApplicationRole>(
                    result,
                    result.PageNumber,
                    result.PageSize,
                    result.TotalItemCount);

            return model;


        }

        public async Task<List<ApplicationRole>> SearchByName(string keyword)
        {

            var roles = await _roleRepository.TableNoTracking
                            .Where(s => s.DisplayNameEn.Contains(keyword) || s.DisplayNameAr.Contains(keyword)).ToListAsync();
            return roles;
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return;
            }

            await _roleManager.DeleteAsync(role);
        }

        public async Task<bool> IsRoleInRoleGroup(string roleName, string roleGroup)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            return role.RoleGroup.ToLower() == roleGroup.ToLower();
        }

        public async Task<bool> IsRoleInRoleGroup(Guid roleId, string roleGroup)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            return role.RoleGroup.ToLower() == roleGroup.ToLower();
        }

        public async Task<List<ApplicationRole>> GetRoleByNames(IList<string> RoleNames)
        {
            var roles = await _roleRepository.TableNoTracking.Where(s => RoleNames.Contains(s.Name)).ToListAsync();
            return roles;

        }
    }
}
