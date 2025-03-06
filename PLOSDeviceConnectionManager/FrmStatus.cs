using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace PLOSDeviceConnectionManager
{
    public enum ProcStatus
    {
        DELETE_TEMP,
        ENCODE_VIDEO
    }
    public partial class FrmStatus : Form
    {
        private const String DEL_FILES = "起動時：素材動画削除中";
        private const String ENC_VIDEO = "終了時：現象動画生成中";
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ProcStatus mStatus;
        String mText = "";

        public FrmStatus(ProcStatus status)
        {
            InitializeComponent();
            mStatus = status;
        }

        private void FrmStatus_Load(object sender, EventArgs e)
        {
            if (mStatus.Equals(ProcStatus.DELETE_TEMP))
            {
                lblStatus.Text = DEL_FILES;
            }
            else
            {
                lblStatus.Text = ENC_VIDEO;
            }
            bgwStatus.RunWorkerAsync();
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (mStatus.Equals(ProcStatus.DELETE_TEMP))
            {
                DeleteTempFile();
            }
            else
            {
                CheckEndThreadStatus();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbStatus.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (mStatus.Equals(ProcStatus.DELETE_TEMP))
            {
                logger.Info(String.Format("【DeleteTempFile】削除処理完了"));
            }
            else
            {
                logger.Info(String.Format("【CheckEndThreadStatus】動画生成完了"));
            }
            this.Close();
        }

        /// <summary>
        /// 起動時、素材動画フォルダの内容物を全削除する
        /// </summary>
        /// <returns></returns>
        private Boolean DeleteTempFile()
        {
            Boolean ret = false;
            int allFiles = -1;
            int procCount = 0;
            try
            {

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Program.AppSetting.WebcamParam.TempFilePath);

                allFiles = dirInfo.GetFiles().Length;
                logger.Info(String.Format("【DeleteTempFile】削除件数：{0}", allFiles));
                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();
                    procCount++;
                    bgwStatus.ReportProgress((int)(procCount * 100 / allFiles));
                }

                bgwStatus.ReportProgress(100);
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【DeleteMovieFile】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        private bool CheckEndThreadStatus()
        {
            Boolean ret = false;
            int allFiles = -1;
            int procCount = 0;
            try
            {

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Program.AppSetting.WebcamParam.TempFilePath);

                allFiles = dirInfo.GetFiles().Length;
                logger.Info(String.Format("【CheckEndThreadStatus】確認開始時件数：{0}", allFiles));
                while (true)
                {
                    if (Program.eventManagementThread.IsFinishProcess)
                    {
                        logger.Info("イベント管理スレッド停止発行");
                        Program.eventManagementThread.Stop();
                        break;
                    }

                    int execCount = allFiles - dirInfo.GetFiles().Length;
                    if (allFiles > 0)
                    {
                        bgwStatus.ReportProgress((int)(execCount * 100 / allFiles));
                    }
                    System.Threading.Thread.Sleep(1000);
                }

                bgwStatus.ReportProgress(100);
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【CheckEndThreadStatus】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }
    }
}
