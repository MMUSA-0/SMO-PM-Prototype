using Framework.Core.Globalization;
using Framework.Core.SharedServices.Services;
using System.Threading.Tasks;

namespace Framework.Core.Notifications
{
    #region usings

    using Framework.Core.Extensions;
    using Framework.Core.SharedServices.Entities;
    using Hangfire;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class NotificationsManager : INotificationsManager
    {

        private readonly NotificationSettings _notificationSettings;
        private readonly NotificationTemplateService _notificationTemplateService;
        private readonly AppSettingsService _appSettingsService;
        private readonly IEmailService _smtpEmailService;
        //private readonly ISmsService _smsService;

        public NotificationsManager(
            NotificationTemplateService notificationTemplateService,
            AppSettingsService appSettingsService,
            IEmailService smtpEmailService
            )
        {
            _notificationTemplateService = notificationTemplateService;
            _appSettingsService = appSettingsService;
            this._notificationSettings = this.LoadNotificationsSettings();
            _smtpEmailService = smtpEmailService;
            //_smsService = smsService;
        }


        public async Task<bool> EnqueueEmailAsync(EmailMessage message, NotificationLanguage? preferredLanguage = null)
        {

            var notification = await this.BuildNotification<EmailMessage>(
                message,
                NotificationTypes.Email,
                preferredLanguage);

            notification.From = _notificationSettings.EmailFromAddress;
            if (notification.DisplayName == null || notification.DisplayName == string.Empty)
            {
                notification.DisplayName =
                    preferredLanguage == NotificationLanguage.Ar ? _notificationSettings.EmailSubjectAr
                    : _notificationSettings.EmailSubjectEn;
            }




            _smtpEmailService.SendEmail(notification, _notificationSettings);
            return true;
        }


        public async Task EnqueueMobileNotificationAsync(
            MobileNotification message,
            NotificationLanguage? preferredLanguage = null)
        {
            var obj = await this.BuildNotification<MobileNotification>(
                message,
                NotificationTypes.WebNotification,
                preferredLanguage);
            BackgroundJob.Enqueue<IMobileNotificationService>(
                s => s.SendMobileNotification(obj, this._notificationSettings));
        }

        public async Task<ApiResponse<string>> EnqueueSmsAsync(SmsMessage message, NotificationLanguage? preferredLanguage = null)
        {
            var notification = await this.BuildNotification<SmsMessage>(message, NotificationTypes.Sms, preferredLanguage);
            //var SMSResponse = await _smsService.SendSms(notification);
            return new ApiResponse<string> { Value=""};

        }

        public async Task EnqueueWebNotificationAsync(WebNotification message, string redirectUrl, NotificationLanguage? preferredLanguage = null)
        {
            // TODO:(Ahmed Gaduo)Add the other actions send to groups / send to group / send to users
            var messageAr = new WebNotification
            {
                UserId = message.UserId,
                Body = message.Body,
                ApplicationId = message.ApplicationId,
                GroupName = message.GroupName,
                GroupsNames = message.GroupsNames,
                TemplateData = message.TemplateData,
                TemplateName = message.TemplateName,
                CreatedBy = message.CreatedBy,
                UsersIds = message.UsersIds
            };
            var messageEn = new WebNotification
            {
                UserId = message.UserId,
                Body = message.Body,
                ApplicationId = message.ApplicationId,
                GroupName = message.GroupName,
                GroupsNames = message.GroupsNames,
                TemplateData = message.TemplateData,
                TemplateName = message.TemplateName,
                CreatedBy = message.CreatedBy,
                UsersIds = message.UsersIds
            };

            await this.BuildNotification<WebNotification>(
               messageAr,
               NotificationTypes.WebNotification,
               NotificationLanguage.Ar);
            await this.BuildNotification<WebNotification>(
                messageEn,
                NotificationTypes.WebNotification,
                NotificationLanguage.En);

            //if (message.UserId != null)
            //{
            //    // just one user
            //    await _useHub.SendMessageToUser(message.UserId, notificationAr.Body, notificationEn.Body, redirectUrl);
            //}
            //else if (message.GroupName != null)
            //{
            //    // just one group/role
            //    await _useHub.SendMessageToGroup(message.GroupName, message.UsersIds, notificationAr.Body, notificationEn.Body, redirectUrl);
            //}
            //else if (message.GroupsNames != null && message.GroupsNames.Count > 1)
            //{
            //    // multiple group/role
            //    await _useHub.SendMessageToGroups(message.GroupsNames, message.UsersIds, notificationAr.Body, notificationEn.Body, redirectUrl);
            //}
            //else
            //{
            //    // here some users without groups or roles
            //    await _useHub.SendMessageToUsers(message.UsersIds.Select(a => a.ToString()).ToList(), notificationAr.Body, notificationEn.Body, redirectUrl);
            //}
        }

        private async Task<T> BuildNotification<T>(
            NotificationMessageBase message,
            NotificationTypes notificationType,
            NotificationLanguage? preferredLanguage = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var isArabic = preferredLanguage == null
                            ? CultureHelper.IsArabic
                            : preferredLanguage == NotificationLanguage.Ar;

            if (!(message is EmailMessage) && !(message is SmsMessage) && !(message is MobileNotification)
                && !(message is WebNotification))
            {
                throw new ArgumentException(
                    "message type must be of any: (EmailMessage, SmsMessage, FireBaseMessage, SignalRWebNotification)");
            }



            var templateName = message.TemplateName;
            message.TemplateData ??= new Dictionary<string, string>();



            var mainContainerTemplateContent = string.Empty;
            var subject = string.Empty;


            //MainEmailContent

            if (notificationType == NotificationTypes.Email)
            {

                var mainTemplate = await _notificationTemplateService.GetTemplateAsync(message.TemplateName, NotificationTypes.Email);
                mainContainerTemplateContent = mainTemplate.Body;
                subject = isArabic ? mainTemplate.SubjectAr.Trim() : mainTemplate.SubjectEn.Trim();

            }
            if (notificationType == NotificationTypes.Sms)
            {
                var mainTemplate = await _notificationTemplateService.GetTemplateAsync(message.TemplateName, NotificationTypes.Sms);
                mainContainerTemplateContent = mainTemplate.Body;
                subject = isArabic ? mainTemplate.SubjectAr.Trim() : mainTemplate.SubjectEn.Trim();
            }

            var template = await _notificationTemplateService.GetTemplateAsync(templateName, notificationType);


            if (mainContainerTemplateContent.IsNullOrEmpty() && notificationType != NotificationTypes.WebNotification)
            {
                throw new NotificationException(
                    $"The template is not available, Check table common.NotificationTemplate");
            }

            // replace values in design template
            ////TODO : Ali
            var templateContent = template.Body;
            subject = $"{template.Subject.Trim()}";

            if (!message.TemplateData.ContainsKey("MailTitle"))
                message.TemplateData.Add("MailTitle", template.Subject.Trim());
            templateContent =
                message.TemplateData.Keys.Aggregate(templateContent, (current, key) =>
                     current.Replace($"{"{" + key + "}"}", message.TemplateData[key]));

            // replace values in common template
            mainContainerTemplateContent =
                message.TemplateData.Keys.Aggregate(mainContainerTemplateContent, (current, key) =>
                     current.Replace($"{"{" + key + "}"}", message.TemplateData[key]));



            // replace values in common template
        
            mainContainerTemplateContent =
                message.TemplateData.Keys.Aggregate(mainContainerTemplateContent, (current, key) =>
                     current.Replace($"{"{" + key + "}"}", message.TemplateData[key]));

            

            //TODO : Ali
            var messageContent = (notificationType == NotificationTypes.WebNotification)
                                 ? templateContent
                                 : mainContainerTemplateContent.Replace("{Body}", templateContent);

           


            switch (notificationType)
            {

                case NotificationTypes.Email:

                    string _mainEmailContent = messageContent;

                    var mainEmailContent = await _notificationTemplateService.GetTemplateAsync(NotificationTemplatesEnum.MainEmailContent.ToString(), notificationType);
                    if (mainEmailContent != null)
                    {
                        _mainEmailContent = isArabic? mainEmailContent.BodyAr : mainEmailContent.BodyEn;
                        _mainEmailContent = _mainEmailContent.Replace("{EmailContent}", messageContent);
                        _mainEmailContent = _mainEmailContent.Replace("{SupportEmail}", _appSettingsService.SupportEmail);
                        _mainEmailContent = _mainEmailContent.Replace("{emailHeaderImage}", _appSettingsService.ApplicationUrl+"assets/images/email-header.jpg");
                    }

                    var email = (EmailMessage)message;
                    email.Body = _mainEmailContent;
                    email.Subject = subject;
                    email.From ??= this._notificationSettings.EmailFromAddress;
                    email.DisplayName ??= isArabic ? this._notificationSettings.EmailSubjectAr : _notificationSettings.EmailSubjectEn;
                    return (T)Convert.ChangeType(email, typeof(T));

                // break;
                case NotificationTypes.Sms:
                    var sms = (SmsMessage)message;
                    sms.Text = messageContent;
                    return (T)Convert.ChangeType(sms, typeof(T));

                // break;
                case NotificationTypes.WebNotification:
                    var webNotification = (WebNotification)message;
                    webNotification.Body = messageContent;
                    return (T)Convert.ChangeType(webNotification, typeof(T));

                // break;
                case NotificationTypes.MobileNotification:
                    var mobileNotification = (MobileNotification)message;
                    return (T)Convert.ChangeType(mobileNotification, typeof(T));

                // break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null);
            }
        }

        public NotificationSettings LoadNotificationsSettings()
        {
            return new NotificationSettings
            {
                EmailFromName = _appSettingsService.GetValue<string>("EmailFromName"),
                EmailFromAddress = _appSettingsService.GetValue<string>("EmailFromAddress"),
                GoogleFCMSenderId = _appSettingsService.GetValue<string>("GoogleFCMSenderId"),
                IsSmtpAuthenticated = _appSettingsService.GetValue<bool>("IsSmtpAuthenticated"),
                SenderId = _appSettingsService.GetValue<string>("SenderId"),
                ServerKey = _appSettingsService.GetValue<string>("ServerKey"),
                SmtpEnableSSL = _appSettingsService.GetValue<bool>("SmtpEnableSSL"),
                SmtpPassword = _appSettingsService.GetValue<string>("SmtpPassword"),
                SmtpPort = _appSettingsService.GetValue<int>("SmtpPort"),
                SmtpUserName = _appSettingsService.GetValue<string>("SmtpUserName"),
                SmtpServer = _appSettingsService.GetValue<string>("SmtpServer"),
                GoogleFCMServerKey = _appSettingsService.GetValue<string>("GoogleFCMServerKey"),
                EmailSubjectAr = _appSettingsService.GetValue<string>("EmailSubjectAr"),
                EmailSubjectEn = _appSettingsService.GetValue<string>("EmailSubjectEn")
            };
        }
    }
}