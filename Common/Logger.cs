using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Common
{
    /// <summary>
    /// エラーログ
    /// </summary>
    public class Logger
    {
        // マルチスレッド対応のシングルトンパターン
        private static volatile Logger instance;
        private static object syncRoot = new Object();
        private Logger() { }

        // ロック用のインスタンス
        private static ReaderWriterLock readerWriterLock = new ReaderWriterLock();

        // シングルトンパターンのインスタンス取得メソッド
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Logger();
                    }
                }
                return instance;
            }
        }

		public int MaximumLoggingDays { get; set; } = 7;
		public String SystemName { get; set; } = "SystemName";

		public String AppName { get; set; } = "SystemName";

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="msg">メッセージ</param>
		/// <remarks></remarks>
		public void WriteTraceLog(String msg)
        {
            WriteTraceLog(msg, null);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="msg">メッセージ</param>
        /// <param name="ex">Exception(無指定の場合はメッセージのみ出力)</param>
        /// <remarks></remarks>
        public void WriteTraceLog(String msg, Exception ex)
        {
            try
            {
                // ログフォルダ名作成
                DateTime dt = DateTime.Now;
				String logSystemFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), $"{SystemName}");
				System.IO.Directory.CreateDirectory(logSystemFolder);
				String logAppFolder = System.IO.Path.Combine(logSystemFolder, $"{AppName}");
                System.IO.Directory.CreateDirectory(logAppFolder);

                // ログファイル名作成
                String logFile = logAppFolder + "\\TraceLog" + dt.ToString("yyyyMMdd") + ".log";
                
                // ログ出力文字列作成
                String logStr;
                logStr = dt.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + msg;
                if (ex != null)
                {
                    logStr = logStr + "\n" + ex.ToString();
                }

                // ロックする
                readerWriterLock.AcquireWriterLock(Timeout.Infinite);

                // Shift-JISでログ出力
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(logFile, true,
                        System.Text.Encoding.GetEncoding("Shift-JIS"));
                    sw.WriteLine(logStr);
                }
                catch(Exception exx) 
                {
                    Console.WriteLine(exx.Message);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                    // ロックを解除する
                    readerWriterLock.ReleaseWriterLock();
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);
            }
        }

        /// <summary>
        /// 既定日以上前のログを削除
        /// </summary>
        public void CheckLogFile() 
        {
            string[] files=null;
            Nullable<DateTime> dtToday = null;
            Nullable<DateTime> dtFile = null;

            try
            {
				String logSystemFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), $"{SystemName}");
				String logAppFolder = System.IO.Path.Combine(logSystemFolder, $"{AppName}");
                files = Directory.GetFiles(logAppFolder);
                dtToday = DateTime.Today;
            }
            catch (Exception err)
            {
                WriteTraceLog(err.ToString());
            }

			try
			{
				for (int i = 0; i < files.Length;i++ )
                {
                    if (files[i].Contains("TraceLog"))
                    {
                        string[] filesnames = files[i].Split('\\');
                        string day = filesnames[filesnames.Length - 1];
                        day = day.Replace("TraceLog", "");
                        day = day.Replace(".log", "");
                        dtFile = new DateTime(int.Parse(day.Substring(0, 4)), int.Parse(day.Substring(4, 2)), int.Parse(day.Substring(6, 2)));
                        if (dtToday - new TimeSpan(MaximumLoggingDays, 0, 0, 0) > dtFile ) { System.IO.File.Delete(files[i]); }
                    }
                }
			}
			catch { }

		}

	}
}
