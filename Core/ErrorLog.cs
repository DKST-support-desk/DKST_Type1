using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
    public class ErrorLog : List<string>
    {
        private static ErrorLog _errLog = null;

        private ErrorLog()
        {
        }

        private static ErrorLog Instance
        {
            get
            {
                if (_errLog == null)
                {
                    _errLog = new ErrorLog();
                }

                return _errLog;
            }
        }

        public static int LogCount
        {
            get
            {
                return ErrorLog.Instance.Count;
            }
        }

        public static void AddLog(string log)
        {
            ErrorLog.Instance.Add(log);
        }

        public static string[] GetLogs()
        {
            return ErrorLog.Instance.ToArray();
        }

        public static void ClearLog()
        {
            ErrorLog.Instance.Clear();
        }

        public static string GetLog(int index)
        {
            if (index > 0 && index < ErrorLog.Instance.Count)
            {
                return ErrorLog.Instance[index];
            }

            return string.Empty;
        }

        public static string GetLastLog()
        {
            if (ErrorLog.LogCount > 0)
            {
                return ErrorLog.Instance[ErrorLog.Instance.Count - 1];
            }

            return string.Empty;
        }
    }
}
