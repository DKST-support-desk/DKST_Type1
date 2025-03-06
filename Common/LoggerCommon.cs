using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Common
{
    public static class LoggerCommon
    {
        static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().Name);

        public static void Error(String msg)
        {
            log.Error(msg);
        }
        public static void Warn(String msg)
        {
            log.Warn(msg);
        }
        public static void Info(String msg)
        {
            log.Info(msg);
        }
        public static void Debug(String msg)
        {
            log.Debug(msg);
        }
    }
}
