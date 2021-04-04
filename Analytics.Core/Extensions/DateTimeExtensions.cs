using System;

namespace Analytics.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return date.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime date) =>
            date.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime date) =>
            new DateTime(date.Year, date.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime date) =>
            date.FirstDayOfMonth().AddMonths(1).AddDays(-1);
    }
}
