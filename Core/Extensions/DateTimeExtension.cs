using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 月の1日を取得
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime FirstDay(this DateTime value)
        {
            DateTime newValue = value.AddDays(-value.Day + 1);

            return newValue;
        }

        /// <summary>
        /// 月末取得
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime LastDay(this DateTime value)
        {
            DateTime newValue = value.FirstDay().AddMonths(1).AddDays(-1);

            return newValue;
        }
    }
}
