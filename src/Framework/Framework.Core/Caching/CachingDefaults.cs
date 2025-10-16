
namespace Framework.Core.Caching
{
    /// <summary>
    /// Represents default values related to caching
    /// </summary>
    public static partial class CachingDefaults
    {
        /// <summary>
        /// Gets the default cache time in minutes
        /// </summary>
        public static int CacheTime => 60;
        public static int LookupsCacheTime => 20000; // Two Weeks, because it didn't change

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string SettingsAllCacheKey => "settings.all";
        public static string LookupsAllCacheKey => "lookups";
        public static string TaskStatus => "lookups.taskStatus";
        public static string VerificationPriority => "lookups.verificationPriority";
        public static string Portfolio => "lookups.portfolio";
        public static string DocumentTitle => "lookups.documentTitle";
        public static string VerificationType => "lookups.verificationType";
        public static string Country => "lookups.country";
        public static string IdentityType => "lookups.identityType";
        public static string Department => "lookups.department";
        public static string City => "lookups.city";
        public static string SocialMediaPlatform => "lookups.socialMediaPlatform";
        public static string Gender => "lookups.gender";



    }
}