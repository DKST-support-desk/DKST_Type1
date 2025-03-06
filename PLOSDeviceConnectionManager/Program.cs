using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnect;
using PLOSDeviceConnectionManager.Thread;
using log4net;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PLOSDeviceConnectionManager
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr dwProcessAffinityMask);

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 二重起動禁止MUTEX
        /// </summary>
        private const String MUTEX_NAME = "DEVICE_CONTROLER_MUTEX";

        /// <summary>
        /// DB接続体(接続チェック用)
        /// </summary>
        private static SqlDBConnector dbConnector;

        /// <summary>
        /// メイン画面のインスタンス
        /// </summary>
        public static FrmMain frm;

        /// <summary>
        /// 設定ファイル情報
        /// </summary>
        public static AppSetting AppSetting { get; set; }

        /// <summary>
        /// イベント管理スレッドの実態
        /// </summary>
        public static EventManagerThread eventManagementThread;

        public const String TEMP_VIDEO_FILE_NAME_FORMAT = "VIDEO_{0}_Process{1}_Cam{2}.mp4";
        public const String TEMP_AVI_VIDEO_FILE_NAME_FORMAT = "VIDEO_{0}_Process{1}_Cam{2}.avi";
        public const String TEMP_AVI_VIDEO_LIST_FILES_NAME_FORMAT = "VIDEO_{0}_Process{1}-{2}_Cam{3}.avi";
        public const String TEMP_AUDIO_FILE_NAME_FORMAT = "AUDIO_{0}_Process{1}_Mic{2}.wav";
        public const String CYCLE_VIDEO_FILE_NAME_FORMAT = "CYCLE_{0}_Process{1}_Cam{2}.mp4";
        public const String CYCLE_VIDEO_FILE_NAME_FORMAT_TEMP = "CYCLE_{0}_Process{1}_Cam{2}_Temp.mp4";
        public const String NO_MIC_USE = "NO_MIC_USE";

        /// <summary> 映像サイズ横 </summary>
        public const int WIDTH = 1280;
        /// <summary> 映像サイズ縦 </summary>
        public const int HEIGHT = 720;
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {

            System.Threading.Mutex mutex = new System.Threading.Mutex(false, MUTEX_NAME);
            try
            {
                logger.Info("――――――――――――――――――――――――");
                logger.Info("【アプリケーション開始】");
#if DEBUG
                logger.Debug("※※※DEBUGビルドアプリでの起動※※※");
#endif
#if NO_CORE
                logger.Debug("※※※カメラおよび動画生成スレッドにコア割り当てなし※※※");
#endif
#if MOVIE_THREAD_1
                logger.Debug("※※※動画生成スレッド1スレッドのみ※※※");
#endif
                // 二重起動確認
                bool hasHandle = false;
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
                    MessageBox.Show("既に立ち上がっています。多重起動はできません。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 設定ファイルの読み込み
                AppSettingSystemSection system = new AppSettingSystemSection();
                AppSettingWebCamSection webcam = new AppSettingWebCamSection();
                AppSettingDioSection dio = new AppSettingDioSection();
                AppSetting appSetting = new AppSetting(system, webcam, dio);
                if (!XmlFileManagement.ReadParamEnvironmentXml(out appSetting))
                {
                    logger.Error("設定ファイル読込失敗。アプリケーションを終了します。");
                    MessageBox.Show("設定ファイル読込失敗。アプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    AppSetting = appSetting;
                    try
                    {
                        if (!System.IO.Directory.Exists(appSetting.WebcamParam.CycleVideoPath))
                        {
                            System.IO.Directory.CreateDirectory(appSetting.WebcamParam.CycleVideoPath);
                        }
                        if (!System.IO.Directory.Exists(appSetting.WebcamParam.TempFilePath))
                        {
                            System.IO.Directory.CreateDirectory(appSetting.WebcamParam.TempFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(String.Format("フォルダ生成失敗。アプリケーションを終了します。\n詳細：{0}", ex.StackTrace));
                        MessageBox.Show("動画保存フォルダ生成失敗。アプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // DBへのインスタンス確立(接続確認し、即クローズ)
                dbConnector = new SqlDBConnector(AppSetting.SystemParam.DbConnect);
                try
                {
                    if (dbConnector == null)
                    {
                        logger.Error("DB接続失敗。アプリケーションを終了します。");
                        MessageBox.Show("DB接続失敗。アプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    logger.Info("DB接続開始。");
                    dbConnector.Create();
                    dbConnector.OpenDatabase();
                    logger.Info("DB接続終了。");
                }
                catch (Exception sql)
                {
                    logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", sql.Message, sql.StackTrace));
                    logger.Error("DB接続失敗。アプリケーションを終了します。");
                    MessageBox.Show("DB接続失敗。アプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                finally
                {
                    dbConnector.CloseDatabase();
                }

                // プロセス優先度変更
                try
                {
                    Process proc = Process.GetCurrentProcess();
                    proc.PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
                }
                catch (Exception exProc)
                {
                    logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", exProc.Message, exProc.StackTrace));
                    logger.Error("プロセス優先度変更失敗。アプリケーションを終了します。");
                    MessageBox.Show("プロセス優先度変更失敗。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // ファイル削除処理
                try
                {
                    FrmStatus status = new FrmStatus(ProcStatus.DELETE_TEMP);
                    status.ShowDialog();
                }
                catch(Exception exFileDelete)
                {
                    logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", exFileDelete.Message, exFileDelete.StackTrace));
                    logger.Error("起動時ファイル削除処理失敗。アプリケーションを終了します。");
                    MessageBox.Show("起動時ファイル削除処理失敗。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Boolean initRet = false;
                logger.Info("イベント管理機能初期化開始。");
                eventManagementThread = new EventManagerThread(out initRet);
                if (!initRet)
                {
                    logger.Error("イベント管理機能初期化失敗。アプリケーションを終了します。");
                    MessageBox.Show("イベント管理機能初期化失敗。アプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


                // イベント管理スレッドの起動
                eventManagementThread.Start();
                // 全スレッドの起動
                eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.INIT));

                eventManagementThread.EventFailedInit += FaildInitEvent;

                frm = new FrmMain();
                Application.Run(frm);

            }
            catch (Exception ex)
            {
                logger.Fatal(String.Format("【APPLICATION DOWN】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                MessageBox.Show("【APPLICATION DOWN】処理できないエラーを検知。\nアプリケーションを終了します。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                mutex.ReleaseMutex();
                logger.Info("【アプリケーション終了】");
            }
        }

        private static void FaildInitEvent(object sender, EventArgs e)
        {
            while (true)
            {
                if (frm != null)
                {
                    break;
                }
            }
            logger.Info("画面終了要求イベント受信");
            frm.IsThreadKill = true;
        }
    }
}
