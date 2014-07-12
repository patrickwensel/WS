using System;

namespace WS.Framework.Extensions
{
    public static class Extentions
    {
        public static DateTime JdeDateToDateTime(this Decimal? jdeDate)
        {
            DateTime convertedDate;

            short yearValue = (short)(Convert.ToDouble(jdeDate) / 1000d + 1900d);
            short dayValue = (short)((Convert.ToDouble(jdeDate) % 1000) - 1);
            convertedDate = new DateTime(yearValue, 1, 1).AddDays(dayValue);

            return convertedDate;
        }

        public static DateTime JdeDateToDateTime(this int? jdeDate)
        {
            DateTime convertedDate;

            short yearValue = (short)(Convert.ToDouble(jdeDate) / 1000d + 1900d);
            short dayValue = (short)((Convert.ToDouble(jdeDate) % 1000) - 1);
            convertedDate = new DateTime(yearValue, 1, 1).AddDays(dayValue);

            return convertedDate;
        }

        public static int DateTimeToJdeDate(this DateTime dateTime)
        {
            int jdeDate;

            int dayInYear = dateTime.DayOfYear;
            int theYear = dateTime.Year - 1900;
            jdeDate = (theYear * 1000) + dayInYear;

            return jdeDate;
        }

        public static int DateTimeToJDETime(this DateTime dateTime)
        {
            int hour = dateTime.Hour;
            int minute = dateTime.Minute;
            int second = dateTime.Second;

            int jdeTime = hour * 10000 + minute * 100 + second;

            return jdeTime;
        }
    }
}
