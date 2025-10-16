using Framework.Core.AutoMapper;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Helper;
using Framework.Identity.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PagedList.Core;
using Framework.Core.Data;
using Framework.Core.Globalization;

namespace Framework.Identity.Data.Services
{
    public class UserRoleAppService
    {
        private readonly UserRolesRepository _userRolesRepository;
        private readonly RoleRepository _roleRepository;
        public UserRoleAppService(UserRolesRepository userRolesRepository, RoleRepository roleRepository)
        {
            _userRolesRepository = userRolesRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<ClaimUserRoleDto>> GetClaimUserRolesList(Guid UserId)
        {
            var RoleIdsList = await _userRolesRepository.TableNoTracking.Where(q => q.UserId == UserId).Select(x => x.RoleId).ToListAsync();
            return await _roleRepository.TableNoTracking.Where(q => RoleIdsList.Contains(q.Id)).Select(q => new ClaimUserRoleDto { RoleId = q.Id, RoleName = q.Name }).ToListAsync();
        }

        public async Task<List<Guid>> GetUserRoleIdsList(Guid UserId)
        {
            var RoleIdsList = await _userRolesRepository.TableNoTracking.Where(q => q.UserId == UserId).Select(x => x.RoleId).ToListAsync();
            return RoleIdsList;
        }

        public async Task<List<string>> GetRolesNamesAsync(Guid UserId)
        {
            var RoleIdsList = await _userRolesRepository.TableNoTracking.Where(q => q.UserId == UserId).Select(x => x.RoleId).ToListAsync();
            return await _roleRepository.TableNoTracking.Where(q => RoleIdsList.Contains(q.Id)).Select(q => q.Name).ToListAsync();
        }

        public async Task<List<ApplicationRole>> GetRolesByUserIdAsync(Guid UserId)
        {
            var RoleIdsList = await _userRolesRepository.TableNoTracking.Where(q => q.UserId == UserId).Select(x => x.RoleId).ToListAsync();
            return await _roleRepository.TableNoTracking.Where(q => RoleIdsList.Contains(q.Id)).ToListAsync();
        }

        public async Task<List<string>> GetRolesDisplayNameAsync(Guid UserId)
        {
            var RoleIdsList = await _userRolesRepository.TableNoTracking.Where(q => q.UserId == UserId).Select(x => x.RoleId).ToListAsync();
            return await _roleRepository.TableNoTracking.Where(q => RoleIdsList.Contains(q.Id)).Select(q => CultureHelper.IsArabic ? q.DisplayNameAr : q.DisplayNameEn).ToListAsync();
        }

        public async Task<Guid?> InsertAsync(UserRolesDto userRole, bool autoSave = false)
        {
            var entity = userRole.MapTo<ApplicationUserRoles>();
            var insertedEntity = await _userRolesRepository.InsertAsync(entity, autoSave);
            if (insertedEntity != null)
            {
                return insertedEntity.Id;
            }
            return null;
        }

        public async Task InsertRangeAsync(List<UserRolesDto> userRoles, bool autoSave = false)
        {
            var userRolesList = userRoles.MapTo<List<ApplicationUserRoles>>();
            await _userRolesRepository.InsertRangeAsync(userRolesList, autoSave);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var oldData = await _userRolesRepository.TableNoTracking.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (oldData != null)
            {
                return await _userRolesRepository.DeleteAsync(s => s.Id == id, true);
            }
            else
            {
                return false;
            }
        }

        public async Task<UserRolesSearchDto> GetList(UserRolesSearchDto model)
        {
            var filters = new List<Expression<Func<ApplicationUserRoles, bool>>>();

            if (model.RoleId.HasValue)
            {
                //if (model.RoleId == RoleHelper.CentralOperationsGM)
                //{
                //    filters.Add(q => q.RoleId == RoleHelper.CentralOperationsGM || q.RoleId == RoleHelper.AuctionManager || q.RoleId == RoleHelper.CommitmentOfficer || q.RoleId == RoleHelper.DebtDepartmentManager);
                //}
                //else
                //{
                    filters.Add(q => q.RoleId == model.RoleId);
                //}
            }
            if (model.UserId.HasValue)
            {
                filters.Add(q => q.UserId == model.UserId);
            }

            model.PageSize = 15;//_appSettingsService.DefaultPagerPageSize;
            var result = _userRolesRepository
                  .SearchAndSelectWithFilters
                  (
                  b => b.OrderBy(a => a.CreatedOn),
                  a => a.MapTo<UserRolesDto>(),
                  filters
                  );
            // get paged list
            var results = result.GetPaged(
                o => o.RoleId, // order by
                model.IsDescending.HasValue && model.IsDescending.Value, // ascending (based on customer comments)
                model.PageNumber, // page number
                model.PageSize.Value);

            // bind items to search results
            model.Items = new StaticPagedList<UserRolesDto>(
                results,
                results.PageNumber,
                15,
                results.TotalItemCount);

            model.TotalItemsCount = model.Items.TotalItemCount;
            return await Task.FromResult(model);

        }

        public void DeleteByUserIdRoleId(Guid userId, Guid roleId)
        {
            var userRoles = _userRolesRepository.TableNoTracking.Where(s => s.UserId == userId && s.RoleId == roleId).ToList();
            _userRolesRepository.DeleteRange(userRoles, true);
        }

        public void DeleteUserRoleNotAssigned(Guid userId, IList<ApplicationRole> Roles)
        {
            var RolesIds = Roles.Select(x => x.Id).ToList();
            var userRoles = _userRolesRepository.TableNoTracking.Where(s => s.UserId == userId && !RolesIds.Contains(s.RoleId)).ToList();
            _userRolesRepository.DeleteRange(userRoles, true);
        }
    }
}
