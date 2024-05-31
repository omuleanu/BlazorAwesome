using System;

namespace Omu.BlazorAwesome.Models.Utils
{
    /// <summary>
    /// Date utils
    /// </summary>
    public static class AweDateUtil
    {
        /// <summary>
        /// Get start of week
        /// </summary>
        /// <param name="firstDayOfWeek"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(DayOfWeek firstDayOfWeek, DateTime date)
        {
            var diff = firstDayOfWeek - date.DayOfWeek;
            if (diff > 0)
            {
                diff = (7 - diff) * -1;
            }

            return date.AddDays(diff);
        }

        /// <summary>
        /// Get first day of calendar
        /// </summary>
        /// <param name="crdate"></param>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfCalendar(DateTime crdate, DayOfWeek firstDayOfWeek)
        {
            var firstDayOfMonth = new DateTime(crdate.Year, crdate.Month, 1);
            return StartOfWeek(firstDayOfWeek, firstDayOfMonth);
        }
    }
}