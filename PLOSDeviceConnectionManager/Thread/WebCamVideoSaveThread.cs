using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using log4net;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Data;
using System.Threading;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum WebCamVideoSaveThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> 直開始 </summary>
        STREAM_START,
        /// <summary> 直終了 </summary>
        STREAM_END,
        /// <summary> サイクル </summary>
        STREAM_CYCLE,
        /// <summary> 画像送信 </summary>
        SEND_IMAGE,
    }

    /// <summary>
    /// Webカメラ動画保存スレッドクラス
    /// </summary>
    public class WebCamVideoSaveThread : WorkerThread
    {
        #region <Field>

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region << CPU割り当てに必要なモジュール >>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr dwProcessAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentThread();

        public Int32 DatacoreNum = 4;

        private int MAX_THREAD = 7;

        private int mThreadCoreNum = 0;
        Boolean mBackGroundSetCore = false;

        #endregion

        /// <summary> 自スレッドインデックス </summary>
        private int mThreadIndex = -1;

        /// <summary> 現在のシフト番号(1 ～ 3) </summary>
        private int mOperationShift = -1;

        /// <summary> 対象カメラのFPS情報 </summary>
        private Double mFps = 30;

        /// <summary> 工程ごとに保持するカメラの使用設定(0:不使用,1:使用,2:異常時のみ)。リスト数は工程数と一致する </summary>
        private List<int> mListCameraUsingStatusByProcess = new List<int>();

        /// <summary> 工程ごとに保持する録画開始時刻 </summary>
        private Dictionary<int, DateTime> mDicCycleDatetimeOnProcess = new Dictionary<int, DateTime>();

        /// <summary> 工程ごとに保持する映像ストリーム </summary>
        private Dictionary<int, VideoWriter> mDicVideoWriteStreamOnProcess = new Dictionary<int, VideoWriter>();
        private DateTime mFileDateTime;
        /// <summary> OpenCvで映像取り込みをするコーデック指定 </summary>
        private FourCC mFourCc = FourCC.MJPG;
        #endregion

        #region <Property>
        #endregion

        #region <Constructor>
        public WebCamVideoSaveThread(int cycleTime, int threadIndex)
        {
            this.CycleTime = cycleTime;
            mThreadIndex = threadIndex;

            //pr = ThreadPriority.Highest;
        }
        #endregion

        #region <Method>
        protected override void Proc()
        {
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
                else if (obj is CmdWebCamVideoSaveThread)
                {
                    switch (((CmdWebCamVideoSaveThread)obj).mStatus)
                    {
                        case WebCamVideoSaveThreadStatus.INIT:
                            break;
                        case WebCamVideoSaveThreadStatus.STREAM_START:
                            mFileDateTime = ((CmdWebCamVideoSaveThread)obj).mCycleDateTime;
                            mListCameraUsingStatusByProcess = ((CmdWebCamVideoSaveThread)obj).mCamSaveConfig;

                            InitAllMovieFileStream(mFileDateTime);
                            break;
                        case WebCamVideoSaveThreadStatus.STREAM_END:
                            if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                            {
                                for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                                {
                                    DateTime start = mDicCycleDatetimeOnProcess[i];
                                    DateTime end = ((CmdWebCamVideoSaveThread)obj).mCycleDateTime;
                                    DateTime operationDate = ((CmdWebCamVideoSaveThread)obj).mOperationDateTime;

                                    int camUseStatus = mListCameraUsingStatusByProcess[i];

                                    DataRow drWorkShift = ((CmdWebCamVideoSaveThread)obj).mShiftTable.Rows[i];
                                    Guid compId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.COMPOSITION_ID);
                                    Guid prodId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.PRODUCT_TYPE_ID);
                                    String worker = drWorkShift.Field<String>(DBConnect.SQL.ColumnWorkingStatus.WORKER_NAME);

                                    if (mListCameraUsingStatusByProcess[i] > 0)
                                    {
                                        mDicVideoWriteStreamOnProcess[i].Release();
                                        mDicVideoWriteStreamOnProcess[i].Dispose();
                                        logger.Debug(String.Format("【SHIFT_END】イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, i, start.ToString("HH:mm.ss.fff")));
                                        DelegateEndMethod(mDicVideoWriteStreamOnProcess[i], i, start, end, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebCamVideoSaveThread)obj).mShiftTable, ((CmdWebCamVideoSaveThread)obj).mDtCycleDataMst);

                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("【SHIFT_END】イベント発行(カメラ不使用設定)。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, i, start.ToString("HH:mm.ss.fff")));
                                        DelegateEndMethod(null, i, start, end, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebCamVideoSaveThread)obj).mShiftTable, ((CmdWebCamVideoSaveThread)obj).mDtCycleDataMst);
                                    }
                                }
                            }
                            else
                            {
                                logger.Debug(String.Format("【SHIFT_END】イベント発行なし。スレッドインデックス：{0}", mThreadIndex));
                            }
                            break;
                        case WebCamVideoSaveThreadStatus.STREAM_CYCLE:

                            int processIdx = ((CmdWebCamVideoSaveThread)obj).mProcessIdx;
                            if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                            {
                                logger.Debug(String.Format("【CYCLE】イベント受信。スレッドインデックス：{0}, 工程インデックス：{1} ", mThreadIndex, processIdx));
                                int camUseStatus = mListCameraUsingStatusByProcess[processIdx];
                                DateTime start = mDicCycleDatetimeOnProcess[processIdx];
                                DateTime nextStart = ((CmdWebCamVideoSaveThread)obj).mCycleDateTime;
                                DateTime operationDate = ((CmdWebCamVideoSaveThread)obj).mOperationDateTime;
                                DataRow drWorkShift = ((CmdWebCamVideoSaveThread)obj).mShiftTable.Rows[processIdx];
                                Guid compId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.COMPOSITION_ID);
                                Guid prodId = drWorkShift.Field<Guid>(DBConnect.SQL.ColumnWorkingStatus.PRODUCT_TYPE_ID);
                                String worker = drWorkShift.Field<String>(DBConnect.SQL.ColumnWorkingStatus.WORKER_NAME);


                                if (camUseStatus > 0)
                                {
                                    VideoWriter vw = mDicVideoWriteStreamOnProcess[processIdx];
                                    {
                                        logger.Debug(String.Format("【CYCLE】イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));

                                        DelegateEndMethod(vw, processIdx, start, nextStart, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebCamVideoSaveThread)obj).mShiftTable, ((CmdWebCamVideoSaveThread)obj).mDtCycleDataMst);
                                        mFileDateTime = nextStart;

                                        InitMovieFileStream(nextStart, processIdx);
                                    }
                                }
                                else
                                {
                                    logger.Debug(String.Format("【CYCLE】イベント発行(カメラ不使用設定)。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));
                                    DelegateEndMethod(null, processIdx, start, nextStart, operationDate, camUseStatus, compId, prodId, worker, ((CmdWebCamVideoSaveThread)obj).mShiftTable, ((CmdWebCamVideoSaveThread)obj).mDtCycleDataMst);
                                }
                                mDicCycleDatetimeOnProcess[processIdx] = nextStart;
                            }
                            else
                            {
                                logger.Debug(String.Format("【CYCLE】工程に使用するカメラ設定がありません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                            }
                            break;
                        case WebCamVideoSaveThreadStatus.SEND_IMAGE:
                            MovieSave(((CmdWebCamVideoSaveThread)obj).mFrame);
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
        /// <param name="setCore"></param>
        /// <returns></returns>
        public Boolean SetCpuCore(int setCore)
        {
            try
            {
                if (Environment.ProcessorCount >= 16)
                {
                    logger.Info(String.Format("【SetCpuCore】CPUコア割り当て。割り当てコア番号：{0}", setCore));

                    IntPtr th = GetCurrentThread();
                    SetThreadAffinityMask(th, new UIntPtr((uint)setCore));
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
                vwRet = new VideoWriter(videoName, mFourCc, mFps, new OpenCvSharp.Size(Program.WIDTH, Program.HEIGHT));
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
        /// 取得した1フレーム(bitmap)をファイルストリームへためる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="frame"></param>
        private void MovieSave(Mat mat)
        {
            String videoName = "";
            //using (Mat mat = frame.ToMat())
            //{
            //}

            //if (mThreadIndex == 0)
            //{
            //logger.Debug(String.Format("【Queカウント出力】{0}, スレッド：{1}", this.QueCount, mThreadIndex));
            //}
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
                    }
                }
            }
            mat.Release();
            mat.Dispose();
            System.GC.Collect();

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
                                logger.Debug(String.Format("【DelegateEndMethod】動画ファイル保存完了。イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}, サイクル終了時刻：{3}", mThreadIndex, processIdx, cycleStartTime.ToString("HH:mm.ss.fff"), cycleEndTime.ToString("HH:mm.ss.fff")));
                                CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(cycleStartTime, cycleEndTime);
                                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_MOVIE_SAVE, fileName, errorList, SaveFileType.MOVIE, mThreadIndex, processIdx, cycleTime, mOperationShift, saveStatus, operationDateTime, comp, prod, worker, shiftTable, cycleDataMst));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    logger.Debug(String.Format("【DelegateEndMethod】動画ファイル保存なし。イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}, サイクル終了時刻：{3}", mThreadIndex, processIdx, cycleStartTime.ToString("HH:mm.ss.fff"), cycleEndTime.ToString("HH:mm.ss.fff")));
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

        #endregion

    }

    /// <summary>
    /// Webカメラ動画保存スレッドコマンドクラス
    /// </summary>
    public class CmdWebCamVideoSaveThread
    {
        public WebCamVideoSaveThreadStatus mStatus;
        public Double mFps;
        public DateTime mCycleDateTime;
        public Mat mFrame;
        public List<int> mCamSaveConfig;
        public int mProcessIdx;
        public DataTable mShiftTable;
        public DateTime mOperationDateTime;
        public DataTable mDtCycleDataMst;
        public int mProcessCount;
        public int mShiftNum;

        /// <summary>
        /// デフォルトの送信コマンド
        /// </summary>
        /// <param name="status"></param>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status)
        {
            mStatus = status;
        }

        /// <summary>
        /// INIT時の送信コマンド
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fps"></param>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status, Double fps)
        {
            mStatus = status;
            mFps = fps;
        }
        /// <summary>
        /// STREAM_START時の送信コマンド
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftStartTime"></param>
        /// <param name="camSaveConfig"></param>
        /// <param name="processCount"></param>
        /// <param name="shift"></param>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status, DateTime shiftStartTime, List<int> camSaveConfig, int processCount, int shift)
        {
            mStatus = status;
            mCycleDateTime = shiftStartTime;
            mCamSaveConfig = camSaveConfig;
            mProcessCount = processCount;
            mShiftNum = shift;
        }

        /// <summary> STREAM_CYCLE時のイベントコンストラクタ </summary>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status, DateTime cycleStartTime, DateTime operationDateTime, int processIdx, DataTable shiftTable, DataTable cycleDataMstTable)
        {
            mStatus = status;
            mCycleDateTime = cycleStartTime;
            mOperationDateTime = operationDateTime;
            mProcessIdx = processIdx;
            mShiftTable = shiftTable;
            mDtCycleDataMst = cycleDataMstTable;
        }

        /// <summary> STREAM_END時のイベントコンストラクタ </summary>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status, DateTime cycleEndTime, DateTime operationDateTime, DataTable shiftTable, DataTable cycleDataMstTable)
        {
            mStatus = status;
            mCycleDateTime = cycleEndTime;
            mOperationDateTime = operationDateTime;
            mShiftTable = shiftTable;
            mDtCycleDataMst = cycleDataMstTable;
        }

        /// <summary>
        /// SEND_IMAGE時の送信コマンド
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fps"></param>
        public CmdWebCamVideoSaveThread(WebCamVideoSaveThreadStatus status, Mat frame)
        {
            mStatus = status;
            mFrame = frame;
        }
    }

}
