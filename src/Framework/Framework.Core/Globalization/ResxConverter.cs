// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResxConverter.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Globalization
{
    #region usings

    using Framework.Core.Extensions;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;

    #endregion

    /// <summary>
    ///     The resx converter.
    /// </summary>
    public class ResxConverter
    {
        /// <summary>
        /// The to dictionary.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <param name="resourceTypeFullName">
        /// The resource type full name.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, string> ToDictionary(
            string assemblyName,
            string resourceTypeFullName,
            string cultureName = "ar-SA")
        {
            var resourcesDictionary = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(resourceTypeFullName) && !string.IsNullOrEmpty(assemblyName))
            {
                var t = Type.GetType($"{resourceTypeFullName}, {assemblyName}");
                var rm = new ResourceManager(resourceTypeFullName, t.Assembly);
                var resourceSet = rm.GetResourceSet(CultureHelper.GetCultureInfo(cultureName), true, true);
                resourcesDictionary = resourceSet.Cast<DictionaryEntry>()
                    .ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            }

            return resourcesDictionary;
        }

        /// <summary>
        /// The to json.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <param name="resourceTypeFullName">
        /// The resource type full name.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToJson(string assemblyName, string resourceTypeFullName, string cultureName = "ar-SA")
        {
            var resourcesDictionary = ToDictionary(assemblyName, resourceTypeFullName, cultureName);

            var settings = JsonDotNetSerializer.SerializerSettings;
            settings.ContractResolver = new DefaultContractResolver();

            return resourcesDictionary.ToJson(settings);
        }
    }
}