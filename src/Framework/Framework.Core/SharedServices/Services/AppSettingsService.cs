using Framework.Core.Caching;
using Framework.Core.Data.Repositories;
using Framework.Core.Globalization;
using Framework.Core.SharedServices.Dto;
using Framework.Core.SharedServices.Entities;
using Framework.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;

namespace Framework.Core.SharedServices.Services
{
    public class AppSettingsService
    {
        private readonly IRepositoryBase<ICommonsDbContext, SystemSetting> _settingRepository;
        private readonly ICacheManager _cacheManager;



        public AppSettingsService(
            IRepositoryBase<ICommonsDbContext, SystemSetting> settingRepository,
            ICacheManager cacheManager)
        {
            _settingRepository = settingRepository;
            _cacheManager = cacheManager;
            LoadSettings();
        }

        protected IDictionary<string, IList<SettingsDto>> GetAllSettingsCached()
        {
            //cache
            return _cacheManager.Get(CachingDefaults.SettingsAllCacheKey, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from s in _settingRepository.TableNoTracking
                            orderby s.Name
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingsDto>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();

                    var settingForCaching = new SettingsDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        GroupName = s.GroupName,
                        ValueType = s.ValueType
                    };

                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingsDto>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }

                return dictionary;
            });
        }

        public List<SettingsDto> GetSettingsByGroup(string groupName)
        {
            var list = new List<SettingsDto>();


            foreach (var setting in GetAllSettingsCached())
            {
                var item = setting.Value.Where(a => a.GroupName.ToLower() == groupName.ToLower());
                list.AddRange(item.ToList());
            }

            return list;
        }

        public List<SettingsDto> GetSettings(string groupName = "NCP")
        {
            if (groupName != null)
                return GetSettingsByGroup(groupName);

            var list = new List<SettingsDto>();
            var settings = _settingRepository.GetSync();

            foreach (var setting in settings)
            {
                list.Add(new SettingsDto
                {
                    Id = setting.Id,
                    GroupName = setting.GroupName,
                    Value = setting.Value,
                    Name = setting.Name,
                    ValueType = setting.ValueType
                });
            }

            return list;
        }

        public SettingsDto GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.Name.ToLowerInvariant() == key);

            return setting;
        }

        public virtual void UpdateSetting(SettingsDto settingsDto, bool clearCache = true)
        {
            if (settingsDto == null)
                throw new ArgumentNullException(nameof(settingsDto));

            var setting = _settingRepository.GetById(settingsDto.Id);
            setting.Value = settingsDto.Value;
            _settingRepository.Update(setting, true);

            //cache
            if (clearCache)
                _cacheManager.Remove(CachingDefaults.SettingsAllCacheKey);
        }

        public void UpdateSettingValue(string key, string value)
        {
            var setting = GetSetting(key);
            if (setting == null) throw new KeyNotFoundException(SharedResources.ItemNotFoundError);

            setting.Value = value;
            UpdateSetting(setting);
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.Name.ToLowerInvariant() == key);


            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        private void LoadSettings()
        {

            foreach (var prop in this.GetType().GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = prop.Name;

                var setting = GetValue<string>(key);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(this, value);
            }

        }

        #region Props

        #region General Settings
        public string SupportEmail { get; set; }
        public string ApplicationUrl { get; set; }
        public string PortalUrl { get; set; }
        public string DateFormat { get; set; }

        public string DateTimeFormat => $"{this.DateFormat} {this.TimeFormat}";

        public int DefaultPagerPageSize { get; set; }

        public string PagerSizeDefaultValues { get; set; }
        
        public int GalleryImageCount { get; set; }

        public string DownloadFileUrl => "/Files/Download/?attId="; // CommonsSettings.ApplicationRootUrl + 

        public int ExportNoOfItems { get; set; } = int.MaxValue;
        public string TimeFormat { get; set; }

        public string RequestDetailsPageUrl { get; set; }
        public string RequestInboxPageUrl { get; set; }

        public bool IsSimulation { get; set; }
        public string SimulationPassKey { get; set; }
        public bool IsDevelopment { get; set; }
        public string MinPeriodToCreateAuction { get; set; }
        public string BaseURL { get; set; }

        #endregion

        #region Attachment Settings

        public int AttachmentsAllowedHeight { get; set; }

        public string AttachmentsAllowedTypes { get; set; }

        public int AttachmentsAllowedWidth { get; set; }

        public int AttachmentsMaxSize { get; set; }

        public string AttachmentsPath { get; set; }

        public bool SaveFilesToDatabase { get; set; }

        public string StructuredAttachmentPath { get; set; }

        public string CouncilSubDirectories { get; set; }

        public string StructuredAttachmentSubDirectories { get; set; }

    #endregion

        #region Notification Settings

    public bool DisableSMSNotifications { get; set; }

        public bool DisableEmailNotifications { get; set; }

        public string ContactUsEmail { get; set; }

        public string EmailSubject => CultureHelper.IsArabic ? this.EmailSubjectAr : this.EmailSubjectEn;

        public string EmailSubjectAr { get; set; }

        public string EmailSubjectEn { get; set; }

        public string EmailFromAddress { get; set; }

        public string EmailFromName { get; set; }


        public string GoogleFCMSenderId { get; set; }

        public string GoogleFCMServerKey { get; set; }

        public bool IsSmtpAuthenticated { get; set; }

        public string SenderId { get; set; }

        public string ServerKey { get; set; }

        public bool SmtpEnableSSL { get; set; }

        public string SmtpPassword { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpServer { get; set; }

        public string SmtpUserName { get; set; }

        public string SmsAPIUserName { get; set; }
        public string SmsAPIPassword { get; set; }
        public string SmsAPIUrl { get; set; }
        public string SmsAPIToken { get; set; }

        public int OTPExpirationDuration { get; set; }
        public int OTPResendDuration { get; set; }
        public int OTPResendTimes { get; set; }

        public bool IsSmsSimulation { get; set; }
        public bool IsNotificationSimulation { get; set; }
        public string NotificationSimulationEmail { get; set; }


        #endregion

        #region Users Management Settings
        public string ActiveDirectoryDomainName { get; set; }
        public string ActiveDirectoryEmail { get; set; }
        public string ActiveDirectoryPassword { get; set; }
        public int VerificationCodeMaxAttempts { get; set; }
        public int IdentityTokenLifespan { get; set; }
        public bool EnableLoginOTP { get; set; }
        public bool EnableAdminLoginOTP { get; set; }
        #endregion

        #region Integeration Settings
        public bool IsNafathSimulation { get; set; }
        public string NafathApiUrl { get; set; }
        public string NafathApiKey { get; set; }

        public string NafathCallBackApiKey { get; set; }
        
        #endregion

        #region PDF Settings
        public string DefaultPDFWatermarkText { get; set; }
        public string PdfDefaultMainLogo { get; set; }
        public string DefaultFlagFolderPath { get; set; }
        #endregion

        #region API Key Settings
        public int DefaultRateLimitPerMinute { get; set; } = 10;
        public int DefaultDaysOfExpirationDate { get; set; } = 365;
        #endregion

        #endregion
    }
}
