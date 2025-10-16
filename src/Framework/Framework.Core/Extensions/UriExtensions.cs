// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    using Microsoft.AspNetCore.Http;
    #region usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    ///     The uri extensions.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Returns the base domain from a domain name
        ///     Example: http://www.west-wind.com returns west-wind.com
        /// </summary>
        /// <param name="uri">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetBaseDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
            {
                return GetBaseDomain(uri.DnsSafeHost);
            }

            return uri.Host;
        }

        /// <summary>
        /// Retrieves a base domain name from a full domain name.
        ///     For example: www.west-wind.com produces west-wind.com
        /// </summary>
        /// <param name="domainName">
        /// Dns Domain name as a string
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetBaseDomain(string domainName)
        {
            var tokens = domainName.Split('.');

            // only split 3 urls like www.west-wind.com
            if (tokens.Length != 3)
            {
                return domainName;
            }

            var tok = new List<string>(tokens);
            var remove = tokens.Length - 2;
            tok.RemoveRange(0, remove);

            return tok[0] + "." + tok[1];
        }

        public static string GetReturnUrl(this HttpRequest httpRequest)
        {
            return $"{httpRequest.Path}{httpRequest.QueryString}".Replace("/", "%2F").Replace("&", "%26");
        }
        public static string GetReturnUrlWithoutHash(this HttpRequest httpRequest)
        {
            return $"{httpRequest.Path}/{httpRequest.QueryString}";
        }
    }
}