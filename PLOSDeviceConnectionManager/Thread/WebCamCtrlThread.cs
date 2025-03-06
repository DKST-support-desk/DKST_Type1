using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using TskCommon;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Data;
using log4net;
using System.Runtime.ExceptionServices;
using System.Drawing;
using System.ComponentModel;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;
using System.Runtime.InteropServices;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum WebCamCtrlThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> 直開始 </summary>
        SHIFT_START,
        /// <summary> 直終了 </summary>
        SHIFT_END,
        /// <summary> サイクル </summary>
        CYCLE,
    }
    /// <summary>
    /// Webカメラ通信管理スレッドクラス
    /// </summary>
    public class WebCamCtrlThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ///// <summary> 映像サイズ横 </summary>
        //private const int Program.WIDTH = 1280;
        ///// <summary> 映像サイズ縦 </summary>
        //private const int Program.HEIGHT = 720;
        private const String NO_CAM_NAME = "NO-CAMERA-DEVICE";

        private const double MOVIE_FPS = 40;

        /// <summary> 自スレッドインデックス </summary>
        private int mThreadIndex = -1;
        /// <summary> 自スレッドが管理しているカメラのデバイスインスタンスパス(フル) </summary>
        private String mCameraDeviceId = "";
        /// <summary> 現在のシフト番号(1 ～ 3) </summary>
        private int mOperationShift = -1;

        /// <summary> 自スレッドの死活 </summary>
        private Boolean mThreadAlive = false;
        /// <summary> 自スレッドが管理しているカメラの死活(外部出力用) </summary>
        private Boolean mIsAliveCamera = false;
        /// <summary> 自スレッドが管理しているカメラの初期化状態 </summary>
        private Boolean mIsInitCamera = false;

        /// <summary> 対象カメラのFPS情報 </summary>
        private Double mFps = 30;
        /// <summary> カメラデバイス情報構造体のリスト </summary>
        private List<StructCameraDevice> mListCameraDevice = new List<StructCameraDevice>();

        /// <summary> 工程ごとに保持する映像ストリーム </summary>
        private Dictionary<int, VideoWriter> mDicVideoWriteStreamOnProcess = new Dictionary<int, VideoWriter>();
        private DateTime mFileDateTime;
        //List<String> mFileNameList = new List<string>();

        /// <summary> 撮影するカメラデバイスの実態 </summary>
        private VideoCaptureDevice mVideoSource = null;


        /// <summary> 工程ごとに保持するカメラの使用設定(0:不使用,1:使用,2:異常時のみ)。リスト数は工程数と一致する </summary>
        private List<int> mListCameraUsingStatusByProcess = new List<int>();

        /// <summary> 工程ごとに保持する録音開始時刻 </summary>
        private Dictionary<int, DateTime> mDicCycleDatetimeOnProcess = new Dictionary<int, DateTime>();

        /// <summary> カメラ名とデバイスIDを保持する構造体 </summary>
        public struct StructCameraDevice
        {
            public String stmCameraName { get; private set; }
            public String stmCameraId { get; private set; }

            public StructCameraDevice(String name, String id)
            {
                stmCameraName = name;
                stmCameraId = id;
            }
        }

        //private delegate void EndCheckDelegate(VideoWriter vw, int prcessIdx, DateTime cycleStartTime, DateTime cycleEndTime, int camUseStatus, DateTime operationDate, Guid composition, Guid productType, String workerName);
        //private delegate void EndCheckAviDelegate(VideoWriterFileClass vw, int prcessIdx, DateTime cycleStartTime, DateTime cycleEndTime, DateTime operationDate, int camUseStatus, Guid composition, Guid productType, String workerName);

        /// <summary> OpenCvでカメラをオープンする際のカメラインデックス </summary>
        public int mOpenCvCameraIndex = -1;

        /// <summary> OpenCVで映像取り込みを行うバックグラウンド処理 </summary>
        private BackgroundWorker mOpenCvBackGroundWorker;

        /// <summary> OpenCvで映像取り込みをするか否かのフラグ </summary>
        private Boolean mOpenCvCameraRunning = false;

        /// <summary> OpenCvで映像取り込みをするコーデック指定 </summary>
        private FourCC mOpenCvCodecFourCc;
        //private FourCC mOpenCvCodecFourCc = FourCC.MP4V; // MP4形式

        private Boolean mOpencvBackGroundStatus = false;

        // 追加
        // CPU割り当てに必要なモジュール
        // ----------------------------------------------------------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr dwProcessAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentThread();

        public Int32 DatacoreNum = 4;

        private int mThreadCoreNum = 0;
        // ----------------------------------------------------------------------------
        private int mShotCount = 0;
        private int mFrameCount = 0;
        TimeSpan mTimeSpan;

        /// <summary> フレームバッファ(常時一枚) </summary>
        private Bitmap mImageShot = new Bitmap(Program.WIDTH, Program.HEIGHT);

        /// <summary> 受信フレームの動画ファイル保存を行うバックグラウンド処理 </summary>
        private BackgroundWorker mSaveMovieBackGroundWorker;
        private Boolean mSaveMovieBackGroundStatus = false;
        private Boolean mSaveMovieBackGroundWorkingStatus = false;
        private Boolean mSaveMovieBackGroundSaverStatus = false;
        private Boolean mBackGroundSetCore = false;

        private DateTime mDtSendImage;
        #endregion

        #region <Property>
        public Boolean IsAliveCamera { get { return this.mIsAliveCamera; } }
        #endregion

        #region <Constructor>

        public WebCamCtrlThread(int cycleTime, int threadIndex)
        {
            this.CycleTime = cycleTime;
            this.mThreadIndex = threadIndex;
            mThreadAlive = false;
            mOpenCvCodecFourCc = FourCC.MJPG;

            //saveThread = new WebCamVideoSaveThread(cycleTime, threadIndex);
            pr = ThreadPriority.Highest;
        }

        #endregion

        #region <Method>

        protected override void Proc()
        {
            if (mThreadAlive)
            {
                mIsAliveCamera = mVideoSource != null ? mVideoSource.IsRunning : false;
            }
            else
            {
                mIsAliveCamera = false;
            }
            base.Proc();
        }

        protected override void QueueEvent(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }
                else if (obj is CmdWebcamCtrl)
                {
                    switch (((CmdWebcamCtrl)obj).mStatus)
                    {
                        case WebCamCtrlThreadStatus.INIT:
                            // カメラ
                            // このスレッドが管理するカメラのデバイスIDをメンバ保持
                            mCameraDeviceId = ((CmdWebcamCtrl)obj).mCameraId;
                            //GetAllCameraDevice(mCameraDeviceId);
                            mFps = ((CmdWebcamCtrl)obj).mFps;
                            mTimeSpan = TimeSpan.FromMilliseconds(1000 / MOVIE_FPS);
                            mThreadAlive = true;
                            logger.Debug(String.Format("【INIT】カメラ情報取得。スレッドインデックス：{0}, カメラデバイスID：{1}, FPS設定：{2}", mThreadIndex, mCameraDeviceId, mFps));

                            break;
                        case WebCamCtrlThreadStatus.TERM:
                            logger.Debug(String.Format("【TERM】カメラ停止処理開始。スレッドインデックス：{0}", mThreadIndex));

                            if (mVideoSource != null)
                            {
                                mVideoSource.Stop();
                            }
                            logger.Debug(String.Format("【TERM】カメラ停止処理完了。スレッドインデックス：{0}", mThreadIndex));


                            //mOpenCvCameraRunning = false;
                            //System.Threading.Thread.Sleep(100);
                            //if (mOpenCvBackGroundWorker != null)
                            //{
                            //    mOpenCvBackGroundWorker.Dispose();
                            //}
                            //System.Threading.Thread.Sleep(100);
                            break;
                        case WebCamCtrlThreadStatus.SHIFT_START:
                            if (mThreadAlive)
                            {
                                mListCameraUsingStatusByProcess = ((CmdWebcamCtrl)obj).mCamSaveConfig;
                                mOperationShift = ((CmdWebcamCtrl)obj).mShiftNum;

                                // このスレッドが管理するカメラを使用する工程のストリームを開く
                                mIsInitCamera = false;
                                if (InitCameraDevice(mCameraDeviceId))
                                {
                                    mIsInitCamera = true;
                                    mFileDateTime = ((CmdWebcamCtrl)obj).mCycleDateTime;

                                    InitAllMovieFileStream(mFileDateTime);

                                    // このスレッドが管理するカメラのデバイスIDをメンバ保持し、取り込みを開始
                                    mSaveMovieBackGroundWorkingStatus = true;
                                    mVideoSource.Start();
                                    mSaveMovieBackGroundWorker.RunWorkerAsync();

                                    //mOpenCvBackGroundWorker.RunWorkerAsync();
                                    //mOpenCvCameraRunning = true;
                                }
                                else
                                {
                                    mIsInitCamera = false;
                                    logger.Warn(String.Format("【SHIFT_START】本スレッドが管理するカメラの初期化失敗。スレッドインデックス：{0}, カメラデバイスID：{1}", mThreadIndex, mCameraDeviceId));
                                }
                            }
                            break;
                        case WebCamCtrlThreadStatus.SHIFT_END:

                            if (mThreadAlive)
                            {
                                if (mIsInitCamera)
                                {
                                    logger.Debug(String.Format("【SHIFT_END】直終了受信。カメラスレッドインデックス：{0}", mThreadIndex));
                                    // 取り込みの停止
                                    mVideoSource.Stop();

                                    mSaveMovieBackGroundWorkingStatus = false;
                                    do
                                    {
                                        if (!mSaveMovieBackGroundStatus && !mSaveMovieBackGroundSaverStatus)
                                        {
                                            break;
                                        }
                                        System.Threading.Thread.Sleep(100);
                                    } while (true);
                                    mSaveMovieBackGroundWorker.Dispose();

                                    //mOpenCvCameraRunning = false;
                                    //System.Threading.Thread.Sleep(100);
                                    //do
                                    //{
                                    //    if (!mOpencvBackGroundStatus)
                                    //    {
                                    //        logger.Debug(String.Format("【SHIFT_END】バックグラウンド処理終了検知。カメラスレッドインデックス：{0}", mThreadIndex));
                                    //        break;
                                    //    }
                                    //} while (true);

                                    if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                                    {
                                        logger.Info(String.Format("【枚数】撮像：{0}, ショット：{1}, カメラスレッド:{2}", mShotCount, mFrameCount, mThreadIndex));
                                        mShotCount = 0;
                                        mFrameCount = 0;
                                        for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                                        {
                                            DateTime start = mDicCycleDatetimeOnProcess[i];
                                            DateTime end = ((CmdWebcamCtrl)obj).mCycleDateTime;
                                            DateTime operationDate = ((CmdWebcamCtrl)obj).mOperationDateTime;

                                            int camUseStatus = mListCameraUsingStatusByProcess[i];

                                            DataRow drWorkShift = ((CmdWebcamCtrl)obj).mShiftTable.Rows[i];
                                            Guid compId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.COMPOSITION_ID);
                                            Guid prodId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.PRODUCT_TYPE_ID);
                                            String worker = drWorkShift.Field<String>(DBConnect.SQL.ColumnWorkingStatus.WORKER_NAME);

                                            if (mListCameraUsingStatusByProcess[i] > 0)
                                            {
                                                mDicVideoWriteStreamOnProcess[i].Release();
                                                mDicVideoWriteStreamOnProcess[i].Dispose();
                                                logger.Debug(String.Format("【SHIFT_END】イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, i, start.ToString("HH:mm.ss.fff")));
                                                DelegateEndMethod(mDicVideoWriteStreamOnProcess[i], i, start, end, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebcamCtrl)obj).mShiftTable, ((CmdWebcamCtrl)obj).mDtCycleDataMst);

                                            }
                                            else
                                            {
                                                logger.Debug(String.Format("【SHIFT_END】イベント発行(カメラ不使用設定)。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, i, start.ToString("HH:mm.ss.fff")));
                                                DelegateEndMethod(null, i, start, end, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebcamCtrl)obj).mShiftTable, ((CmdWebcamCtrl)obj).mDtCycleDataMst);
                                            }
                                        }
                                    }
                                    //mOpenCvBackGroundWorker.Dispose();
                                }
                                else
                                {
                                    logger.Debug(String.Format("【SHIFT_END】本スレッド管理するカメラが初期化されていません。スレッドインデックス：{0}", mThreadIndex));
                                }
                            }
                            mThreadAlive = false;
                            break;
                        case WebCamCtrlThreadStatus.CYCLE:
                            if (mThreadAlive)
                            {
                                int processIdx = ((CmdWebcamCtrl)obj).mProcessIdx;
                                if (mIsInitCamera)
                                {
                                    //saveThread.AddEvent(new CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus.STREAM_CYCLE, ((CmdWebcamCtrl)obj).mCycleDateTime, ((CmdWebcamCtrl)obj).mOperationDateTime, ((CmdWebcamCtrl)obj).mProcessIdx, ((CmdWebcamCtrl)obj).mShiftTable, ((CmdWebcamCtrl)obj).mDtCycleDataMst));

                                    #region
                                    if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                                    {
                                        logger.Debug(String.Format("【CYCLE】イベント受信。スレッドインデックス：{0}, 工程インデックス：{1} ", mThreadIndex, processIdx));
                                        int camUseStatus = mListCameraUsingStatusByProcess[processIdx];
                                        DateTime start = mDicCycleDatetimeOnProcess[processIdx];
                                        DateTime nextStart = ((CmdWebcamCtrl)obj).mCycleDateTime;
                                        DateTime operationDate = ((CmdWebcamCtrl)obj).mOperationDateTime;
                                        DataRow drWorkShift = ((CmdWebcamCtrl)obj).mShiftTable.Rows[processIdx];
                                        Guid compId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.COMPOSITION_ID);
                                        Guid prodId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.PRODUCT_TYPE_ID);
                                        String worker = drWorkShift.Field<String>(DBConnect.SQL.ColumnWorkingStatus.WORKER_NAME);


                                        if (camUseStatus > 0)
                                        {
                                            VideoWriter vw = mDicVideoWriteStreamOnProcess[processIdx];
                                            {
                                                logger.Debug(String.Format("【CYCLE】イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));

                                                DelegateEndMethod(vw, processIdx, start, nextStart, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebcamCtrl)obj).mShiftTable, ((CmdWebcamCtrl)obj).mDtCycleDataMst);
                                                mFileDateTime = nextStart;

                                                logger.Info(String.Format("【枚数】撮像：{0}, ショット：{1}, カメラスレッド:{2}", mShotCount, mFrameCount, mThreadIndex));
                                                mShotCount = 0;

                                                InitMovieFileStream(nextStart, processIdx);
                                                mFrameCount = 0;
                                            }
                                        }
                                        else
                                        {
                                            logger.Debug(String.Format("【CYCLE】イベント発行(カメラ不使用設定)。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));
                                            DelegateEndMethod(null, processIdx, start, nextStart, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebcamCtrl)obj).mShiftTable, ((CmdWebcamCtrl)obj).mDtCycleDataMst);
                                        }
                                        mDicCycleDatetimeOnProcess[processIdx] = nextStart;
                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("【CYCLE】工程に使用するカメラ設定がありません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                    }
                                    #endregion
                                }
                                else
                                {
                                    logger.Debug(String.Format("【CYCLE】工程に使用するカメラが初期化されていません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【QueueEvent】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// コア割り当て処理
        /// </summary>
        /// <param name="numCore"></param>
        /// <returns></returns>
        public Boolean SetCpuCore(int numCore)
        {
            try
            {
                if (Environment.ProcessorCount >= 16)
                {
                    logger.Info(String.Format("【SetCpuCore】CPUコア割り当て。割り当てコア番号：{0}", numCore));
                    // 割り当てるコア
                    Int32 core;
                    switch (numCore)
                    {
                        case 2:
                            core = 0x04;
                            break;
                        case 3:
                            core = 0x08;
                            break;
                        case 4:
                            core = 0x10;
                            break;
                        case 5:
                            core = 0x20;
                            break;
                        case 6:
                            core = 0x40;
                            break;
                        case 7:
                            core = 0x80;
                            break;
                        case 8:
                            core = 0x100;
                            break;
                        case 9:
                            core = 0x200;
                            break;
                        case 10:
                            core = 0x400;
                            break;
                        default:
                            return false;
                    }
                    IntPtr th = GetCurrentThread();
                    SetThreadAffinityMask(th, new UIntPtr((uint)core));
                    mThreadCoreNum = core;
                }
                else
                {
                    logger.Info(String.Format("【SetCpuCore】コア割り当てなし。認識プロセッサ数：{0}", Environment.ProcessorCount));
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【SetCpuCore】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }
        private Boolean SetCore()
        {
            try
            {

                IntPtr th = GetCurrentThread();
                SetThreadAffinityMask(th, new UIntPtr((uint)mThreadCoreNum));
                mBackGroundSetCore = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【SetCpuCore】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }


        /// <summary>
        /// 管理PCに接続されているすべてのカメラの接続情報を取得する
        /// </summary>
        /// <param name="camId"></param>
        /// <returns></returns>
        private Boolean GetAllCameraDevice(String camId)
        {
            Boolean ret = false;
            try
            {
                logger.Info(String.Format("【GetAllCameraDevice】カメラ情報の取得開始。カメラスレッドインデックス：{0}", mThreadIndex));
                // 現在接続されているUSBカメラを取得する
                using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPSignedDriver WHERE DeviceID LIKE 'USB%' AND DeviceClass = 'Camera'"))
                {
                    mListCameraDevice.Clear();
                    // PDO順がカメラインデックス順　※OpenCVで使えないカメラも含まれるので、このインデックス＝OpenCVのOpenで使うインデックスではない
                    foreach (var device in searcher.Get().Cast<ManagementObject>().OrderBy(n => n["PDO"]))
                    {
                        String deviceId = device.GetPropertyValue("DeviceID").ToString();
                        String deviceName = device.GetPropertyValue("FriendlyName") != null ? device.GetPropertyValue("FriendlyName").ToString() : NO_CAM_NAME;

                        // OpenCVで使用不可のカメラ(赤外線カメラなど)はあらかじめはじく
                        if (deviceName.Contains("IR"))
                        {
                            continue;
                        }
                        mListCameraDevice.Add(new StructCameraDevice(deviceName, deviceId));
                    }
                }
                logger.Info(String.Format("【GetAllCameraDevice】カメラ情報の取得終了。カメラスレッドインデックス：{0}, 取得件数：{1}", mThreadIndex, mListCameraDevice.Count));

                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            logger.Info(String.Format("【GetAllCameraDevice】カメラ情報の取得完了。カメラスレッドインデックス：{0}, 取得ステータス:{1}", mThreadIndex, ret));
            return ret;
        }

        /// <summary>
        /// このスレッドで管理・使用するカメラの初期化処理を行う
        /// </summary>
        /// <param name="cameraId">カメラID</param>
        /// <returns></returns>
        private Boolean InitCameraDevice(String cameraId)
        {
            Boolean ret = false;
            Boolean isGetCamInfo = false;
            try
            {
                logger.Info(String.Format("【InitCameraDevice】カメラの初期化開始。カメラスレッドインデックス：{0}", mThreadIndex));
                if (cameraId != null && cameraId != String.Empty)
                {
                    //対象のカメラの取得
                    FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    foreach (FilterInfo device in videoDevices)
                    {
                        // カメラのデバイスIDをContainで取得するため、\マークの変換と全文字大文字化を実施する
                        String camId = cameraId.Replace('\\', '#');
                        String camIdUpper = camId.ToUpper();
                        String deviceIdUpper = device.MonikerString.ToUpper();

                        if (deviceIdUpper.Contains(camIdUpper))
                        {
                            mVideoSource = new VideoCaptureDevice(device.MonikerString);
                            isGetCamInfo = true;
                            break;
                        }
                    }
                    if (!isGetCamInfo)
                    {
                        // カメラの特定に失敗
                        logger.Error(String.Format("【ERROR】カメラの特定に失敗"));
                        return false;
                    }
                    mVideoSource.NewFrame += new NewFrameEventHandler(this.VideoNewFrame);

                    //カメラのプロパティ情報取得
                    var videoCapabilities = mVideoSource.VideoCapabilities;
                    isGetCamInfo = false;
                    foreach (VideoCapabilities capabilty in videoCapabilities)
                    {
                        if (capabilty.FrameSize.Width == Program.WIDTH && capabilty.FrameSize.Height == Program.HEIGHT)
                        {
                            //使うカメラ設定を指定
                            mVideoSource.VideoResolution = capabilty;
                            isGetCamInfo = true;
                            break;
                        }
                    }
                    if (!isGetCamInfo)
                    {
                        // カメラのプロパティ情報取得に失敗
                        logger.Error(String.Format("【ERROR】カメラのプロパティ情報取得に失敗"));
                        return false;
                    }
                }
                mSaveMovieBackGroundWorker = new BackgroundWorker();
                mSaveMovieBackGroundWorker.WorkerReportsProgress = true;
                mSaveMovieBackGroundWorker.ProgressChanged += MovieSaverProgressChanged;
                mSaveMovieBackGroundWorker.DoWork += MovieSaverProgressDoWork;
                mBackGroundSetCore = false;

                #region <NO USE >
                /*
                if (mListCameraDevice == null || mListCameraDevice.Count < 1)
                {
                    // 初期化に進入させない
                    logger.Info(String.Format("【InitCameraDevice】カメラデバイスリスト無し。カメラスレッドインデックス：{0}", mThreadIndex));
                    return ret;
                }
                if (cameraId != null && cameraId != String.Empty)
                {
                    String camIdUpper = cameraId.ToUpper();
                    for (int i = 0; i < mListCameraDevice.Count; i++)
                    {
                        logger.Debug(String.Format("Check Camera(スレッド:{0}).\ncameraId：{1}\nListCamId：{2}", mThreadIndex, cameraId, mListCameraDevice[i].stmCameraId));
                        if (mListCameraDevice[i].stmCameraId.Contains(camIdUpper))
                        {
                            isGetCamInfo = true;
                            mOpenCvCameraIndex = i;
                            break;
                        }
                    }
                    if (!isGetCamInfo)
                    {
                        // カメラの特定に失敗
                        logger.Error(String.Format("【InitCameraDevice】カメラの特定に失敗。カメラスレッドインデックス：{0}", mThreadIndex));
                        return false;
                    }
                    mOpenCvBackGroundWorker = new BackgroundWorker();
                    mOpenCvBackGroundWorker.WorkerReportsProgress = true;
                    mOpenCvBackGroundWorker.ProgressChanged += WorkerProgressChanged;
                    mOpenCvBackGroundWorker.DoWork += WorkeDoWork;
                }
                else
                {
                    if (cameraId == null)
                    {
                        logger.Error(String.Format("【InitCameraDevice】指定したカメラIDがNULL。カメラスレッドインデックス：{0}", mThreadIndex));
                        return false;
                    }
                    else if (cameraId == String.Empty)
                    {
                        logger.Error(String.Format("【InitCameraDevice】指定したカメラIDが空文字。カメラスレッドインデックス：{0}", mThreadIndex));
                    }
                }
                */
                #endregion

                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【InitCameraDevice】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            logger.Info(String.Format("【InitCameraDevice】カメラの初期化終了。カメラスレッドインデックス：{0}, 初期化ステータス:{1}", mThreadIndex, ret));
            return ret;
        }



        /// <summary>
        /// このスレッドで管理する全ファイルストリームの初期化を行う
        /// </summary>
        /// <param name="fileDateTime">サイクルの開始時刻</param>
        /// <returns></returns>
        private Boolean InitAllMovieFileStream(DateTime fileDateTime)
        {
            Boolean ret = false;
            try
            {
                mDicVideoWriteStreamOnProcess.Clear();
                mDicCycleDatetimeOnProcess.Clear();
                for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                {
                    if (mListCameraUsingStatusByProcess[i] > 0)
                    {
                        VideoWriter vw = InitCameraFileStreamProcess(fileDateTime, i);
                        mDicVideoWriteStreamOnProcess.Add(i, vw);
                    }
                    mDicCycleDatetimeOnProcess.Add(i, fileDateTime);
                }
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// このスレッドで管理する特定工程のファイルストリームの初期化を行う
        /// </summary>
        /// <param name="fileDateTime">サイクルの開始時刻</param>
        /// <param name="processIdx">対象の工程インデックス</param>
        /// <returns></returns>
        private Boolean InitMovieFileStream(DateTime fileDateTime, int processIdx)
        {
            Boolean ret = false;
            try
            {
                if (mListCameraUsingStatusByProcess[processIdx] > 0)
                {
                    logger.Debug(String.Format("特定工程のビデオストリームの初期化。カメラスレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, processIdx, fileDateTime.ToString("HH:mm.ss.fff")));
                    VideoWriter vw = InitCameraFileStreamProcess(fileDateTime, processIdx);
                    mDicVideoWriteStreamOnProcess[processIdx] = vw;
                }
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// カメラ映像取得時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void VideoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var img = eventArgs.Frame;
                //String videoName = "";
                lock (mImageShot)
                {
                    mImageShot = (Bitmap)img.Clone();
                }

                //// 新しい画像を取得したのでReportProgressメソッドを使って、ProgressChangedイベントを発生させる
                //mSaveMovieBackGroundWorker.ReportProgress(0, (Bitmap)img.Clone());

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【VideoNewFrame】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
            mShotCount++;
        }


        /// <summary>
        /// ビデオファイルストリームの初期化を行う(OpenCV)
        /// </summary>
        /// <param name="fileDateTime"></param>
        /// <param name="processidx"></param>
        private VideoWriter InitCameraFileStreamProcess(DateTime fileDateTime, int processidx)
        {
            VideoWriter vwRet = null;
            try
            {
                // TEMP_AVI_VIDEO_FILE_NAME_FORMAT
                String videoNameTemp = String.Format(Program.TEMP_AVI_VIDEO_FILE_NAME_FORMAT, fileDateTime.ToString("yyyyMMdd_HHmmss"), processidx + 1, mThreadIndex + 1);
                //String videoNameTemp = String.Format(Program.TEMP_VIDEO_FILE_NAME_FORMAT, fileDateTime.ToString("yyyyMMdd_HHmmss"), processidx + 1, mThreadIndex + 1);
                String tempDir = Program.AppSetting.WebcamParam.TempFilePath;
                String videoName = System.IO.Path.Combine(tempDir, videoNameTemp);
                vwRet = new VideoWriter(videoName, mOpenCvCodecFourCc, MOVIE_FPS, new OpenCvSharp.Size(Program.WIDTH, Program.HEIGHT));
                logger.Debug(String.Format("【InitCameraFileStreamProcess】ビデオファイルストリームオープン。対象ファイル：{0}", videoName));

                //__STREAM_COUNT__ = 0;

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【InitCameraFileStreamProcess】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
            return vwRet;
        }

        /// <summary>
        /// 保存した映像をイベント管理スレッドに投げ、結合処理に回す
        /// </summary>
        /// <param name="vw"></param>
        /// <param name="processIdx"></param>
        /// <param name="cycleStartTime"></param>
        /// <param name="cycleEndTime"></param>
        /// <param name="operationDateTime"></param>
        /// <param name="saveStatus"></param>
        /// <param name="comp"></param>
        /// <param name="prod"></param>
        /// <param name="worker"></param>
        [HandleProcessCorruptedStateExceptions]
        private void DelegateEndMethod(VideoWriter vw, int processIdx, DateTime cycleStartTime, DateTime cycleEndTime, DateTime operationDateTime, int saveStatus, Guid comp, Guid prod, String worker, DataTable shiftTable, DataTable cycleDataMst)
        {
            List<String> errorList = new List<string>();
            try
            {
                if (vw != null)
                {
                    lock (vw)
                    {
                        String fileName = vw != null ? vw.FileName : "";
                        if (vw != null)
                        {
                            vw.Dispose();
                        }
                        while (true)
                        {
                            if (vw == null || vw.IsDisposed)
                            {
                                // 現在取り込んでいるビデオストリームは出力イベントを発行して書き込みさせる
                                logger.Debug(String.Format("【DelegateEndMethod】動画ファイル保存完了。イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}, サイクル終了時刻：{3}",
                                    mThreadIndex, processIdx, cycleStartTime.ToString("HH:mm.ss.fff"), cycleEndTime.ToString("HH:mm.ss.fff")));
                                CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(cycleStartTime, cycleEndTime);
                                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_MOVIE_SAVE, fileName, errorList, SaveFileType.MOVIE, mThreadIndex, processIdx, cycleTime, mOperationShift, saveStatus, operationDateTime, comp, prod, worker, shiftTable, cycleDataMst));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    logger.Debug(String.Format("【DelegateEndMethod】動画ファイル保存なし。イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}, サイクル終了時刻：{3}",
                                   mThreadIndex, processIdx, cycleStartTime.ToString("HH:mm.ss.fff"), cycleEndTime.ToString("HH:mm.ss.fff")));
                    CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(cycleStartTime, cycleEndTime);
                    Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_MOVIE_SAVE, "", errorList, SaveFileType.MOVIE, mThreadIndex, processIdx, cycleTime, mOperationShift, saveStatus, operationDateTime, comp, prod, worker, shiftTable, cycleDataMst));

                }
            }
            catch (Exception avex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", avex.Message, avex.StackTrace));
                CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(cycleStartTime, cycleEndTime);
                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_MOVIE_SAVE, "", errorList, SaveFileType.MOVIE, mThreadIndex, processIdx, cycleTime, mOperationShift, saveStatus, operationDateTime, comp, prod, worker, shiftTable, cycleDataMst));
            }
        }

        /// <summary>
        /// OpenCVバックグラウンド処理(映像受信)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkeDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                logger.Debug(String.Format("【WorkeDoWork】バックグラウンド処理開始点。カメラスレッドインデックス：{0} ", mThreadIndex));
                // カメラからの映像を受け取る
                using (VideoCapture capture = new VideoCapture(mOpenCvCameraIndex))
                {
                    logger.Debug(String.Format("【WorkeDoWork】カメラ認識完了。カメラスレッドインデックス：{0}, OpenCVカメラ認識インデックス：{1} ", mThreadIndex, mOpenCvCameraIndex));
                    //カメラ画像取得用のVideoCapture作成
                    capture.FrameWidth = Program.WIDTH;
                    capture.FrameHeight = Program.HEIGHT;
                    capture.Set(VideoCaptureProperties.Fps, mFps);
                    logger.Debug(String.Format("【WorkeDoWork】映像取得オープン。カメラスレッドインデックス：{0}, Program.WIDTH：{1}, Program.HEIGHT：{2}, FPS：{3}",
                        mThreadIndex, capture.Get(VideoCaptureProperties.FrameWidth), capture.Get(VideoCaptureProperties.FrameHeight), capture.Get(VideoCaptureProperties.Fps)));

                    logger.Debug(String.Format("【WorkeDoWork】バックグラウンドループ処理開始。カメラスレッドインデックス：{0} ", mThreadIndex));
                    mOpencvBackGroundStatus = true;
                    while (true)
                    {
                        Mat frame = new Mat(Program.HEIGHT, Program.WIDTH, MatType.CV_8UC3);
                        mIsAliveCamera = capture.IsOpened();
                        if (capture.IsOpened() == false)
                        {
                            logger.Error(String.Format("【WorkeDoWork】カメラ接続異常。カメラスレッドインデックス：{0} ", mThreadIndex));
                        }

                        if (mOpenCvCameraRunning == false)
                        {
                            logger.Debug(String.Format("【WorkeDoWork】バックグラウンドループ処理break。カメラスレッドインデックス：{0} ", mThreadIndex));
                            break;
                        }

                        bool ret = capture.Read(frame);
                        if (ret == false)
                        {
                            logger.Error(String.Format("映像取得失敗。カメラスレッド：{0}", mThreadIndex));
                        }
                        else
                        {
                            // 新しい画像を取得したのでReportProgressメソッドを使って、ProgressChangedイベントを発生させる
                            mOpenCvBackGroundWorker.ReportProgress(0, frame);
                        }
                        //frame.Dispose();
                        //System.GC.Collect();
                    }
                    mIsAliveCamera = false;
                    Program.frm.CapBitMap = null;
                    mOpencvBackGroundStatus = false;
                }

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 動画保存 ProgressChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                //  frameがe.UserStateプロパティにセットされて渡されてくる
                Mat frame = (Mat)e.UserState;
                {
                    String videoName = "";
                    // 動画ファイルに書き込み
                    for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                    {
                        //このカメラを使用する対象工程か否かを取り、対象工程ならば書き込みをする
                        if (mListCameraUsingStatusByProcess[i] > 0)
                        {
                            try
                            {
                                videoName = mDicVideoWriteStreamOnProcess[i].FileName;
                                VideoWriter vw = mDicVideoWriteStreamOnProcess[i];
                                lock (vw)
                                {
                                    if (!mDicVideoWriteStreamOnProcess[i].IsDisposed && mDicVideoWriteStreamOnProcess[i].IsOpened())
                                    {
                                        if (mOpenCvCameraRunning)
                                        {
                                            vw.Write(frame);
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("Disposed Object. カメラスレッドインデックス：{0}", mThreadIndex));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(String.Format("【WorkerProgressChanged】映像取得失敗。{0}, スレッド：{1}, 対象ファイル：{2}, 詳細：{3}", ex.Message, mThreadIndex, videoName, ex.StackTrace));
                            }
                        }
                    }
                }
                if (mOpenCvCameraIndex == Program.frm.SelectCapBitMap)
                {
                    Program.frm.CapBitMap = BitmapConverter.ToBitmap(frame);
                }
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【WorkerProgressChanged】映像取得失敗。{0}, スレッド：{1}, 詳細：{2}", ex.Message, mThreadIndex, ex.StackTrace));
            }
        }


        private void MovieSaverProgressDoWork(object sender, DoWorkEventArgs e)
        {
            int filter = 10;
            mSaveMovieBackGroundStatus = true;
            mDtSendImage = DateTime.Now;
            while (true)
            {
                try
                {
                    if (!mSaveMovieBackGroundWorkingStatus)
                    {
                        break;
                    }
                    // 新しい画像を取得したのでReportProgressメソッドを使って、ProgressChangedイベントを発生させる
                    //if (mImageShot != null)
                    if (DateTime.Now >= mDtSendImage)
                    {
                        //logger.Debug(String.Format("【IMAGE SEND】スレッド：{0}, 時刻：{1}", mThreadIndex, mDtSendImage.ToString("HH:mm:ss.fff")));
                        lock (mImageShot)
                        {
                            mSaveMovieBackGroundWorker.ReportProgress(0, (Bitmap)mImageShot.Clone());
                        }
                        mDtSendImage += mTimeSpan;
                        mFrameCount++;
                    }
                    System.Threading.Thread.Sleep(filter);
                }
                catch (Exception ex)
                {
                    logger.Error(String.Format("【MovieSaverProgressDoWork】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                }
            }
            mSaveMovieBackGroundStatus = false;
        }


        /// <summary>
        /// 動画保存機能のバックグラウンド処理化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="frame"></param>
        [HandleProcessCorruptedStateExceptions]
        private void MovieSaverProgressChanged(object sender, ProgressChangedEventArgs frame)
        {
            mSaveMovieBackGroundSaverStatus = true;
            var img = (Bitmap)frame.UserState;
            String videoName = "";
            using (Mat mat = img.ToMat())
            {

                // 動画ファイルに書き込み
                for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                {
                    //このカメラを使用する対象工程か否かを取り、対象工程ならば書き込みをする
                    if (mListCameraUsingStatusByProcess[i] > 0)
                    {
                        try
                        {
                            videoName = mDicVideoWriteStreamOnProcess[i].FileName;
                            VideoWriter vw = mDicVideoWriteStreamOnProcess[i];
                            lock (vw)
                            {
                                if (!mDicVideoWriteStreamOnProcess[i].IsDisposed && mDicVideoWriteStreamOnProcess[i].IsOpened())
                                {
                                    vw.Write(mat);
                                }
                                else
                                {
                                    logger.Debug(String.Format("【MovieSaverProgressChanged】オブジェクト破棄済。カメラスレッドインデックス：{0}", mThreadIndex));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(String.Format("【MovieSaverProgressChanged】映像取得失敗。{0}, スレッド：{1}, 対象ファイル：{2}, 詳細：{3}", ex.Message, mThreadIndex, videoName, ex.StackTrace));
                            break;
                        }
                    }
                }
                System.GC.Collect();
            }
            mSaveMovieBackGroundSaverStatus = false;
        }
        #endregion

    }

    /// <summary>
    /// Webカメラ通信スレッドコマンドクラス
    /// </summary>
    public class CmdWebcamCtrl
    {
        public WebCamCtrlThreadStatus mStatus;
        public String mCameraId;
        public DateTime mCycleDateTime;
        public int mProcessCount;
        public Boolean mAliveStatus;
        public List<int> mCamSaveConfig;
        public Double mFps;
        public int mProcessIdx;
        public int mShiftNum;
        public DataTable mShiftTable;
        public DateTime mOperationDateTime;
        public DataTable mDtCycleDataMst;

        /// <summary> デフォルトのイベントコンストラクタ </summary>
        public CmdWebcamCtrl(WebCamCtrlThreadStatus status)
        {
            mStatus = status;
        }

        /// <summary> INIT時のイベントコンストラクタ </summary>
        public CmdWebcamCtrl(WebCamCtrlThreadStatus status, String cameraId, Double fps, Boolean alive)
        {
            mStatus = status;
            mCameraId = cameraId;
            mFps = fps;
            mAliveStatus = alive;
        }

        /// <summary> SHIFT_START時のイベントコンストラクタ </summary>
        public CmdWebcamCtrl(WebCamCtrlThreadStatus status, DateTime shiftStartTime, int processCount, List<int> camSaveConfig, int shift)
        {
            mStatus = status;
            mCycleDateTime = shiftStartTime;
            mProcessCount = processCount;
            mCamSaveConfig = camSaveConfig;
            mShiftNum = shift;
        }

        /// <summary> CYCLE時のイベントコンストラクタ </summary>
        public CmdWebcamCtrl(WebCamCtrlThreadStatus status, DateTime cycleStartTime, DateTime operationDateTime, int processIdx, DataTable shiftTable, DataTable cycleDataMstTable)
        {
            mStatus = status;
            mCycleDateTime = cycleStartTime;
            mProcessIdx = processIdx;
            mShiftTable = shiftTable;
            mOperationDateTime = operationDateTime;
            mDtCycleDataMst = cycleDataMstTable;
        }
        /// <summary> SHIFT_END時のイベントコンストラクタ </summary>
        public CmdWebcamCtrl(WebCamCtrlThreadStatus status, DateTime cycleEndTime, DateTime operationDateTime, DataTable shiftTable, DataTable cycleDataMstTable)
        {
            mStatus = status;
            mCycleDateTime = cycleEndTime;
            mShiftTable = shiftTable;
            mOperationDateTime = operationDateTime;
            mDtCycleDataMst = cycleDataMstTable;
        }

    }
}
