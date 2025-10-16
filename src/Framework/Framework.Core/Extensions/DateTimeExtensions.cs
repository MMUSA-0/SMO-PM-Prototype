// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    #region

    #region usings

    using Framework.Core.Utils;
    using System;
    using System.Globalization;
    using System.Text;

    #endregion

    #endregion

    /// <summary>
    ///     The date time extension methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns 12:00am time for the date passed.
        ///     Useful for date only search ranges start value
        /// </summary>
        /// <param name="date">
        /// Date to convert
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime BeginningOfDay(this DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// Returns the Start of the given month (the fist millisecond of the given date)
        /// </summary>
        /// <param name="obj">
        /// DateTime Base, from where the calculation will be preformed.
        /// </param>
        /// <returns>
        /// Returns the Start of the given month (the fist millisecond of the given date)
        /// </returns>
        public static DateTime BeginningOfMonth(this DateTime obj)
        {
            return new DateTime(obj.Year, obj.Month, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// Returns true if the date is between or equal to one of the two values.
        /// </summary>
        /// <param name="date">
        /// DateTime Base, from where the calculation will be preformed.
        /// </param>
        /// <param name="startDate">
        /// Start date to check for
        /// </param>
        /// <param name="endDate">
        /// End date to check for
        /// </param>
        /// <returns>
        /// boolean value indicating if the date is between or equal to one of the two values
        /// </returns>
        public static bool Between(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date.Ticks >= startDate.Ticks && date.Ticks <= endDate.Ticks;
        }

        /// <summary>
        /// The calculate age.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Now.Year - dateTime.Year;
            if (DateTime.Now < dateTime.AddYears(age)) age--;
            return age;
        }

        /// <summary>
        /// Returns 12:59:59pm time for the date passed.
        ///     Useful for date only search ranges end value
        /// </summary>
        /// <param name="date">
        /// Date to convert
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Returns the very end of the given month (the last millisecond of the last hour for the given date)
        /// </summary>
        /// <param name="obj">
        /// DateTime Base, from where the calculation will be preformed.
        /// </param>
        /// <returns>
        /// Returns the very end of the given month (the last millisecond of the last hour for the given date)
        /// </returns>
        public static DateTime EndOfMonth(this DateTime obj)
        {
            return new DateTime(obj.Year, obj.Month, DateTime.DaysInMonth(obj.Year, obj.Month), 23, 59, 59, 999);
        }

        /// <summary>
        /// The is weekend.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday;
        }

        /// <summary>
        /// The is working day.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday;
        }

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="dayOfWeek">
        /// The day of week.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - current.DayOfWeek;
            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }

            return current.AddDays(offsetDays);
        }

        /// <summary>
        /// The next workday.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextWorkday(this DateTime date)
        {
            var nextDay = date;
            while (!nextDay.IsWorkingDay())
            {
                nextDay = nextDay.AddDays(1);
            }

            return nextDay;
        }

        /// <summary>
        /// Returns a nicely formatted duration
        ///     eg. 3 seconds ago, 4 hours ago etc.
        /// </summary>
        /// <param name="dateTime">
        /// The datetime value
        /// </param>
        /// <returns>
        /// A nicely formatted duration
        /// </returns>
        /// <see cref="http://samscode.com/index.php/2009/12/timespan-or-datetime-to-friendly-duration-text-e-g-3-days-ago/"/>
        public static string TimeAgoString(this DateTime dateTime)
        {
            var sb = new StringBuilder();
            var timespan = DateTime.Now - dateTime;

            // A year or more?  Do "[Y] years and [M] months ago"
            if ((int)timespan.TotalDays >= 365)
            {
                // Years
                var nYears = (int)timespan.TotalDays / 365;
                sb.Append(nYears);
                sb.Append(nYears > 1 ? " years" : " year");

                // Months
                var remainingDays = (int)timespan.TotalDays - nYears * 365;
                var nMonths = remainingDays / 30;
                if (nMonths == 1)
                {
                    sb.Append(" and ").Append(nMonths).Append(" month");
                }
                else if (nMonths > 1)
                {
                    sb.Append(" and ").Append(nMonths).Append(" months");
                }
            }

            // More than 60 days? (appx 2 months or 8 weeks)
            else if ((int)timespan.TotalDays >= 60)
            {
                // Do months
                var nMonths = (int)timespan.TotalDays / 30;
                sb.Append(nMonths).Append(" months");
            }

            // Weeks? (7 days or more)
            else if ((int)timespan.TotalDays >= 7)
            {
                var nWeeks = (int)timespan.TotalDays / 7;
                sb.Append(nWeeks);
                sb.Append(nWeeks == 1 ? " week" : " weeks");
            }

            // Days? (1 or more)
            else if ((int)timespan.TotalDays >= 1)
            {
                var nDays = (int)timespan.TotalDays;
                sb.Append(nDays);
                sb.Append(nDays == 1 ? " day" : " days");
            }

            // Hours?
            else if ((int)timespan.TotalHours >= 1)
            {
                var nHours = (int)timespan.TotalHours;
                sb.Append(nHours);
                sb.Append(nHours == 1 ? " hour" : " hours");
            }

            // Minutes?
            else if ((int)timespan.TotalMinutes >= 1)
            {
                var nMinutes = (int)timespan.TotalMinutes;
                sb.Append(nMinutes);
                sb.Append(nMinutes == 1 ? " minute" : " minutes");
            }

            // Seconds?
            else if ((int)timespan.TotalSeconds >= 1)
            {
                var nSeconds = (int)timespan.TotalSeconds;
                sb.Append(nSeconds);
                sb.Append(nSeconds == 1 ? " second" : " seconds");
            }

            // Just say "1 second" as the smallest unit of time
            else
            {
                sb.Append("1 second");
            }

            sb.Append(" ago");

            // For anything more than 6 months back, put " ([Month] [Year])" at the end, for better reference
            if ((int)timespan.TotalDays >= 30 * 6)
            {
                sb.Append(" (" + dateTime.ToString("MMMM") + " " + dateTime.Year + ")");
            }

            return sb.ToString();
        }

        /// <summary>
        /// The to gregorian date string.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <param name="dateFormat">
        /// The date format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToGregorianDateString(this DateTime dateTime, string dateFormat = "dd/MM/yyy")
        {
            return DateTimeHelper.GetGregoreanDateString(dateTime, dateFormat);
        }

        /// <summary>
        /// The to um alqura date string.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <param name="dateFormat">
        /// The date format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToUmAlquraDateString(this DateTime dateTime, string dateFormat = "dd/MM/yyy")
        {
            return DateTimeHelper.GetCurrentUmAlQuraDateString(dateTime, dateFormat);
        }

        // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)

        /// <summary>
        /// The to unix time stamp.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ToUnixTimeStamp(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
        public static string ConvertToStringFormat(this DateTime dt,string formatEn)
        {
            var input = "";
            input = dt.ToString(formatEn, new CultureInfo("en-Us"));
            input = dt.ToString(formatEn);
            return input;
            //if (LanguageIsArabic)
            //{
            //    input = dt.ToString(formatAr, new CultureInfo("ar-EG"));
            //    System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
            //    System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
            //    System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
            //    char[] convertedChar = new char[1];
            //    byte[] bytes = new byte[] { 217, 160 };
            //    char[] inputCharArray = input.ToCharArray();
            //    foreach (char c in inputCharArray)
            //    {
            //        if (char.IsDigit(c))
            //        {
            //            //if (c != '0')
            //            //{
            //            bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
            //            utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
            //            convertedChars.Append(convertedChar[0]);
            //            //}
            //        }
            //        else
            //        {
            //            convertedChars.Append(c);
            //        }
            //    }
            //    return convertedChars.ToString();
            //}
            //else
            //{
            //    input = dt.ToString(formatEn, new CultureInfo("en-Us"));
            //    input = dt.ToString(formatEn);
            //    return input;
            //}

        }
        ////public static string ToReadableTime(this DateTime value)
        ////{
        ////    var ts = new TimeSpan(DateTime.UtcNow.Ticks - value.Ticks);
        ////    double delta = ts.TotalSeconds;
        ////    if (delta < 60)
        ////    {
        ////        return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
        ////    }
        ////    if (delta < 120)
        ////    {
        ////        return "a minute ago";
        ////    }
        ////    if (delta < 2700) // 45 * 60
        ////    {
        ////        return ts.Minutes + " minutes ago";
        ////    }
        ////    if (delta < 5400) // 90 * 60
        ////    {
        ////        return "an hour ago";
        ////    }
        ////    if (delta < 86400) // 24 * 60 * 60
        ////    {
        ////        return ts.Hours + " hours ago";
        ////    }
        ////    if (delta < 172800) // 48 * 60 * 60
        ////    {
        ////        return "yesterday";
        ////    }
        ////    if (delta < 2592000) // 30 * 24 * 60 * 60
        ////    {
        ////        return ts.Days + " days ago";
        ////    }
        ////    if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
        ////    {
        ////        int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
        ////        return months <= 1 ? "one month ago" : months + " months ago";
        ////    }
        ////    var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        ////    return years <= 1 ? "one year ago" : years + " years ago";
        ////}

        /// <summary>
        /// The to w 3 c date.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToW3CDate(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("s") + "Z";
        }

        public static string GetArabicDayName(this DateTime dt)
        {
            if (dt.IsNullOrDefault<DateTime>()) return "";

            return ((ArabicDaysNames)dt.DayOfWeek).ToString();
        }

        public static string GetEnglishDayName(this DateTime dt)
        {
            if (dt.IsNullOrDefault<DateTime>()) return "";

            return ((EnglishDaysNames)dt.DayOfWeek).ToString();
        }

        public enum ArabicDaysNames
        {
            //
            // Summary:
            //     Indicates Sunday.
            الأحد = 0,
            //
            // Summary:
            //     Indicates Monday.
            الإثنين = 1,
            //
            // Summary:
            //     Indicates Tuesday.
            الثلاثاء = 2,
            //
            // Summary:
            //     Indicates Wednesday.
            الأربعاء = 3,
            //
            // Summary:
            //     Indicates Thursday.
            الخميس = 4,
            //
            // Summary:
            //     Indicates Friday.
            الجمعة = 5,
            //
            // Summary:
            //     Indicates Saturday.
            السبت = 6
        }
        public enum EnglishDaysNames
        {
            //
            // Summary:
            //     Indicates Sunday.
            Sunday = 0,
            //
            // Summary:
            //     Indicates Monday.
            Monday = 1,
            //
            // Summary:
            //     Indicates Tuesday.
            Tuesday = 2,
            //
            // Summary:
            //     Indicates Wednesday.
            Wednesday = 3,
            //
            // Summary:
            //     Indicates Thursday.
            Thursday = 4,
            //
            // Summary:
            //     Indicates Friday.
            Friday = 5,
            //
            // Summary:
            //     Indicates Saturday.
            Saturday = 6
        }
    }
}