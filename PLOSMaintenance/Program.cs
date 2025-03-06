using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using DBConnect;

namespace PLOSMaintenance
{
	static class Program
	{
        /// <summary>
        /// 二重起動禁止MUTEX
        /// </summary>
        private const String MUTEX_NAME = "MASTER_MAINTENANCE_MUTEX";

        /// <summary>
        /// DB接続体(接続チェック用)
        /// </summary>
        private static SqlDBConnector dbConnector;

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, MUTEX_NAME);
            bool hasHandle = false;
            try
            { 
			    logger.Info("------アプリケーション開始------");

                // 二重起動確認
                try
                {
                    //ミューテックスの所有権を要求する
                    hasHandle = mutex.WaitOne(0, false);
                }
                //.NET Framework 2.0以降の場合
                catch (System.Threading.AbandonedMutexException)
                {
                    //別のアプリケーションがミューテックスを解放しないで終了した時
                    hasHandle = true;
                }
                //ミューテックスを得られたか調べる
                if (hasHandle == false)
                {
                    //得られなかった場合は、すでに起動していると判断して終了
                    logger.Error("多重起動検知。多重起動はできません。");
                    MessageBox.Show("既に立ち上がっています。多重起動はできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // DBへのインスタンス確立
                dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
                if (dbConnector == null)
                {
                    logger.Error("DB接続失敗。アプリケーションを終了します。");
                    MessageBox.Show("DB接続失敗。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //DB接続確認し、即クローズ
                try
                {
                    logger.Info("DB接続開始。");
                    dbConnector.Create();
                    dbConnector.OpenDatabase();
                    logger.Info("DB接続終了。");
                }
                catch (Exception sql)
                {
                    logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", sql.Message, sql.StackTrace));
                    logger.Error("DB接続失敗。アプリケーションを終了します。");
                    MessageBox.Show("DB接続失敗。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    dbConnector.CloseDatabase();
                }

                //Common.Logger.Instance.SystemName = "PLOS";
			    //Common.Logger.Instance.AppName = "Maintenance";
			    //Common.Logger.Instance.WriteTraceLog("Start System");
			    //Common.Logger.Instance.CheckLogFile();

			    Application.EnableVisualStyles();
			    Application.SetCompatibleTextRenderingDefault(false);
			    Application.Run(new FrmMain());
            }
            catch (Exception ex)
            {
                logger.Fatal(String.Format("【APPLICATION DOWN】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                if (hasHandle)
                {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }
                mutex.Close();
                
                logger.Info("【アプリケーション終了】");
            }
        }
	}
}
