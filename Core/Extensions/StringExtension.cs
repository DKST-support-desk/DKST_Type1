using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core.Extensions
{
    public static class StringExtension
    {
        public static bool IsEmpy(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32(this string value)
        {
            int val;

            int.TryParse((value ?? "").ToString(), out val);

            return val;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToInt64(this string value)
        {
            long val;

            long.TryParse((value ?? "").ToString(), out val);

            return val;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat(this string value)
        {
            float val;

            float.TryParse((value ?? "").ToString(), out val);

            return val;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            double val;

            double.TryParse((value ?? "").ToString(), out val);

            return val;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string value)
        {
            decimal val;

            decimal.TryParse((value??"").ToString(), out val);

            return val;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            DateTime val;

            DateTime.TryParse((value ?? "").ToString(), out val);

            return val;
        }
    }
}
