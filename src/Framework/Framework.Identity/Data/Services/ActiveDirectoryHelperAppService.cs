using Framework.Core;
using Framework.Core.SharedServices.Services;
using Framework.Identity.Data.Dtos;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Framework.Identity.Data.Services
{
    public class ActiveDirectoryHelperAppService
    {
        private readonly AppSettingsService _appSettingsService;

        public ActiveDirectoryHelperAppService(AppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public ADUserCreateDto GetUserFromActiveDirectory(string userName)
        {
            var ADDomain = _appSettingsService.ActiveDirectoryDomainName;
            var ADPassword = _appSettingsService.ActiveDirectoryPassword;
            var ADEmail = _appSettingsService.ActiveDirectoryEmail;

            using var pc = new PrincipalContext(ContextType.Domain, ADDomain, ADEmail, ADPassword);
            var user = UserPrincipal.FindByIdentity(pc, userName);

            return user == null
                ? null
                : new ADUserCreateDto
                {
                    SurName = user.Surname,
                    Description = user.Description,
                    GivenName = user.GivenName,
                    DisplayName = user.DisplayName,
                    Name = user.Name,
                    UserPrincipalName = user.UserPrincipalName,
                    Email = user.EmailAddress ?? user.UserPrincipalName,
                    FullName = user.DisplayName,
                    NationalId = user.EmployeeId,
                    UserName = user.SamAccountName,
                    PhoneNumber = user.VoiceTelephoneNumber
                };
        }

        public bool ValidateADUser(string userEmail, string password)
        {
            bool isValid;
            var ADPassword = _appSettingsService.ActiveDirectoryPassword;
            var ADEmail = _appSettingsService.ActiveDirectoryEmail;
            var ADDomain = _appSettingsService.ActiveDirectoryDomainName;

            using (var ctx = new PrincipalContext(ContextType.Domain, ADDomain, ADEmail, ADPassword))
            {
                isValid = ctx.ValidateCredentials(userEmail, password);
            }

            return isValid;
        }


        public List<ADUserCreateDto> GetAllUsersBySearch(string SearchText)
        {
            var ADUsers = new List<ADUserCreateDto>();

            try
            {
                var myDomainUsers = new List<Principal>();

                var ADDomain = _appSettingsService.ActiveDirectoryDomainName;
                var ADPassword = _appSettingsService.ActiveDirectoryPassword;
                var ADEmail = _appSettingsService.ActiveDirectoryEmail;

                using (var ctx = new PrincipalContext(ContextType.Domain, ADDomain, ADEmail, ADPassword))
                {
                    List<UserPrincipal> searchPrinciples = new List<UserPrincipal>();
                    //searchPrinciples.Add(new UserPrincipal(ctx) { DisplayName = $"*{SearchText}*" });
                    //searchPrinciples.Add(new UserPrincipal(ctx) { SamAccountName = $"*{SearchText}*" });
                    searchPrinciples.Add(new UserPrincipal(ctx) { UserPrincipalName = $"*{SearchText}*" });

                    List<Principal> results = new List<Principal>();
                    var searcher = new PrincipalSearcher();
                    foreach (var item in searchPrinciples)
                    {
                        searcher = new PrincipalSearcher(item);

                        foreach (var principle in searcher.FindAll())
                        {
                            if (!myDomainUsers.Any(x => x.SamAccountName == principle.SamAccountName))
                            {
                                ADUsers.Add(new ADUserCreateDto
                                {
                                    Description = principle.Description,
                                    DisplayName = principle.DisplayName,
                                    Name = principle.Name,
                                    UserPrincipalName = principle.UserPrincipalName,
                                    Email = principle.UserPrincipalName,
                                    FullName = principle.DisplayName,
                                    UserName = principle.SamAccountName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return ADUsers;
        }
    }
}
