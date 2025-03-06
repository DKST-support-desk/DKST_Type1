using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public static class DataGridViewCellEx
    {
        public static double GetDouble(this DataGridViewCell cell)
        {
            double value = 0;

            if (cell.Value != null)
            {
                double.TryParse(cell.Value.ToString(), out value);
            }

            return value;
        }

        public static float GetFloat(this DataGridViewCell cell)
        {
            float value = 0;

            if (cell.Value != null)
            {
                float.TryParse(cell.Value.ToString(), out value);
            }

            return value;
        }

        public static int GetInt32(this DataGridViewCell cell)
        {
            int value = 0;

            if (cell.Value != null)
            {
                int.TryParse(cell.Value.ToString(), out value);
            }

            return value;
        }

        public static long GetInt64(this DataGridViewCell cell)
        {
            long value = 0;

            if (cell.Value != null)
            {
                long.TryParse(cell.Value.ToString(), out value);
            }

            return value;
        }

        public static string GetString(this DataGridViewCell cell)
        {
            return (cell.Value ?? "").ToString();
        }

    }
}
