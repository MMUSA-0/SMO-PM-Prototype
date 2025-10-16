using Framework.Core;
using Framework.Core.Extensions;
using Framework.Core.Notifications;
using Framework.Core.SharedServices.Services;
using Framework.Identity.Data.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Identity
{
    public class NotificationService
    {

        private readonly INotificationsManager _notificationsManager;
        private readonly ILogger<NotificationService> _logger;
        private readonly AppSettingsService _appSettingsService;

        public NotificationService(INotificationsManager notificationsManager, ILogger<NotificationService> logger,
            AppSettingsService appSettingsService)
        {
            _notificationsManager = notificationsManager;
            _logger = logger;
            _appSettingsService = appSettingsService;
        }

        public async Task<ApiResponse<string>> SendSMS(string MobileNo, string replacement, string TemplateName)
        {
            var sms = new SmsMessage
            {
                PhoneNumber = FormatPhoneNumberForSms(MobileNo),
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    {
                        "replacement", replacement
                    }
                }
            };
            
            return await _notificationsManager.EnqueueSmsAsync(sms);
        }

        public async Task<bool> SendEmailWithCallBack(string UserEmail, string FullName, string callbackUrl, string TemplateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { UserEmail },
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    { "PageUrl", callbackUrl },
                    { "FullName", FullName }
                }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }
        public async Task<bool> SendAdminVerificationWithCallBack(string UserEmail, string Code, string TemplateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { UserEmail },
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    { "Code", Code },
                    { "SupportEmail", _appSettingsService.SupportEmail }
                }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }
        public async Task<ApiResponse<string>> SendWelcomeSMS(string MobileNo , string TemplateName)
        {
            var sms = new SmsMessage
            {
                PhoneNumber = FormatPhoneNumberForSms(MobileNo),
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    {
                        "ApplicationURL", _appSettingsService.ApplicationUrl
                    }
                }
            };
            return await _notificationsManager.EnqueueSmsAsync(sms);
        }

        public async Task<bool> SendWelcomeEmail(string UserEmail, string StakeholderType, string CompanyName, string CallBack ,  string TemplateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { UserEmail },
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    { "StakeholderType", StakeholderType },
                    { "CompanyName", CompanyName },
                    { "CallBack", CallBack },
                    { "ApplicationURL", _appSettingsService.ApplicationUrl }
            }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }

        public async Task<bool> SendRegistrationPendingApprovalEmail(string UserEmail, string StakeholderType, string CompanyName, string TemplateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { UserEmail },
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    { "StakeholderType", StakeholderType },
                    { "CompanyName", CompanyName },
                    { "ApplicationURL", _appSettingsService.ApplicationUrl }
            }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }

        public async Task<ApiResponse<string>> SendRegistrationPendingApprovalSMS(string MobileNo, string TemplateName)
        {
            var sms = new SmsMessage
            {
                PhoneNumber = FormatPhoneNumberForSms(MobileNo),
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    {
                        "ApplicationURL", _appSettingsService.ApplicationUrl
                    }
                }
            };
            return await _notificationsManager.EnqueueSmsAsync(sms);
        }

        public async Task<bool> SendRegistrationRejectionEmail(string UserEmail, string StakeholderType, string CompanyName, string TemplateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { UserEmail },
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    { "StakeholderType", StakeholderType },
                    { "CompanyName", CompanyName },
                    { "ApplicationURL", _appSettingsService.ApplicationUrl }
            }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }

        public async Task<ApiResponse<string>> SendRegistrationRejectionSMS(string MobileNo, string TemplateName)
        {
            var sms = new SmsMessage
            {
                PhoneNumber = FormatPhoneNumberForSms(MobileNo),
                TemplateName = TemplateName,
                TemplateData = new Dictionary<string, string> {
                    {
                        "ApplicationURL", _appSettingsService.ApplicationUrl
                    }
                }
            };
            return await _notificationsManager.EnqueueSmsAsync(sms);
        }


        public async Task<bool> SendVerifyProjectSubmissionEmail(string userEmail, string projectName, string projectSubName, string callBack, string templateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { userEmail },
                TemplateName = templateName,
                TemplateData = new Dictionary<string, string> {
                    { "ProjectName", projectName },
                    { "ProjectSubName", projectSubName },
                    { "CallBack", callBack },
                    //{ "ApplicationURL", _appSettingsService.ApplicationUrl }
                }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }

        public async Task<bool> SendProjectSubmissionStatusEmail(string userEmail, string projectName, string projectSubName, string templateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { userEmail },
                TemplateName = templateName,
                TemplateData = new Dictionary<string, string> {
                    { "ProjectName", projectName },
                    { "ProjectSubName", projectSubName }
                }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }

        public async Task<bool> SendProjectSubmissionStatusEmail(string userEmail, string projectName, string projectSubName, string notes, string templateName)
        {
            var mail = new EmailMessage
            {
                To = new List<string> { userEmail },
                TemplateName = templateName,
                TemplateData = new Dictionary<string, string> {
                    { "ProjectName", projectName },
                    { "ProjectSubName", projectSubName },
                    { "Notes", notes }
                }
            };
            return await _notificationsManager.EnqueueEmailAsync(mail);
        }


        public async Task RequestDetailedReview(List<string> userEmails, string serviceName, string requestNumber, string callbackUrl, string templateName)
        {
            foreach (var email in userEmails)
            {
                var mail = new EmailMessage
                {
                    To = new List<string> { email },
                    TemplateName = templateName,
                    TemplateData = new Dictionary<string, string> {
                        { "FullName", email },
                        { "RequestNumber", requestNumber },
                        { "ServiceName",  serviceName},
                        { "CallBack", callbackUrl }
                    }
                };

                await _notificationsManager.EnqueueEmailAsync(mail);
            }
        }

        //public async Task ResetPasswordConfirmation(Guid userId)
        //{
        //    var user = await this._userAppService.FindByIdAsync(userId);
        //    var mail = new EmailMessage
        //    {
        //        To = new List<string> { user.Email },
        //        TemplateName = EmailTemplateNames.Users_ResetPasswordConfirmation.ToString(),
        //        TemplateData = new Dictionary<string, string> {
        //            { "FullName", user.FullName??user.Email  }
        //        }
        //    };
        //    await _notificationsManager.EnqueueEmailAsync(mail);
        //}

        //public async Task<bool> TwoFactorAuthentication_Email(string Email, string FullName, string code)
        //{
        //    try
        //    {
        //        var mail = new EmailMessage
        //        {
        //            To = new List<string> { Email },
        //            TemplateName = EmailTemplateNames.Users_ConfirmEmail.ToString(),
        //            TemplateData = new Dictionary<string, string> {
        //                { "code", code },
        //                { "FullName", FullName }
        //            }
        //        };
        //        await _notificationsManager.EnqueueEmailAsync(mail);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Email failed", ex);
        //        throw;
        //    }
        //}

        private string FormatPhoneNumberForSms(string phone)
        {
            if (phone.StartsWith("+"))
            {
                return phone.Replace("+", "");
            }
            if (phone.StartsWith("00"))
            {
                return phone.Replace("00", "");
            }

            return phone.Trim();
        }
    }
}
