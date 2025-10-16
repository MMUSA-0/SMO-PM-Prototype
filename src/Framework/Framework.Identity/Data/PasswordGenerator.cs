using Framework.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Identity.Data
{
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        /// 
        public static string Generate(
            int requiredLength = 8,
            int requiredUniqueChars = 4,
            bool requireDigit = true,
            bool requireLowercase = true,
            bool requireNonAlphanumeric = true,
            bool requireUppercase = true)
        {
            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
            };

            List<char> chars = new List<char>();

            if (requireUppercase)
                chars.Insert(chars.Count, randomChars[0][CommonHelper.GenerateRandomInteger(0, randomChars[0].Length)]);

            if (requireLowercase)
                chars.Insert(chars.Count, randomChars[1][CommonHelper.GenerateRandomInteger(0, randomChars[1].Length)]);

            if (requireDigit)
                chars.Insert(chars.Count, randomChars[2][CommonHelper.GenerateRandomInteger(0, randomChars[2].Length)]);

            if (requireNonAlphanumeric)
                chars.Insert(chars.Count, randomChars[3][CommonHelper.GenerateRandomInteger(0, randomChars[3].Length)]);


            for (int i = chars.Count; i < requiredLength || chars.Distinct().Count() < requiredUniqueChars; i++)
            {
                string rcs = randomChars[CommonHelper.GenerateRandomInteger(0, randomChars.Length)];
                chars.Insert(CommonHelper.GenerateRandomInteger(0, chars.Count), rcs[CommonHelper.GenerateRandomInteger(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
