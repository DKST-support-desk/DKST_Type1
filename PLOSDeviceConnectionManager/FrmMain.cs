
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnect.SQL;
using PLOSDeviceConnectionManager.Thread;
using log4net;
using System.Management;

namespace PLOSDeviceConnectionManager
{
    public partial class FrmMain : Form
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>終了バックグラウンド処理 </summary>
        private BackgroundWorker mEndProcessBackGround;
        #endregion

        #region <Property>
        public Boolean IsThreadKill { get; set; }

        public Bitmap CapBitMap
        {
            set { this.pcbStreamView.Image = value; }
        }
        public int SelectCapBitMap { get; private set; }
        #endregion

        #region <Constructor>

        public FrmMain()
        {
            InitializeComponent();
        }

        #endregion

        #region <Method>

        /// <summary>
        /// 画面ロード時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            pcbStreamView.SizeMode = PictureBoxSizeMode.Zoom;
            // カメラリストの取得
            foreach (DataRow row in Program.eventManagementThread.CameraDeviceMst.Rows)
            {
                String data = row.Field<String>(ColumnCameraDeviceMst.CAMERA_NAME);
                dgvCameraStatus.Rows.Add(data, "撮像×");
            }

            // QRリーダリストの取得
            foreach (DataRow row in Program.eventManagementThread.ReaderDeviceMst.Rows)
            {
                String data = row.Field<String>(ColumnReaderDeviceMst.READER_NAME);
                dgvQrStatus.Rows.Add(data, "接続×");
            }

            for (int i = 0; i < Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex.Count; i++)
            {
                cmbSelectInnerProcess.Items.Add(String.Format("工程{0}", i + 1));
            }

            cmbCamIndex.Items.Add(String.Format("未選択"));
            for (int i = 0; i < Program.eventManagementThread.CameraDeviceMst.Rows.Count; i++)
            {
                cmbCamIndex.Items.Add(String.Format("表示番号{0}", i));
            }
            cmbCamIndex.SelectedIndex = 0;

            if(Program.AppSetting.WebcamParam.ShowStream > 0)
            {
                pnlStreamViewer.Visible = true;
            }
            else
            {
                pnlStreamViewer.Visible = false;
            }

            tmrDeviceStatus.Start();
            //debugTimer.Start();
        }

        /// <summary>
        /// 機器監視タイマーの定周期イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmrDeviceStatus_Tick(object sender, EventArgs e)
        {
            if (IsThreadKill)
            {
                this.Close();
            }

            if (Program.eventManagementThread.IsOperating)
            {
                lblOperating.Text = "直状態信号：ON";
                lblOperating.BackColor = Color.Lime;
                btnUpdateShift.Enabled = false;
            }
            else
            {
                lblOperating.Text = "直状態信号：OFF";
                lblOperating.BackColor = Color.Red;
                btnUpdateShift.Enabled = true;
            }
            if (DioThread.DioStatus)
            {
                lblStatusDio.Text = "DIO接続状態：正常";
                lblStatusDio.BackColor = Color.Lime;
            }
            else
            {
                lblStatusDio.Text = "DIO接続状態：異常";
                lblStatusDio.BackColor = Color.Red;
            }

            lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
            // 各機器への問い合わせ(カメラ、QRリーダ、DIOの死活をイベント発行)
            Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.DEVICE_STATUS));

            // 各カメラ,QRリーダのステータス更新(DGV更新)
            for (int i = 0; i < dgvQrStatus.Rows.Count; i++)
            {
                // QRリーダ
                if (Program.eventManagementThread.ListQrReaderAlive.Count > i)
                {
                    dgvQrStatus.Rows[i].Cells["clmQrStatus"].Value = Program.eventManagementThread.ListQrReaderAlive[i] ? "接続中" : "接続×";
                }
            }
            // 各カメラ,QRリーダのステータス更新(DGV更新)
            for (int i = 0; i < dgvCameraStatus.Rows.Count; i++)
            {
                // カメラ
                if (Program.eventManagementThread.ListCameraAlive.Count > i)
                {
                    dgvCameraStatus.Rows[i].Cells["clmCameraStatus"].Value = Program.eventManagementThread.ListCameraAlive[i] ? "撮像中" : "撮像×";
                    dgvCameraStatus.Rows[i].Cells["clmCameraViewIndex"].Value = Program.eventManagementThread.ListCameraIndex[i];
                }
            }

            // DIOの状態表示
            for (int i = 0; i < Thread.DioCtrlThread.listHardSensorStatus.Count; i++)
            {
                ucBitViewInput.ChangeBitStatus(i, Thread.DioCtrlThread.listHardSensorStatus[i].mThisBitStatus);
            }
            for (int i = 0; i < Thread.DioCtrlThread.listHardOutputStatus.Count; i++)
            {
                uclBitViewOutput.ChangeBitStatus(i, Thread.DioCtrlThread.listHardOutputStatus[i].mOutputBitStatus);
            }
            int selectProcessIndex = cmbSelectInnerProcess.SelectedIndex;
            if (selectProcessIndex < 0)
            {
            }
            else
            {
                if (Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex != null && Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex.Count > 0)
                {
                    for (int i = 0; i < Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex[selectProcessIndex].Count; i++)
                    {
                        // 内部工程1
                        ucBitViewInnerSelect.ChangeBitStatus(i, Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex[selectProcessIndex][i].mInnerBitStatus);
                    }
                }
            }
        }

        /// <summary>
        /// シフト更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateShift_Click(object sender, EventArgs e)
        {
            Program.eventManagementThread.CheckShiftUpdate();
        }

        /// <summary>
        /// フォーム終了時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsThreadKill)
            {
                DialogResult dr = MessageBox.Show("本画面を閉じるとサイクル実績の取得ができなくなります。\n本当に閉じてよろしいですか？", "画面終了確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr != DialogResult.OK)
                {
                    e.Cancel = true;
                }
                logger.Info("システム終了処理開始");
                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.TERM));

                FrmStatus status = new FrmStatus(ProcStatus.ENCODE_VIDEO);
                status.ShowDialog();

                //while (true)
                //{
                //    if (Program.eventManagementThread.IsFinishProcess)
                //    {
                //        logger.Info("イベント管理スレッド停止発行");
                //        Program.eventManagementThread.Stop();
                //        break;
                //    }
                //    System.Threading.Thread.Sleep(100);
                //}
                logger.Info("システム終了処理完了");

                //mEndProcessBackGround = new BackgroundWorker();
                //mEndProcessBackGround.WorkerReportsProgress = true;
                //mEndProcessBackGround.DoWork += WorkeDoWorkEndProcess;
                //logger.Debug(String.Format("【StopThreads】バックグラウンド処理で停止処理の開始"));
                //mEndProcessBackGround.RunWorkerAsync();
            }
        }

        private void WorkeDoWorkEndProcess(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (Program.eventManagementThread.IsFinishProcess)
                {
                    logger.Info("イベント管理スレッド停止発行");
                    Program.eventManagementThread.Stop();
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
            logger.Info("システム終了処理完了");
        }

        /// <summary>
        /// 内部工程センサのリスト更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInnerProcessReset_Click(object sender, EventArgs e)
        {
            cmbSelectInnerProcess.Items.Clear();
            for (int i = 0; i < Thread.DioCtrlThread.mListInnerSensorStatusByProcessIndex.Count; i++)
            {
                cmbSelectInnerProcess.Items.Add(String.Format("工程{0}", i + 1));
            }
        }

        /// <summary>
        /// 確認するカメラを選択する(初期表示：未選択)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCamIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCapBitMap = ((ComboBox)sender).SelectedIndex - 1;
            if (((ComboBox)sender).SelectedIndex < 1)
            {
                this.pcbStreamView.Image = null;
            }
        }

        #endregion
    }
}
