using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnect;
using DBConnect.SQL;
using TskCommon;
using log4net;
using System.ComponentModel;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum EventManagementThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> DI受信通知 </summary>
        RECV_DI,
        /// <summary> QR読込通知 </summary>
        READ_QR,
        /// <summary> 撮影動画保存通知 </summary>
        RECV_MOVIE_SAVE,
        /// <summary> 録音音声保存通知 </summary>
        RECV_AUDIO_SAVE,
        /// <summary> 現象動画作成完了 </summary>
        RECV_MOVIE_CREATED,
        /// <summary> デバイス死活確認要求 </summary>
        DEVICE_STATUS,
    }
    /// <summary> 保存するファイル種別を定義 </summary>
    public enum SaveFileType
    {
        MOVIE,
        AUDIO,
    }
    /// <summary>
    /// 全スレッドイベント統合管理クラス
    /// </summary>
    public class EventManagerThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const String ANO_CLM_COMPOSITION_NAME = "CompositionName";
        private const String ANO_CLM_PROCESS_NAME = "ProcessName";
        private const String ANO_CLM_PRODUCT_TYPE_NAME = "ProductTypeName";
        private const String ANO_CLM_WORKER_NAME = "WorkerName";

        /// <summary> 動画エンコードスレッド生成数 </summary>
        private const int ENCODE_THREAD_NUM = 1;

        /// <summary> TskWorkerThreadの監視周期 </summary>
        //public const int THREAD_CYCLE_TIME = 10;

        /// <summary> 設定ファイルで定義するカメラの接続最大数 </summary>
        private int mCamNumMax = -1;
        /// <summary> 設定ファイルで定義するQRリーダの接続最大数 </summary>
        private int mQrNumMax = -1;
        /// <summary> シフト更新時刻の時間(hour)部分 </summary>
        private int mShiftUpdateTimeHour = -1;
        /// <summary> シフト更新時刻の分(minute)部分 </summary>
        private int mShiftUpdateTimeMinute = -1;
        /// <summary> 現在の直のシフト </summary>
        private int mOperatingShift = -1;
        /// <summary> シフト情報アップデート済フラグ </summary>
        private Boolean mUpdatedShiftFlag = false;
        /// <summary> 現在の直の稼働状態 </summary>
        private Boolean mIsOperating = false;
        /// <summary> 各スレッドの初期化完了状態 </summary>
        private Boolean mIsInitEnd = false;
        /// <summary> シフトの初期化完了状態 </summary>
        private Boolean mIsInitGetShift = false;

        /// <summary> 前回シフト情報をアップデートした日付 </summary>
        private DateTime mPrevUpdateDay;
        /// <summary> 現在の直の稼働日 </summary>
        private DateTime mOperatingDate;
        /// <summary> 現在の直の開始時刻 </summary>
        private DateTime mOperatingShiftStartTime;
        /// <summary> 現在の直の終了時刻 </summary>
        private DateTime mOperatingShiftEndTime;
        /// <summary> 現在の直の使用是非 </summary>
        private Boolean mOperatingShiftuseFlag;
        /// <summary> サイクル実績の開始時刻 </summary>
        private DateTime mCycleStartDateTime;
        /// <summary> 現在の作業登録の編成ID </summary>
        private Guid mCompositionId;
        /// <summary> 現在の作業登録の品番タイプID </summary>
        //private Guid mProductTypeId;


        /// <summary> QRリーダの接続状態 </summary>
        private List<Boolean> mListQrReaderAlive = new List<Boolean>();
        /// <summary> カメラの接続状態 </summary>
        private List<Boolean> mListCameraAlive = new List<Boolean>();
        /// <summary> カメラの接続インデックス </summary>
        private List<int> mListCameraIndex = new List<int>();

        /// <summary> 計画稼働シフトテーブル接続インスタンス </summary>
        private SqlPlanOperatingShiftTbl mSqlPlanOperatingShiftTbl;
        /// <summary> 稼働登録テーブル接続インスタンス </summary>
        private SqlWorkingStatus mSqlWorkingStatus;
        /// <summary> カメラ機器マスタテーブルの参照 </summary>
        private SqlCameraDeviceMst mSqlCameraDeviceMst;
        /// <summary> QRリーダ機器マスタテーブルの参照 </summary>
        private SqlReaderDeviceMst mSqlReaderDeviceMst;
        /// <summary> サイクル実績テーブルの参照 </summary>
        private SqlCycleResultTbl mSqlCycleResultTbl;
        /// <summary> 標準値品番工程別マスタテーブルの参照 </summary>
        private SqlStandardValProductTypeProcessMst mSqlStandardValProductTypeProcessMst;
        /// <summary> 実績稼働シフトテーブルの参照 </summary>
        private SqlResultOperatingShiftTbl mSqlResultOperatingShiftTbl;
        /// <summary> 稼働シフト品番タイプ毎生産数テーブルの参照 </summary>
        private SqlOperatingShiftProductionQuantityTbl mSqlOperatingShiftProductionQuantityTbl;
        /// <summary> データ収集カメラマスタテーブルの参照 </summary>
        private SqlDataCollectionCameraMst mSqlDataCollectionCameraMst;

        /// <summary> 取得したシフト情報 </summary>
        private DataTable mDtPlanOperationShift;
        /// <summary> 取得した翌稼働日のシフト情報 </summary>
        private DataTable mDtPlanOperationNextDateShift;
        /// <summary> 取得した稼働登録情報 </summary>
        private DataTable mWorkingStatusTbl;
        /// <summary> カメラ機器マスタから取得したデータの管理 </summary>
        private DataTable mDtCameraDeviceMst;
        /// <summary> QRリーダ機器マスタから取得したデータの管理 </summary>
        private DataTable mDtReaderDeviceMst;
        ///// <summary> サイクル実績テーブルから取得したデータの管理 </summary>
        //private DataTable mDtCycleResultTbl;
        /// <summary> 標準値品番工程別マスタから取得したサイクル用データの管理 </summary>
        private DataTable mDtCycleDataMst;
        /// <summary> データ収集カメラマスタテーブルから取得したデータの管理 </summary>
        private DataTable mDtDataCollectionCameraMst;

        private List<ConcatMovieInfo> mListConcatMovieInfo = new List<ConcatMovieInfo>();

        private DioCtrlThread dioCtrlThread;
        //private DataCtrlThread dataSaveThread;
        private DataDeleteCtrl dataDeleteThread;
        private List<WebCamCtrlThread> webCamCtrlThreads = new List<WebCamCtrlThread>();
        private List<MicCtrlThread> micCtrlThreads = new List<MicCtrlThread>();
        private List<QrReaderCtrlThread> qrReaderCtrlThread = new List<QrReaderCtrlThread>();
        private List<DataCtrlThread> dataSaveThreadList = new List<DataCtrlThread>();

        private int mEncodeThreadCnt = 0;

        /// <summary> 初期化処理からの発行イベント </summary>
        public event EventHandler EventFailedInit;

        /// <summary>終了バックグラウンド処理 </summary>
        private BackgroundWorker mEndProcessBackGround;

        #endregion

        #region <Property>

        public Boolean IsOperating
        {
            get { return this.mIsOperating; }
        }
        public DataTable CameraDeviceMst
        {
            get { return this.mDtCameraDeviceMst; }
        }
        public DataTable ReaderDeviceMst
        {
            get { return this.mDtReaderDeviceMst; }
        }
        public List<Boolean> ListCameraAlive
        {
            get { return this.mListCameraAlive; }
        }
        public List<int> ListCameraIndex
        {
            get { return this.mListCameraIndex; }
        }
        public List<Boolean> ListQrReaderAlive
        {
            get { return this.mListQrReaderAlive; }
        }

        public Boolean IsFinishProcess { get; set; }

        #endregion

        #region <Constructor>

        /// <summary>
        /// Constructor
        /// </summary>
        public EventManagerThread(out Boolean isInitevent)
        {
            isInitevent = false;

            mCamNumMax = Program.AppSetting.SystemParam.CameraMaxNum;
            mQrNumMax = Program.AppSetting.SystemParam.QrReaderMaxNum;

            if (mCamNumMax < 1 || mQrNumMax < 1)
            {
                logger.Error("起動スレッド数異常");
                isInitevent = false;
                return;
            }

            mSqlPlanOperatingShiftTbl = new SqlPlanOperatingShiftTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlWorkingStatus = new SqlWorkingStatus(Program.AppSetting.SystemParam.DbConnect);
            mSqlCameraDeviceMst = new SqlCameraDeviceMst(Program.AppSetting.SystemParam.DbConnect);
            mSqlReaderDeviceMst = new SqlReaderDeviceMst(Program.AppSetting.SystemParam.DbConnect);
            mSqlCycleResultTbl = new SqlCycleResultTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlStandardValProductTypeProcessMst = new SqlStandardValProductTypeProcessMst(Program.AppSetting.SystemParam.DbConnect);
            mSqlResultOperatingShiftTbl = new SqlResultOperatingShiftTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlOperatingShiftProductionQuantityTbl = new SqlOperatingShiftProductionQuantityTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlDataCollectionCameraMst = new SqlDataCollectionCameraMst(Program.AppSetting.SystemParam.DbConnect);

            // シフト更新時刻の作成用の時分定義
            String[] splitShiftUpdateTime = Program.AppSetting.SystemParam.UpdateShiftInfo.Split(':');
            if (int.TryParse(splitShiftUpdateTime[0], out mShiftUpdateTimeHour) && int.TryParse(splitShiftUpdateTime[1], out mShiftUpdateTimeMinute))
            {
                if (mShiftUpdateTimeHour < 0 || 23 < mShiftUpdateTimeHour)
                {
                    logger.Error(String.Format("シフト更新時刻設定(時間部分)異常。読込データ：{0}", mShiftUpdateTimeHour));
                    isInitevent = false;
                    return;
                }
                if (mShiftUpdateTimeMinute < 0 || 59 < mShiftUpdateTimeMinute)
                {
                    logger.Error(String.Format("シフト更新時刻設定(分部分)異常。読込データ：{0}", mShiftUpdateTimeMinute));
                    isInitevent = false;
                    return;
                }
            }
            else
            {
                logger.Error(String.Format("シフト更新時刻設定異常。読込データ：{0}", Program.AppSetting.SystemParam.UpdateShiftInfo));
                isInitevent = false;
                return;
            }

            // DIOスレッドの展開
            Boolean initRet = false;
            dioCtrlThread = new DioCtrlThread(Program.AppSetting.SystemParam.TimeInterval, out initRet);
            if (!initRet)
            {
                logger.Error("DIO管理クラス初期化異常");
                isInitevent = false;
                return;
            }

            // データ保存スレッドの展開
            //dataSaveThread = new DataCtrlThread(Program.AppSetting.SystemParam.TimeInterval, 0);
            //dataSaveThread.SetCpuCore(5);
            // データ削除スレッドの展開
            dataDeleteThread = new DataDeleteCtrl(Program.AppSetting.SystemParam.TimeInterval);

            // カメラ数分スレッドの展開
            mDtCameraDeviceMst = mSqlCameraDeviceMst.Select();
            for (int i = 0; i < mCamNumMax; i++)
            {
                mListCameraAlive.Add(true);
                mListCameraIndex.Add(-1);
                webCamCtrlThreads.Add(new WebCamCtrlThread(Program.AppSetting.SystemParam.TimeInterval, i));

                //int core = 4 + (i / 2);

                // 2022.11.10 コア割り当て停止
                //int core = 5 + i;
                //webCamCtrlThreads[i].SetCpuCore(core);


                micCtrlThreads.Add(new MicCtrlThread(Program.AppSetting.SystemParam.TimeInterval, i));
            }
            for (int i = 0; i < ENCODE_THREAD_NUM; i++)
            {
                dataSaveThreadList.Add(new DataCtrlThread(Program.AppSetting.SystemParam.TimeInterval, i));
                // 2022.11.10 コア割り当て停止
                //dataSaveThreadList[i].SetCpuCore(4);
            }
            mEncodeThreadCnt = 0;

            // QRリーダ数分スレッドの展開
            mDtReaderDeviceMst = mSqlReaderDeviceMst.Select();
            for (int i = 0; i < mQrNumMax; i++)
            {
                mListQrReaderAlive.Add(false);
                qrReaderCtrlThread.Add(new QrReaderCtrlThread(Program.AppSetting.SystemParam.TimeInterval, i));
            }

            // シフト情報の初期取得
            if (!InitShift())
            {
                logger.Error("シフト情報の初期取得異常");
                isInitevent = false;
                return;
            }

            // 起動時点での稼働登録のテーブルを全件取得する
            mWorkingStatusTbl = mSqlWorkingStatus.Select();
            if (mWorkingStatusTbl != null && mWorkingStatusTbl.Rows.Count > 0)
            {
                mCompositionId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                //mProductTypeId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                var productTypeIdCheck = mWorkingStatusTbl.Rows[0][ColumnWorkingStatus.PRODUCT_TYPE_ID];
                if (productTypeIdCheck != DBNull.Value)
                {
                    Guid productTypeId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                    mDtCycleDataMst = mSqlStandardValProductTypeProcessMst.SelectCycleDataWithDataCollectionTrigerMst(mCompositionId, productTypeId);
                }
                else
                {
                    // 警告表示
                    logger.Warn("作業者登録に品番タイプIDなし。作業登録(Working_Status)テーブルをクリアし、QR再読み込み要求");
                    System.Windows.Forms.MessageBox.Show("作業登録に品番タイプIDがありません。\n作業登録をクリアします。再度QR読込してください。", "作業登録無し", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    mSqlWorkingStatus.Delete();
                    mWorkingStatusTbl.Clear();
                }

            }
            IsFinishProcess = false;
            isInitevent = true;
        }

        #endregion

        #region <Method>

        /// <summary>
        /// 定周期処理
        /// </summary>
        protected override void Proc()
        {
            if (mIsInitEnd)
            {
                if (mIsInitGetShift)
                {
                    // 直の開始・終了の状態を更新する
                    ShiftStartEnd();

                    // シフトデータ更新
                    UpdateShift();

                    // 当直の稼働計画がDBに存在しているか(マスタから消されていないか)をチェック
                    if (mIsOperating)
                    {
                        CheckShiftWorking();
                    }
                }
                else
                {
                    // 本日の実績を再度取得しにかかる。
                    GetShiftRetryBetweenShift();
                }
            }

            base.Proc();
        }

        /// <summary>
        /// イベント受信処理
        /// </summary>
        /// <param name="obj"></param>
        protected override void QueueEvent(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }
                else if (obj is CmdEventManager)
                {
                    switch (((CmdEventManager)obj).mStatus)
                    {
                        case EventManagementThreadStatus.INIT:
                            logger.Debug(String.Format("【INIT】管理下スレッドの開始処理"));
                            mIsInitEnd = StartThreads();
                            if (!mIsInitEnd)
                            {
                                logger.Error("【INIT】管理下スレッドの開始処理に失敗。終了処理発行");
                                EventFailedInit(this, EventArgs.Empty);
                            }
                            break;
                        case EventManagementThreadStatus.TERM:
                            logger.Debug(String.Format("【TERM】管理下スレッドの停止処理"));
                            StopThreads();
                            break;
                        case EventManagementThreadStatus.RECV_DI:

                            if (mIsOperating)
                            {
                                logger.Debug(String.Format("【RECV_DI】センサー変化イベント受信。各カメラ・マイクへのイベント発行。発行元工程インデックス：{0}, 発行時刻：{1}", ((CmdEventManager)obj).mProcessIndex, ((CmdEventManager)obj).mDateTime.ToString("HH:mm.ss.fff")));

                                // 特定のカメラ、マイクに対して撮像・録音要求
                                // ※呼び元ではどのカメラ、マイクがどの工程で使用されているか不明のため、全カメラおよびマイクにイベントを送ってカメラスレッド側で判断させる
                                for (int i = 0; i < mCamNumMax; i++)
                                {
                                    webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.CYCLE, ((CmdEventManager)obj).mDateTime, mOperatingDate, ((CmdEventManager)obj).mProcessIndex, mWorkingStatusTbl, mDtCycleDataMst));
                                    micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.CYCLE, ((CmdEventManager)obj).mDateTime, ((CmdEventManager)obj).mProcessIndex));
                                }
                            }
                            else
                            {
                                logger.Debug(String.Format("【RECV_DI】センサー変化イベント受信。直時間外のためカメラ・マイクへのイベント発行無し。発行元工程インデックス：{0}, 発行時刻：{1}", ((CmdEventManager)obj).mProcessIndex, ((CmdEventManager)obj).mDateTime.ToString("HH:mm.ss.fff")));
                            }

                            break;
                        case EventManagementThreadStatus.READ_QR:
                            int qrDataNo = ((CmdEventManager)obj).mQrParam.mDataNo;
                            int qrProcIdx = ((CmdEventManager)obj).mQrParam.mProcessIndex;

                            DataTable copyShiftTable = null;
                            DataTable copyCycmeMstTable = null;
                            // QR読込結果によって作業登録を実施
                            logger.Debug(String.Format("【READ_QR】QR読み取りイベント受信。データNo：{0}, 工程インデックス：{1}, アノテーション情報：{2}", qrDataNo, qrProcIdx, ((CmdEventManager)obj).mQrParam.mAnnotationInfo));
                            //if (mIsOperating)
                            {
                                if (mWorkingStatusTbl != null && mWorkingStatusTbl.Rows.Count > 0)
                                {
                                    copyShiftTable = mWorkingStatusTbl.Copy();
                                }
                                if (mDtCycleDataMst != null && mDtCycleDataMst.Rows.Count > 0)
                                {
                                    copyCycmeMstTable = mDtCycleDataMst.Copy();
                                }
                                if (GetQrData(((CmdEventManager)obj).mQrParam.mDataNo, ((CmdEventManager)obj).mQrParam.mProcessIndex, ((CmdEventManager)obj).mQrParam.mAnnotationInfo))
                                {
                                    if (mIsOperating)
                                    {
                                        if (mWorkingStatusTbl != null && mDtCycleDataMst != null)
                                        {
                                            logger.Debug(String.Format("【READ_QR】QR読み取りイベント受信。稼働中のサイクルおよびカメラの停止"));
                                            if (qrDataNo == 1 || (qrDataNo == 2 && qrProcIdx == 0))
                                            {
                                                // 一括変更の場合、現在のサイクルをいったん切る仕掛けが必要
                                                // 既存のカメラ、マイクスレッドを止めて再開させる
                                                DateTime shiftEndTimeByAllProductQr = DateTime.Now;
                                                // 動いている各カメラを停止させる
                                                for (int i = 0; i < mCamNumMax; i++)
                                                {
                                                    webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_END, shiftEndTimeByAllProductQr, mOperatingDate, copyShiftTable, copyCycmeMstTable));
                                                    micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_END, shiftEndTimeByAllProductQr));
                                                }
                                                mCycleStartDateTime = DateTime.Now;
                                                ShiftStartAndRecordOn();
                                            }
                                        }
                                        else
                                        {
                                            logger.Debug(String.Format("【READ_QR】稼働登録情報または標準値マスタ情報がNULL。可動情報登録有：{0}, 標準値マスタ情報有：{1}", mWorkingStatusTbl != null, mDtCycleDataMst != null));
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("【READ_QR】直時間外QR読み取りイベント。"));
                                    }
                                }
                                else
                                {
                                    logger.Warn("【READ_QR】QR読込・作業登録データ更新処理に失敗");
                                }
                            }
                            //else
                            //{
                            //    logger.Debug(String.Format("【READ_QR】計画稼働時間外のQR読み込みのため、ブザー全点発報"));
                            //    List<int> listBuzzerBit = new List<int>();
                            //    // 【計画稼働時間外のQR読み込み】のため、ブザー全点発報
                            //    GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            //    dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            //}
                            break;
                        case EventManagementThreadStatus.RECV_MOVIE_SAVE:
                        case EventManagementThreadStatus.RECV_AUDIO_SAVE:
                            // 録画データ保存完了通知
                            // 録音データ保存完了通知
                            int procIdx = ((CmdEventManager)obj).mProcessIndex;
                            int camIdx = ((CmdEventManager)obj).mCameraIndex;
                            DateTime operationDate = ((CmdEventManager)obj).mOperationDate;
                            //DateTime cycleDt = ((CmdEventManager)obj).mDateTime;
                            String fileName = ((CmdEventManager)obj).mSaveFileName;

                            // START,ENDをひっぱりだして実績の登録を仕掛ける
                            DateTime start = ((CmdEventManager)obj).mCycleStartEnd.cycleStart;
                            DateTime end = ((CmdEventManager)obj).mCycleStartEnd.cycleEnd;

                            Boolean needToSave = false;

                            int status = ((CmdEventManager)obj).mSaveStatus;
                            int errorFlg = -10; ;
                            if (((CmdEventManager)obj).mFileType == SaveFileType.MOVIE)
                            {
                                Guid comp = ((CmdEventManager)obj).mCompositionId;
                                Guid prod = ((CmdEventManager)obj).mProductTypeId;
                                String worker = ((CmdEventManager)obj).mWorkerName;

                                int shift = ((CmdEventManager)obj).mOperationShift;
                                // サイクルの実績を作成する

                                DataTable tableSelectCount = mSqlCycleResultTbl.SelectProductionByDate(start, end, procIdx);
                                int count = tableSelectCount != null ? tableSelectCount.Rows.Count : 0;
                                //int count = tableSelectCount.Rows[0].Field<int>(SqlCycleResultTbl.SELECT_COUNT_COLUMN_NAME);
                                if (count < 1)
                                {
                                    UpdateCycleTime(((CmdEventManager)obj).mProcessIndex, start, end, operationDate, shift, comp, prod, worker, ((CmdEventManager)obj).mCycleMstTable, out errorFlg);
                                }
                                else
                                {
                                    int? tempFlag = tableSelectCount.Rows[0].Field<int?>(ColumnCycleResult.ERROR_FLAG);
                                    if (tempFlag == null)
                                    {
                                        errorFlg = 0;
                                    }
                                    else
                                    {
                                        errorFlg = tableSelectCount.Rows[0].Field<int>(ColumnCycleResult.ERROR_FLAG);
                                    }
                                }
                            }
                            else
                            {
                                DataTable tableSelectCount = mSqlCycleResultTbl.SelectProductionByDate(start, end, procIdx);
                                if (tableSelectCount != null && tableSelectCount.Rows.Count > 0)
                                {
                                    int? tempFlag = tableSelectCount.Rows[0].Field<int?>(ColumnCycleResult.ERROR_FLAG);
                                    if (tempFlag == null)
                                    {
                                        errorFlg = 0;
                                    }
                                    else
                                    {
                                        errorFlg = tableSelectCount.Rows[0].Field<int>(ColumnCycleResult.ERROR_FLAG);
                                    }
                                }
                            }

                            // 当該工程、当該カメラの設定が1(全保存)、または2(異常のみ保存)で異常フラグONのとき、要保存フラグをtrueにする 
                            if (status == 1 || (status == 2 && errorFlg > 0))
                            {
                                needToSave = true;
                                // サイクル動画生成情報リストから、対象のカメラ、工程インデックス、サイクル開始時刻の情報を持つ行を取得する
                                ConcatMovieInfo temp = mListConcatMovieInfo.Find(x => (x.mProcessIdx == procIdx) && (x.mCameraIdx == camIdx) && (x.mCycleTime == start));
                                if (temp != null)
                                {
                                    // 行有の場合、通知されたファイル種別に従って内容を情報リストに入れてイベントを発行する
                                    switch (((CmdEventManager)obj).mFileType)
                                    {
                                        case SaveFileType.MOVIE:
                                            temp.mVideoFile = fileName;
                                            temp.mVideoSavedFlg = true;
                                            temp.mAviVideoFileList = ((CmdEventManager)obj).mListFilePath;
                                            temp.mShiftTable = ((CmdEventManager)obj).mShiftTable;
                                            break;
                                        case SaveFileType.AUDIO:
                                            temp.mAudioFile = fileName;
                                            temp.mAudioSavedFlg = true;
                                            break;
                                        default:
                                            // ERROR
                                            break;
                                    }
                                    //if (mEncodeThreadCnt == 0)
                                    //{
                                    //    mEncodeThreadCnt = 1;
                                    //}
                                    //else if (mEncodeThreadCnt == 1)
                                    //{
                                    //    mEncodeThreadCnt = 2;
                                    //}
                                    //else
                                    //{
                                    //    mEncodeThreadCnt = 0;
                                    //}

                                    logger.Debug(String.Format("【動画保存発行】工程{0}, サイクル開始時刻：{1}、サイクル終了時刻：{2}、カメラIdx：{3}、保存要求：{4}, status: {5},error: {6}", procIdx + 1, start, end, camIdx + 1, needToSave, status, errorFlg));
                                    logger.Debug(String.Format("【動画保存発行】工程{0}, 対象エンコード処理スレッド：{1}", procIdx + 1, mEncodeThreadCnt));
                                    dataSaveThreadList[mEncodeThreadCnt].AddEvent(new CmdDataSave(DataSaveThreadStatus.CONCAT, start, end, temp.mVideoFile, temp.mAviVideoFileList, temp.mAudioFile, camIdx, procIdx, needToSave, temp.mShiftTable));
                                    //dataSaveThread.AddEvent(new CmdDataSave(DataSaveThreadStatus.CONCAT, start, end, temp.mVideoFile, temp.mAviVideoFileList, temp.mAudioFile, camIdx, procIdx, needToSave, temp.mShiftTable));
                                    mListConcatMovieInfo.Remove(temp);
                                }
                                else
                                {
                                    // 行無しの場合、新規に行を起こして、同カメラ・工程の録音(録画)データがそろうのを待つ
                                    switch (((CmdEventManager)obj).mFileType)
                                    {
                                        case SaveFileType.MOVIE:
                                            mListConcatMovieInfo.Add(new ConcatMovieInfo(procIdx, camIdx, start, fileName, ((CmdEventManager)obj).mListFilePath, "", true, false, ((CmdEventManager)obj).mShiftTable));
                                            break;
                                        case SaveFileType.AUDIO:
                                            mListConcatMovieInfo.Add(new ConcatMovieInfo(procIdx, camIdx, start, "", ((CmdEventManager)obj).mListFilePath, fileName, false, true, null));
                                            break;
                                        default:
                                            // ERROR
                                            break;
                                    }
                                    logger.Debug(String.Format("【動画保存新規行生成】工程{0}, サイクル開始時刻：{1}、サイクル終了時刻：{2}、カメラIdx：{3}、保存種別：{4}", procIdx + 1, start, end, camIdx + 1, ((CmdEventManager)obj).mFileType));
                                }
                            }

                            break;
                        case EventManagementThreadStatus.RECV_MOVIE_CREATED:
                            // 現象動画保存完了したことの通知
                            int procIdxRecv = ((CmdEventManager)obj).mProcessIndex;
                            DateTime cycleTime = ((CmdEventManager)obj).mDateTime;
                            String cycleFileName = ((CmdEventManager)obj).mSaveFileName;
                            DataTable shiftTable = ((CmdEventManager)obj).mShiftTable;
                            logger.Debug(String.Format("【RECV_MOVIE_CREATED】動画保存通知受信。工程{0}, サイクル時刻：{1}、ファイル名：{2}", procIdxRecv, cycleTime.ToString("HH:mm.ss.fff"), cycleFileName));
                            UpdateCycleTimeWithFileName(procIdxRecv, cycleTime, cycleFileName, shiftTable);
                            break;
                        case EventManagementThreadStatus.DEVICE_STATUS:
                            // カメラ、QRリーダの死活監視
                            for (int i = 0; i < mQrNumMax; i++)
                            {
                                // QRリーダの死活応答
                                mListQrReaderAlive[i] = qrReaderCtrlThread[i].IsAliveQrReader;
                            }
                            for (int i = 0; i < mCamNumMax; i++)
                            {
                                //カメラの死活応答
                                mListCameraAlive[i] = webCamCtrlThreads[i].IsAliveCamera;
                                mListCameraIndex[i] = webCamCtrlThreads[i].IsAliveCamera ? i : -1;
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }

        }

        /// <summary>
        /// 管理下にある各デバイス管理スレッドの全起動
        /// </summary>
        /// <returns></returns>
        private Boolean StartThreads()
        {
            try
            {
                dioCtrlThread.Start();
                dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.INIT));

                //dataSaveThread.Start();
                dataDeleteThread.Start();
                // カメラ、マイクスレッドの開始
                for (int i = 0; i < mCamNumMax; i++)
                {
                    if (mDtCameraDeviceMst.Rows.Count <= i)
                    {
                        break;
                    }
                    webCamCtrlThreads[i].Start();
                    micCtrlThreads[i].Start();

                }

                for (int i = 0; i < ENCODE_THREAD_NUM; i++)
                {
                    dataSaveThreadList[i].Start();
                }

                // QRリーダスレッドの開始
                for (int i = 0; i < mQrNumMax; i++)
                {
                    if (mDtReaderDeviceMst.Rows.Count <= i)
                    {
                        break;
                    }
                    qrReaderCtrlThread[i].Start();
                    qrReaderCtrlThread[i].AddEvent(new CmdQrReaderCtrl(QrReaderCtrlThreadStatus.INIT, mDtReaderDeviceMst.Rows[i]));
                }

                return true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                return false; ;
            }
        }

        /// <summary>
        /// 管理下にある各デバイス管理スレッドの全終了
        /// </summary>
        private void StopThreads()
        {
            try
            {
                mEndProcessBackGround = new BackgroundWorker();
                mEndProcessBackGround.WorkerReportsProgress = true;
                mEndProcessBackGround.DoWork += WorkeDoWorkEndProcess;
                logger.Debug(String.Format("【StopThreads】バックグラウンド処理で停止処理の開始"));
                mEndProcessBackGround.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【StopThreads】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        private void WorkeDoWorkEndProcess(object sender, DoWorkEventArgs e)
        {
            try
            {
                logger.Debug(String.Format("【WorkeDoWork】各スレッド終了コマンド発行"));
                dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.TERM));
                dioCtrlThread.Stop();
                foreach (QrReaderCtrlThread thread in qrReaderCtrlThread)
                {
                    if (thread.IsAliveQrReader)
                    {
                        thread.AddEvent(new CmdQrReaderCtrl(QrReaderCtrlThreadStatus.TERM));
                        thread.Stop();
                    }
                }

                foreach (WebCamCtrlThread thread in webCamCtrlThreads)
                {
                    if (mIsOperating)
                    {
                        thread.AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_END, DateTime.Now, mOperatingDate, mWorkingStatusTbl, mDtCycleDataMst));
                    }
                    thread.AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.TERM));
                    thread.Stop();
                }
                foreach (MicCtrlThread thread in micCtrlThreads)
                {
                    if (mIsOperating)
                    {
                        thread.AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_END, DateTime.Now));
                    }
                    thread.Stop();
                }

                logger.Debug(String.Format("【WorkeDoWorkEndProcess】動画生成処理待機A"));
                System.Threading.Thread.Sleep(1000);
                logger.Debug(String.Format("【WorkeDoWorkEndProcess】動画生成処理待機B"));
                foreach (DataCtrlThread thread in dataSaveThreadList)
                {
                    thread.Stop();
                }
                //dataSaveThread.Stop();
                dataDeleteThread.Stop();
                logger.Debug(String.Format("【WorkeDoWorkEndProcess】動画生成処理終了"));

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【WorkeDoWorkEndProcess】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                logger.Debug(String.Format("【WorkeDoWorkEndProcess】各スレッド処理終了"));
                IsFinishProcess = true;
            }
        }

        /// <summary>
        /// 直の開始・終了を取得し更新する
        /// </summary>
        /// <returns></returns>
        private Boolean ShiftStartEnd()
        {
            Boolean ret = false;
            try
            {
                // 直作業中
                if (mIsOperating)
                {

                    if (DateTime.Now > mOperatingShiftEndTime)
                    {
                        logger.Info(String.Format("【ShiftStartEnd】直終了。終了時刻：{0}, 稼働シフト: {1}", mOperatingShiftEndTime.ToString("yyyy/MM/dd HH:mm:ss.fff"), mOperatingShift));
                        // 直作業中 → 直終了への処理
                        mIsOperating = false;
                        Boolean resetFlg = false;
                        dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.SHIFT_END));

                        // 動いている各カメラを停止させる
                        for (int i = 0; i < mCamNumMax; i++)
                        {
                            webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime, mOperatingDate, mWorkingStatusTbl, mDtCycleDataMst));
                            micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime));
                        }

                        // 次直の情報を設定
                        resetFlg = GetBeforeShift();

                        if (!resetFlg)
                        {
                            // 当稼働日の次の直情報が取れていない場合
                            if (mUpdatedShiftFlag && (mDtPlanOperationNextDateShift != null))
                            {
                                // すでに定時更新で翌稼働日の直を取得している場合にはそれを取得する
                                mDtPlanOperationShift = mDtPlanOperationNextDateShift;
                                mDtPlanOperationNextDateShift = null;
                            }
                            else
                            {
                                // この日の直がなくなったため、翌稼働日の直を取得する。
                                logger.Info("【ShiftStartEnd】本稼働日の直なし。翌稼働日の直を取得する");
                                mDtPlanOperationShift = mSqlPlanOperatingShiftTbl.Select(mOperatingDate.AddDays(1));

                            }
                            // 翌日稼働日の条件で再取得を実施
                            if (!GetBeforeShift())
                            {
                                // 次の日が休み等で無稼働の場合、直停止中にシフトを取り直せるようにシフト番号を初期化する
                                logger.Info("【ShiftStartEnd】翌稼働日の直なし。再取得状態に変化");
                                mOperatingShift = -1;
                            }
                        }
                    }
                }
                // 直停止中
                else
                {
                    // シフト情報がない
                    if (mOperatingShift < 0 && mDtPlanOperationNextDateShift != null)
                    {
                        //GetShiftRetryBetweenShift();

                        // 開始・終了時刻等を取り直す
                        mDtPlanOperationShift = mDtPlanOperationNextDateShift;
                        mDtPlanOperationNextDateShift = null;
                        if (mDtPlanOperationShift != null)
                        {
                            GetBeforeShift();
                        }
                    }

                    if (mOperatingShift > 0 && DateTime.Now > mOperatingShiftStartTime)
                    {
                        // 直停止中 → 直開始への処理
                        mIsOperating = true;
                        logger.Info(String.Format("【ShiftStartEnd】直開始。開始時刻：{0}, 稼働シフト: {1}", mOperatingShiftStartTime.ToString("yyyy/MM/dd HH:mm:ss.fff"), mOperatingShift));

                        // 最初のサイクルデータを登録させる
                        dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.SHIFT_START, new CmdDioCtrlShiftTime(mOperatingShiftStartTime, mOperatingShiftEndTime)));
                        mCycleStartDateTime = mOperatingShiftStartTime;

                        // 現在の作業登録データの作業者名を全て未登録で置き換える
                        mSqlWorkingStatus.UpdateWokerNameReset();

                        // 本直の実績稼働シフトテーブルを作成する
                        CreateResultOperatingShiftData();

                        // 直開始時の録画・録音要求を発行する
                        ShiftStartAndRecordOn();
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 稼働シフトテーブルの初期取得をし、
        /// 最初の直開始時刻と直番号を控える
        /// </summary>
        /// <returns></returns>
        private Boolean InitShift()
        {
            Boolean ret = false;
            try
            {

                // 計画稼働シフトテーブルの参照
                mDtPlanOperationShift = mSqlPlanOperatingShiftTbl.Select(DateTime.Today);
                mUpdatedShiftFlag = true;
                mPrevUpdateDay = DateTime.Today;

                // 取得した計画稼働シフトテーブルから直近で動かすべきシフトを取得する
                GetBetweenShift();

                if (mOperatingShift < 0)
                {
                    // 直間のため、取得条件を変更して最初の時間を取得する
                    GetBeforeShift();
                }

                if (mOperatingShift < 0)
                {
                    // この状態でも取得できなければ"本日"はシフトがないため、定周期で取得にかかる
                    mIsInitGetShift = false;
                    logger.Debug(String.Format("【InitShift】本稼働日の稼働シフト無し。定周期取得の開始"));
                }
                else
                {
                    mIsInitGetShift = true;
                }


                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 稼働シフトテーブルを再取得し、最初の直開始時刻と直番号を取得しなおす
        /// DN-明治DR #11より。直時間内の計画削除対応
        /// </summary>
        /// <returns></returns>
        private Boolean GetShiftRetryBetweenShift()
        {
            Boolean ret = false;
            try
            {
                // 計画稼働シフトテーブルの参照
                mDtPlanOperationShift = mSqlPlanOperatingShiftTbl.Select(DateTime.Today);
                mUpdatedShiftFlag = true;
                mPrevUpdateDay = DateTime.Today;

                // 取得した計画稼働シフトテーブルから直近で動かすべきシフトを取得する
                GetBetweenShift();
                if (!mOperatingShiftuseFlag)
                {
                    // 計画は存在するが使用状態false
                    mOperatingShift = -1;
                    mIsInitGetShift = false;
                }
                if (mOperatingShift < 0)
                {
                    // この状態でも取得できなければ、再度定周期で取得にかかる
                    mIsInitGetShift = false;
                }
                else
                {
                    mIsInitGetShift = true;
                    logger.Debug(String.Format("【GetShiftRetryBetweenShift】本稼働日の定周期取得完了"));
                }

                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 現在すでに取得した稼働日、稼働シフトの計画がDB上にあるかを検索する
        /// DN-明治DR #11より。直時間内の計画削除対応
        /// </summary>
        /// <returns></returns>
        private Boolean CheckShiftWorking()
        {
            Boolean ret = false;
            try
            {
                DataTable dt = mSqlPlanOperatingShiftTbl.Select(mOperatingDate, mOperatingShift);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //現在稼働している直の計画はDB内にあるが、シフト有効チェックを消されたらその時点で停止をかける
                    mOperatingShiftuseFlag = dt.Rows[0].Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG);
                    if (!mOperatingShiftuseFlag)
                    {
                        logger.Warn(String.Format("【CheckShiftWorking】稼働計画シフト有効チェックオフを検知のため、サイクル即時停止発行。稼働日：{0}, 稼働シフト：{1}", mOperatingDate, mOperatingShift));
                        mIsInitGetShift = false;
                        mIsOperating = false;
                        dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.SHIFT_END));

                        mOperatingShiftEndTime = DateTime.Now;
                        // 動いている各カメラを停止させる
                        for (int i = 0; i < mCamNumMax; i++)
                        {
                            webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime, mOperatingDate, mWorkingStatusTbl, mDtCycleDataMst));
                            micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime));
                        }
                        mOperatingShift = -1;
                    }
                }
                else
                {
                    logger.Warn(String.Format("【CheckShiftWorking】稼働計画削除を検知のため、サイクル即時停止発行。稼働日：{0}, 稼働シフト：{1}", mOperatingDate, mOperatingShift));
                    // 計画がなくなったため、即時停止を発行する
                    mIsInitGetShift = false;
                    mIsOperating = false;
                    dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.SHIFT_END));

                    mOperatingShiftEndTime = DateTime.Now;
                    // 動いている各カメラを停止させる
                    for (int i = 0; i < mCamNumMax; i++)
                    {
                        webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime, mOperatingDate, mWorkingStatusTbl, mDtCycleDataMst));
                        micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_END, mOperatingShiftEndTime));
                    }
                    mOperatingShift = -1;
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
        /// 計画稼働シフトの更新
        /// 夜間の自動直更新を想定し、「処理が動いた日」のシフトを「次の稼働日のシフト」として保持する
        /// </summary>
        /// <returns></returns>
        private Boolean UpdateShift()
        {
            Boolean ret = false;
            try
            {
                DateTime shiftUpdateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, mShiftUpdateTimeHour, mShiftUpdateTimeMinute, 0);

                // 当日のアップデート済フラグが更新されていなくて、シフト更新時刻を超えた場合
                if (!mUpdatedShiftFlag && DateTime.Now > shiftUpdateTime)
                {
                    logger.Info(String.Format("【UpdateShift】計画稼働シフトの定時刻取得開始"));
                    // 計画稼働シフトテーブルを参照し、本日の稼働シフトを取得する(次稼働日の分として保持)
                    mDtPlanOperationNextDateShift = mSqlPlanOperatingShiftTbl.Select(DateTime.Today);
                    mUpdatedShiftFlag = true;
                    mPrevUpdateDay = DateTime.Today;
                }
                // 現在の日付が前回更新日付を超えていれば再更新する
                if (mUpdatedShiftFlag && DateTime.Today > mPrevUpdateDay.Date)
                {
                    logger.Info(String.Format("【UpdateShift】計画稼働シフトの定時刻更新日超え"));
                    mUpdatedShiftFlag = false;
                }

                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 稼働計画の開始～終了間の時刻でシフトを取得するときの処理
        /// </summary>
        private Boolean GetBetweenShift()
        {
            try
            {
                foreach (DataRow row in mDtPlanOperationShift.Rows)
                {
                    DateTime startDateTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME);
                    DateTime endDateTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME);
                    // 計画開始時刻 < 現在時刻 < 計画終了時刻 を条件にシフトを取得
                    if (startDateTime < DateTime.Now && DateTime.Now < endDateTime)
                    {
                        mOperatingDate = row.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE);
                        mOperatingShift = row.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT);
                        mOperatingShiftStartTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME);
                        mOperatingShiftEndTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME);
                        mOperatingShiftuseFlag = row.Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG);

                        logger.Info(String.Format("【GetBetweenShift】計画稼働シフトの取得。取得稼働日：{0}, 取得稼働シフト：{1}, 取得稼働シフト開始時刻：{2},取得稼働シフト終了時刻：{3}",
                            mOperatingDate.ToString("yyyy/MM/dd"), mOperatingShift, mOperatingShiftStartTime.ToString("HH:mm:ss"), mOperatingShiftEndTime.ToString("HH:mm:ss")));
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 稼働計画の開始前時刻でシフトを取得するときの処理
        /// </summary>
        private Boolean GetBeforeShift()
        {
            try
            {
                // 直間の非稼働時間のため、取得条件を変更して最初の時間を取得する
                foreach (DataRow row in mDtPlanOperationShift.Rows)
                {
                    DateTime startDateTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME);
                    // 計画開始時刻 > 現在時刻 を条件にシフトを取得
                    if (startDateTime > DateTime.Now)
                    {
                        mOperatingDate = row.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE);
                        mOperatingShift = row.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT);
                        mOperatingShiftStartTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME);
                        mOperatingShiftEndTime = row.Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME);

                        logger.Info(String.Format("【GetBeforeShift】計画稼働シフトの取得。取得稼働日：{0}, 取得稼働シフト：{1}, 取得稼働シフト開始時刻：{2}, 取得稼働シフト開始時刻：{3}",
                            mOperatingDate.ToString("yyyy/MM/dd"), mOperatingShift, mOperatingShiftStartTime.ToString("HH:mm:ss"), mOperatingShiftEndTime.ToString("HH:mm:ss")));
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 各直の実績稼働シフトテーブルを作成する
        /// 直の稼働開始時に一度だけ実行する
        /// </summary>
        /// <returns></returns>
        private Boolean CreateResultOperatingShiftData()
        {
            Boolean ret = false;
            try
            {
                logger.Info(String.Format("【CreateResultOperatingShiftData】実績稼働シフトテーブルを作成。稼働日：{0}, 稼働シフト：{1}", mOperatingDate.ToString("yyyy/MM/dd"), mOperatingShift));

                DataTable insertData = new DataTable(ColumnResultOperatingShiftTbl.TABLE_NAME);
                insertData.Rows.Clear();
                insertData.Rows.Add(insertData.NewRow());
                // 稼働日
                insertData.Columns.Add(ColumnResultOperatingShiftTbl.OPERATION_DATE, typeof(DateTime));
                insertData.Rows[0][ColumnResultOperatingShiftTbl.OPERATION_DATE] = mOperatingDate;
                // 稼働シフト
                insertData.Columns.Add(ColumnResultOperatingShiftTbl.OPERATION_SHIFT, typeof(int));
                insertData.Rows[0][ColumnResultOperatingShiftTbl.OPERATION_SHIFT] = mOperatingShift;
                // 開始時刻
                insertData.Columns.Add(ColumnResultOperatingShiftTbl.START_TIME, typeof(DateTime));
                insertData.Rows[0][ColumnResultOperatingShiftTbl.START_TIME] = mOperatingShiftStartTime;
                // 終了時刻
                insertData.Columns.Add(ColumnResultOperatingShiftTbl.END_TIME, typeof(DateTime));
                insertData.Rows[0][ColumnResultOperatingShiftTbl.END_TIME] = mOperatingShiftEndTime;

                // INSERTの発行
                ret = mSqlResultOperatingShiftTbl.Insert(insertData);
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 読み込んだQR情報を基にアノテーション情報をDB検索してテーブルに保管する
        /// </summary>
        /// <param name="dataNo">読み取りデータ識別子</param>
        /// <param name="processIndex">工程Index</param>
        /// <param name="annotationInfo">アノテーション情報</param>
        /// <returns></returns>
        private Boolean GetQrData(int dataNo, int processIndex, String annotationInfo)
        {
            Boolean ret = false;
            List<int> listBuzzerBit = new List<int>();
            try
            {
                switch (dataNo)
                {
                    case 1:
                        Guid compositionId = new Guid();
                        Guid productTypeId = new Guid();
                        int processIdx = -1;
                        Boolean isExistSensor = false;
                        // 編成情報

                        // 編成マスタテーブルからアノテーション情報で渡される編成Noの編成を引き出してくる
                        // 既存のデータを削除し、工程Index分のデータを「作業者登録ステータス」テーブルにINSERT
                        SqlCompositionProcessMst sqlCompositionProcessMst = new SqlCompositionProcessMst(Program.AppSetting.SystemParam.DbConnect);
                        DataTable tempTableComposition = sqlCompositionProcessMst.Select(annotationInfo);
                        if (tempTableComposition == null || tempTableComposition.Rows.Count < 1)
                        {
                            // DB未登録の編成Noで検索されたため、DIOスレッドへ発報要求。
                            logger.Warn(String.Format("【GetQrData】DB未登録の編成No読込。全ブザーを発報します。検索編成No：{0}", annotationInfo));
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }

                        // 前回のデータが存在しない場合、取得した編成をそのまま反映させる(品番タイプnull、作業者【未登録】とする) 
                        if (mWorkingStatusTbl.Rows.Count < 1)
                        {
                            foreach (DataRow row in tempTableComposition.Rows)
                            {
                                mWorkingStatusTbl.Rows.Add(row.Field<int>(ColumnWorkingStatus.PROCESS_IDX), row.Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID), null, SqlWorkingStatus.NON_REGISTRY_NAME);
                            }
                            mCompositionId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                            //mProductTypeId = new Guid();
                            if (mDtCycleDataMst != null)
                            {
                                mDtCycleDataMst.Clear();
                            }

                            return mSqlWorkingStatus.Insert(mWorkingStatusTbl);
                        }

                        // 前回のデータが存在する場合、最初にサイクルタイムセンサが現れる工程の品番を全行程に割り当てる
                        SqlDataCollectionTriggerMst sqlDataCollectionTriggerMst = new SqlDataCollectionTriggerMst(Program.AppSetting.SystemParam.DbConnect);
                        foreach (DataRow row in mWorkingStatusTbl.Rows)
                        {
                            // データ収集トリガマスタを検索。作業登録テーブルに登録されている編成ID,品番タイプID,工程インデックスから、センサIDの有無を検索する
                            compositionId = row.Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                            productTypeId = row.Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                            processIdx = row.Field<int>(ColumnWorkingStatus.PROCESS_IDX);

                            DataTable tempTable = sqlDataCollectionTriggerMst.Select(compositionId, productTypeId, processIdx);
                            if (tempTable == null || tempTable.Rows.Count.Equals(0))
                            {
                                continue;
                            }
                            if (tempTable.Rows.Count > 1)
                            {
                                // 工程インデックスまで含めて取得設定をしてもなお複数行取れる場合、どこかに被りがある
                                logger.Warn(String.Format("【GetQrData】センサ設定複数取得。編成ID：{0}, 品番タイプID：{1}, 工程インデックス：{2}", compositionId, productTypeId, processIdx));
                                continue;
                            }
                            if (!(tempTable.Rows[0][ColumnDataCollectionTriggerMst.SENSOR_ID_1] is System.DBNull))
                            {
                                // センサーID1が存在する
                                isExistSensor = true;
                                break;
                            }
                        }

                        if (isExistSensor)
                        {
                            // いったん全ての情報をクリアする
                            mWorkingStatusTbl.Clear();
                            mSqlWorkingStatus.Delete();

                            // センサーIDが取得できた場合、当該の品番タイプIDをもって登録する
                            foreach (DataRow row in tempTableComposition.Rows)
                            {
                                mWorkingStatusTbl.Rows.Add(row.Field<int>(ColumnWorkingStatus.PROCESS_IDX), row.Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID), productTypeId, SqlWorkingStatus.NON_REGISTRY_NAME);
                            }

                            // 編成、品番タイプ確定でCT上下限、良品カウンタ、工程カメラ情報の取得にかかる
                            mCompositionId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                            //mProductTypeId = productTypeId;
                            mDtCycleDataMst = mSqlStandardValProductTypeProcessMst.SelectCycleDataWithDataCollectionTrigerMst(mCompositionId, productTypeId);
                            mDtDataCollectionCameraMst = mSqlDataCollectionCameraMst.Select(mCompositionId, productTypeId);

                            logger.Debug(String.Format("【GetQrData】編成IDの一括更新。対象編成ID:{0}", mCompositionId));
                            ret = mSqlWorkingStatus.Insert(mWorkingStatusTbl);
                        }
                        else
                        {
                            // センサーIDが取得できなかった場合、異常としてDIOスレッドへ発報要求
                            // 工程に紐づかないエラーのため、全ブザーエラー発報(長音発報)
                            logger.Warn("【GetQrData】センサーIDの取得に失敗。全ブザーを発報します。");
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }
                        break;
                    case 2:
                        // 品番タイプ情報
                        // 品番マスタテーブルからアノテーション情報で渡される品番タイプ名の品番IDを取得する
                        SqlProductTypeMst sqlProductTypeMst = new SqlProductTypeMst(Program.AppSetting.SystemParam.DbConnect);
                        DataTable tempTableProductType = sqlProductTypeMst.Select(annotationInfo);
                        if (tempTableProductType == null || tempTableProductType.Rows.Count < 1)
                        {
                            // DB未登録の品番で検索されたため、DIOスレッドへ発報要求
                            // 工程に紐づかないエラーのため、全ブザーエラー発報(長音発報)
                            logger.Warn("【GetQrData】品番タイプマスタ未登録の品番読込。全ブザーを発報します。");
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }
                        Guid productId = tempTableProductType.Rows[0].Field<Guid>(ColumnProductTypeMst.PRODUCT_TYPE_ID);

                        if (mDtCycleDataMst != null)
                        {
                            mDtCycleDataMst.Clear();
                        }
                        mDtCycleDataMst = mSqlStandardValProductTypeProcessMst.SelectCycleDataWithDataCollectionTrigerMst(mCompositionId, productId);
                        mDtDataCollectionCameraMst = mSqlDataCollectionCameraMst.Select(mCompositionId, productId);
                        if (mDtCycleDataMst == null || mDtCycleDataMst.Rows.Count < 1)
                        {
                            // DB未登録の品番で検索されたため、DIOスレッドへ発報要求
                            logger.Warn(String.Format("【GetQrData】標準値マスタ未登録の品番読込。全ブザーを発報します。検索編成ID：{0}, 検索品番タイプID：{1}", mCompositionId, productId));
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }
                        if (mDtDataCollectionCameraMst == null || mDtDataCollectionCameraMst.Rows.Count < 1)
                        {
                            // DB未登録の品番で検索されたため、DIOスレッドへ発報要求
                            logger.Warn(String.Format("【GetQrData】標準値マスタのカメラ未登録の品番読込。全ブザーを発報します。検索編成ID：{0}, 検索品番タイプID：{1}", mCompositionId, productId));
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }

                        if (processIndex.Equals(0))
                        {
                            // 工程インデックスが0の場合、全工程の品番タイプを変更する(品番タイプQR一括変更)
                            // 編成、品番タイプ確定でCT上下限、良品カウンタ、工程カメラ情報の取得にかかる
                            //mProductTypeId = productId;

                            foreach (DataRow row in mWorkingStatusTbl.Rows)
                            {
                                row[ColumnProductTypeMst.PRODUCT_TYPE_ID] = productId;
                            }
                            mSqlWorkingStatus.Delete();
                            logger.Debug(String.Format("【GetQrData】全行程の品番タイプ更新。編成ID:{0}, 対象品番タイプID：{1}", mCompositionId, productId));
                            ret = mSqlWorkingStatus.Insert(mWorkingStatusTbl);

                        }
                        else
                        {
                            // 工程インデックスが0ではない場合、特定の工程の品番タイプを変更する
                            // 編成、品番タイプ確定でCT上下限、良品カウンタの取得にかかる
                            //mProductTypeId = productId;
                            var temp = mSqlStandardValProductTypeProcessMst.SelectCycleDataWithDataCollectionTrigerMst(mCompositionId, productId, (processIndex - 1));
                            if (temp.Rows.Count.Equals(1))
                            {
                                mDtCycleDataMst.Rows[processIndex - 1][ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER] = temp.Rows[0].Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER);
                                mDtCycleDataMst.Rows[processIndex - 1][ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER] = temp.Rows[0].Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER);
                                mDtCycleDataMst.Rows[processIndex - 1][ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER] = temp.Rows[0].Field<int>(ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER);
                            }
                            else
                            {
                                // DB未登録の品番で検索されたため、DIOスレッドへ発報要求
                                logger.Warn(String.Format("【GetQrData】特定工程の標準値マスタ未登録の品番読込。全ブザーを発報します。検索編成ID：{0}, 検索品番タイプID：{1}, 検索工程インデックス：{2}", mCompositionId, productId, processIndex));
                                GetLongBuzzerBitNumber(0, out listBuzzerBit);
                                dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                                return false;
                            }
                            mWorkingStatusTbl.Rows[processIndex - 1][ColumnProductTypeMst.PRODUCT_TYPE_ID] = productId;
                            logger.Debug(String.Format("【GetQrData】行程{0}の品番タイプ更新。編成ID:{1}, 対象品番タイプID：{2}", processIndex, mCompositionId, productId));
                            ret = mSqlWorkingStatus.Update(mWorkingStatusTbl);
                        }

                        // TODO:　個別変更の場合、次のサイクルからアノテーションを切り替えて実施する
                        break;
                    case 3:
                        // 作業者名情報
                        // QR情報で指定された工程インデックスの作業者名をアノテーション情報で置き換える
                        if (processIndex > 0 && mWorkingStatusTbl.Rows.Count > processIndex - 1)
                        {
                            mWorkingStatusTbl.Rows[processIndex - 1][ColumnWorkingStatus.WORKER_NAME] = annotationInfo;
                            ret = mSqlWorkingStatus.Update(mWorkingStatusTbl);

                            // 特定のブザーに対して成功発報(短音発報)
                            Guid compId = mWorkingStatusTbl.Rows[processIndex - 1].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                            Guid prodId = mWorkingStatusTbl.Rows[processIndex - 1].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                            int buzzerBit = -1;
                            GetShortBuzzerBitNumber(compId, prodId, processIndex, out buzzerBit);
                            if (buzzerBit > -1)
                            {
                                dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, buzzerBit, BuzzerRingStatus.INDIVIDUAL));
                            }
                        }
                        else
                        {
                            //現在の編成に存在しない工程インデックスの作業者QRを読んだ
                            // 工程に紐づかないエラーのため、全ブザーエラー発報(長音発報)
                            GetLongBuzzerBitNumber(0, out listBuzzerBit);
                            dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.READ_QR, listBuzzerBit, BuzzerRingStatus.ALL));
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【GetQrData】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 正常発報対象とするブザーの短音接点番号を取得する
        /// </summary>
        /// <param name="processIndex">ブザーが紐づいている工程インデックス</param>
        /// <param name="listBuzzerBit">ブザーの接点番号リスト</param>
        /// <param name="compositionId">編成ID</param>
        /// <param name="productId">品番タイプID</param>
        /// <returns></returns>
        private Boolean GetShortBuzzerBitNumber(Guid compositionId, Guid productId, int processIndex, out int buzzerBit)
        {
            buzzerBit = -1;
            Boolean ret = false;
            try
            {
                SqlDoDeviceMst sqlDoDeviceMst = new SqlDoDeviceMst(Program.AppSetting.SystemParam.DbConnect);
                // 工程に紐づくため、個別のブザーIDをとる

                using (DataTable dt = sqlDoDeviceMst.SelectWithDataCollectionDoMaster(compositionId, productId, (processIndex - 1)))
                {
                    if (dt != null)
                    {
                        foreach (DataRow rows in dt.Rows)
                        {
                            int buzzerShortOut = rows.Field<int>(ColumnDoDeviceMst.SHORT_OUTPUT_NO);
                            buzzerBit = buzzerShortOut - 1;
                        }
                    }
                    else
                    {
                        return false;
                    }
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
        /// エラー発報対象とするブザーの長音接点番号を取得する
        /// </summary>
        /// <param name="processIndex">ブザーが紐づいている工程インデックス(0の場合は工程紐づき無しとみなす)</param>
        /// <param name="listBuzzerBit">ブザーの接点番号リスト</param>
        /// <param name="compositionId">編成ID</param>
        /// <param name="productId">品番タイプID</param>
        /// <returns></returns>
        private Boolean GetLongBuzzerBitNumber(int processIndex, out List<int> listBuzzerBit, Guid compositionId = new Guid(), Guid productId = new Guid())
        {
            listBuzzerBit = new List<int>();
            Boolean ret = false;
            try
            {
                listBuzzerBit.Clear();
                SqlDoDeviceMst sqlDoDeviceMst = new SqlDoDeviceMst(Program.AppSetting.SystemParam.DbConnect);
                // 工程に紐づくため、個別のブザーIDをとる
                if (processIndex > 0)
                {
                    using (DataTable dt = sqlDoDeviceMst.SelectWithDataCollectionDoMaster(compositionId, productId, (processIndex - 1)))
                    {
                        if (dt != null)
                        {
                            foreach (DataRow rows in dt.Rows)
                            {
                                int buzzerLongOut = rows.Field<int>(ColumnDoDeviceMst.LONG_OUTPUT_NO);
                                listBuzzerBit.Add(buzzerLongOut - 1);
                            }
                        }
                    }
                }
                // 工程に紐づかないため、全接点を取得する
                else
                {
                    using (DataTable dt = sqlDoDeviceMst.Select())
                    {
                        if (dt != null)
                        {
                            foreach (DataRow rows in dt.Rows)
                            {
                                int buzzerLongOut = rows.Field<int>(ColumnDoDeviceMst.LONG_OUTPUT_NO);
                                listBuzzerBit.Add(buzzerLongOut - 1);
                            }
                        }
                    }
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
        /// サイクルの実績を作成する
        /// </summary>
        /// <param name="processIndex"></param>
        /// <param name="cycleChangeTime"></param>
        /// <returns></returns>
        private Boolean UpdateCycleTime(int processIndex, DateTime cycleStartTime, DateTime cycleEndTime, DateTime operationgDate, int operationShift, Guid compositionId, Guid productTypeId, String workerName, DataTable cycleDataMst, out int errorFlg)
        {
            Boolean ret = false;
            errorFlg = -1;
            try
            {
                DataTable table = mSqlCycleResultTbl.SelectCycleExistCount(compositionId, productTypeId, cycleStartTime, processIndex);
                if (table.Rows.Count > 0)
                {
                    logger.Debug(String.Format("【UpdateCycleTime】対象時刻・工程Idxサイクル実績存在。工程インデックス：{0}, サイクル開始時刻：{1}", processIndex, cycleStartTime.ToString("yyyy/MM/dd HH:mm.ss.fff")));
                    return true;
                }

                // サイクル実績テーブルの形式をもつ空テーブルを明示的に作成し、1行追加する
                DataTable insertData = new DataTable(ColumnCycleResult.TABLE_NAME);
                insertData.Rows.Clear();
                insertData.Rows.Add(insertData.NewRow());

                // 編成ID
                insertData.Columns.Add(ColumnCycleResult.COMPOSITION_ID, typeof(Guid));
                insertData.Rows[0][ColumnCycleResult.COMPOSITION_ID] = compositionId;
                // 品番タイプID
                insertData.Columns.Add(ColumnCycleResult.PRODUCT_TYPE_ID, typeof(Guid));
                insertData.Rows[0][ColumnCycleResult.PRODUCT_TYPE_ID] = productTypeId;
                // 工程インデックス
                insertData.Columns.Add(ColumnCycleResult.PROCESS_IDX, typeof(int));
                insertData.Rows[0][ColumnCycleResult.PROCESS_IDX] = processIndex;
                // 開始時刻
                insertData.Columns.Add(ColumnCycleResult.START_TIME, typeof(DateTime));
                insertData.Rows[0][ColumnCycleResult.START_TIME] = cycleStartTime;
                // 終了時刻
                insertData.Columns.Add(ColumnCycleResult.END_TIME, typeof(DateTime));
                insertData.Rows[0][ColumnCycleResult.END_TIME] = cycleEndTime;
                // サイクルタイム
                decimal cycleTime = decimal.Round((decimal)(cycleEndTime - cycleStartTime).TotalSeconds, 3);
                insertData.Columns.Add(ColumnCycleResult.CYCLE_TIME, typeof(decimal));
                insertData.Rows[0][ColumnCycleResult.CYCLE_TIME] = cycleTime;
                // 稼働日
                insertData.Columns.Add(ColumnCycleResult.OPERATION_DATE, typeof(DateTime));
                //insertData.Rows[0][ColumnCycleResult.OPERATION_DATE] = mOperatingDate;
                insertData.Rows[0][ColumnCycleResult.OPERATION_DATE] = operationgDate;
                // 稼働シフト
                insertData.Columns.Add(ColumnCycleResult.OPERATION_SHIFT, typeof(int));
                insertData.Rows[0][ColumnCycleResult.OPERATION_SHIFT] = operationShift;
                // 作業者名
                insertData.Columns.Add(ColumnCycleResult.WORKER_NAME, typeof(String));
                insertData.Rows[0][ColumnCycleResult.WORKER_NAME] = workerName;

                // 異常フラグ上下限
                decimal upper = 10000;
                decimal lower = 0;
                // 良品フラグ(良品カウンタ通過工程)
                int goodFlg = -1;

                // mDtCycleDataMst -> cycleDataMst
                if (cycleDataMst.Rows.Count > 0)
                {
                    upper = cycleDataMst.Rows[processIndex].Field<decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER);
                    lower = cycleDataMst.Rows[processIndex].Field<decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER);
                }

                errorFlg = (lower < cycleTime && cycleTime < upper) ? 0 : 1;
                insertData.Columns.Add(ColumnCycleResult.ERROR_FLAG, typeof(int));
                insertData.Rows[0][ColumnCycleResult.ERROR_FLAG] = 0;
                // 良品フラグ(良品カウンタ通過工程)
                goodFlg = cycleDataMst.Rows[processIndex].Field<int>(ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER);
                insertData.Columns.Add(ColumnCycleResult.GOOD_FLAG, typeof(int));
                insertData.Rows[0][ColumnCycleResult.GOOD_FLAG] = goodFlg > 0 ? 1 : 0;

                logger.Debug(String.Format("【UpdateCycleTime】サイクル実績作成。工程インデックス：{0}, サイクル開始時刻：{1}", processIndex, cycleStartTime.ToString("yyyy/MM/dd HH:mm.ss.fff")));
                ret = mSqlCycleResultTbl.Insert(insertData);
                //mCycleStartDateTime = cycleChangeTime;

                // 「直稼働時間内」で良品フラグ通過実績のみ、稼働シフト品番タイプ毎生産数テーブルへデータ作成する
                if (goodFlg > 0 && mIsOperating)
                {
                    // 稼働シフト品番タイプごと生産数テーブルの形式をもつ空テーブルを明示的に作成し、1行追加する
                    DataTable opeData = new DataTable(ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME);
                    opeData.Rows.Clear();
                    opeData.Rows.Add(opeData.NewRow());
                    // 稼働日
                    opeData.Columns.Add(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE, typeof(DateTime));
                    //opeData.Rows[0][ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE] = mOperatingDate;
                    opeData.Rows[0][ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE] = operationgDate;
                    // 稼働シフト
                    opeData.Columns.Add(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT, typeof(int));
                    opeData.Rows[0][ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT] = operationShift;
                    // 編成ID
                    opeData.Columns.Add(ColumnCycleResult.COMPOSITION_ID, typeof(Guid));
                    opeData.Rows[0][ColumnCycleResult.COMPOSITION_ID] = compositionId;
                    // 品番タイプID
                    opeData.Columns.Add(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID, typeof(Guid));
                    opeData.Rows[0][ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID] = productTypeId;
                    // 編集生産数
                    opeData.Columns.Add(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE, typeof(int));
                    opeData.Rows[0][ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE] = 1;

                    logger.Debug(String.Format("【UpdateCycleTime】稼働シフト品番タイプ毎生産数データ作成。工程インデックス：{0}, サイクル開始時刻：{1}", processIndex, cycleStartTime.ToString("yyyy/MM/dd HH:mm.ss.fff")));
                    ret = mSqlOperatingShiftProductionQuantityTbl.Upsert(opeData);
                }

            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// サイクル実績の現象動画ファイル名を更新する
        /// </summary>
        /// <param name="procIdx"></param>
        /// <param name="cycleTime"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Boolean UpdateCycleTimeWithFileName(int procIdx, DateTime cycleTime, String fileName, DataTable shiftTable)
        {
            Boolean ret = false;
            try
            {
                // サイクル実績テーブルの形式をもつ空テーブルを明示的に作成し、1行追加する
                DataTable updateData = new DataTable(ColumnCycleResult.TABLE_NAME);
                updateData.Rows.Clear();
                updateData.Rows.Add(updateData.NewRow());

                // 編成ID
                updateData.Columns.Add(ColumnCycleResult.COMPOSITION_ID, typeof(Guid));
                updateData.Rows[0][ColumnCycleResult.COMPOSITION_ID] = shiftTable.Rows[procIdx].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                // 品番タイプID
                updateData.Columns.Add(ColumnCycleResult.PRODUCT_TYPE_ID, typeof(Guid));
                updateData.Rows[0][ColumnCycleResult.PRODUCT_TYPE_ID] = shiftTable.Rows[procIdx].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                // 工程インデックス
                updateData.Columns.Add(ColumnCycleResult.PROCESS_IDX, typeof(int));
                updateData.Rows[0][ColumnCycleResult.PROCESS_IDX] = procIdx;
                // 開始時刻
                updateData.Columns.Add(ColumnCycleResult.START_TIME, typeof(DateTime));
                updateData.Rows[0][ColumnCycleResult.START_TIME] = cycleTime;
                //// 稼働日
                //updateData.Columns.Add(ColumnCycleResult.OPERATION_DATE, typeof(DateTime));
                //updateData.Rows[0][ColumnCycleResult.OPERATION_DATE] = mOperatingDate;
                //// 稼働シフト
                //updateData.Columns.Add(ColumnCycleResult.OPERATION_SHIFT, typeof(int));
                //updateData.Rows[0][ColumnCycleResult.OPERATION_SHIFT] = mOperatingShift;

                // 現象動画ファイル名
                updateData.Columns.Add(ColumnCycleResult.VIDEO_FILE_NAME, typeof(String));
                updateData.Rows[0][ColumnCycleResult.VIDEO_FILE_NAME] = fileName;

                mSqlCycleResultTbl.Update(updateData);

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
        /// 直開始時の設定を取得し、録画・録音開始要求を投げる
        /// </summary>
        /// <returns></returns>
        private Boolean ShiftStartAndRecordOn()
        {
            Boolean ret = false;
            logger.Debug(String.Format("【ShiftStartAndRecordOn】直開始時の設定を取得し、録画・録音開始要求を発行"));
            try
            {
                if (mWorkingStatusTbl == null || mWorkingStatusTbl.Rows.Count < 1)
                {
                    logger.Debug(String.Format("【ShiftStartAndRecordOn】稼働登録情報登録なし"));
                    return false;
                }

                // DBに登録されているカメラの設定を拾いなおし、カメラ、マイク各スレッドに初期化をかける
                mDtCameraDeviceMst = mSqlCameraDeviceMst.Select();
                for (int i = 0; i < mDtCameraDeviceMst.Rows.Count; i++)
                {
                    String cameraId = mDtCameraDeviceMst.Rows[i].Field<String>(ColumnCameraDeviceMst.CAMERA_DEVICE_CODE);
                    String micId = mDtCameraDeviceMst.Rows[i].Field<String>(ColumnCameraDeviceMst.MIC_DEVICE_CODE);
                    Double fps = mDtCameraDeviceMst.Rows[i].Field<int>(ColumnCameraDeviceMst.DEVICE_NUMNBER);
                    webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.INIT, cameraId, fps, true));
                    micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.INIT, micId, true));
                }

                mCompositionId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                //mProductTypeId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                {
                    var tempProductTpeId = mWorkingStatusTbl.Rows[0].Field<Guid?>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                    if (tempProductTpeId is null)
                    {
                        logger.Warn("品番タイプIDが登録されていません。録画・録音指示は出しません。");
                        return false;
                    }
                }
                Guid productTypeId = mWorkingStatusTbl.Rows[0].Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                mDtDataCollectionCameraMst = mSqlDataCollectionCameraMst.Select(mCompositionId, productTypeId);
                if (mDtDataCollectionCameraMst != null)
                {
                    // 直の録画開始要求
                    for (int i = 0; i < mCamNumMax; i++)
                    {
                        // 対象カメラを使用する工程を特定する
                        List<int> saveStatus = new List<int>();
                        String columnName = "";
                        switch (i)
                        {
                            case 0:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1;
                                break;
                            case 1:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2;
                                break;
                            case 2:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3;
                                break;
                            case 3:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4;
                                break;
                            case 4:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5;
                                break;
                            case 5:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6;
                                break;
                            case 6:
                                columnName = ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7;
                                break;
                            default:
                                columnName = "";
                                break;
                        }
                        for (int ii = 0; ii < mWorkingStatusTbl.Rows.Count; ii++)
                        {
                            if (mDtDataCollectionCameraMst.Rows.Count == 0 || mDtDataCollectionCameraMst.Rows.Count < ii)
                            {
                                break;
                            }
                            // 2022.09.26 有効無効で生かす工程の判断が足りない
                            int? temp = mDtDataCollectionCameraMst.Rows[ii].Field<int?>(columnName);
                            int status = temp != null ? (int)temp : 0;

                            saveStatus.Add(status);
                        }

                        webCamCtrlThreads[i].AddEvent(new CmdWebcamCtrl(WebCamCtrlThreadStatus.SHIFT_START, mCycleStartDateTime, mWorkingStatusTbl.Rows.Count, saveStatus, mOperatingShift));
                        micCtrlThreads[i].AddEvent(new CmdMicCtrl(MicCtrlThreadStatus.SHIFT_START, mCycleStartDateTime, saveStatus));
                    }
                }
                dioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.SHIFT_START, new CmdDioCtrlShiftTime(mOperatingShiftStartTime, mOperatingShiftEndTime)));
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        public void CheckShiftUpdate()
        {
            if (mIsOperating)
            {
                // 直実行中は更新できない
            }
            else
            {
                mIsInitGetShift = false;
            }
        }

        #endregion
    }

    public class CmdEventManager
    {
        public EventManagementThreadStatus mStatus;
        public CmdEventQrReadParam mQrParam;
        /// <summary> 工程インデックス </summary>
        public int mProcessIndex = -1;
        /// <summary> サイクル開始時刻 </summary>
        public DateTime mDateTime;


        public String mSaveFileName;
        public List<String> mListFilePath;
        public SaveFileType mFileType;
        public int mCameraIndex = -1;
        public int mOperationShift = -1;
        public CycleStartEndDateTime mCycleStartEnd;
        public int mSaveStatus = -1;
        public DateTime mOperationDate;

        public Guid mCompositionId;
        public Guid mProductTypeId;
        public String mWorkerName;

        public DataTable mShiftTable;
        public DataTable mCycleMstTable;

        public CmdEventManager(EventManagementThreadStatus status)
        {
            mStatus = status;
        }
        public CmdEventManager(EventManagementThreadStatus status, CmdEventQrReadParam qrParam)
        {
            mStatus = status;
            mQrParam = qrParam;
        }
        public CmdEventManager(EventManagementThreadStatus status, int processIndex, DateTime dateTime)
        {
            mStatus = status;
            mProcessIndex = processIndex;
            mDateTime = dateTime;
        }
        /// <summary>
        /// 音声保存イベント時のコンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="camIdx"></param>
        /// <param name="procIdx"></param>
        /// <param name="cycleTime"></param>
        public CmdEventManager(EventManagementThreadStatus status, String fileName, SaveFileType type, int camIdx, int procIdx, CycleStartEndDateTime cycleTime, int shift, int saveStatus)
        {
            mStatus = status;
            mSaveFileName = fileName;
            mFileType = type;
            mCameraIndex = camIdx;
            mProcessIndex = procIdx;
            mCycleStartEnd = cycleTime;
            mOperationShift = shift;
            mSaveStatus = saveStatus;
        }
        /// <summary>
        /// 動画保存イベント時のコンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="camIdx"></param>
        /// <param name="procIdx"></param>
        /// <param name="cycleTime"></param>
        public CmdEventManager(EventManagementThreadStatus status, String fileName, List<String> listFilePath, SaveFileType type, int camIdx, int procIdx, CycleStartEndDateTime cycleTime, int shift, int saveStatus, DateTime operatingDate, Guid comp, Guid prod, String worker, DataTable shiftTable, DataTable cycleMstTable)
        {
            mStatus = status;
            mSaveFileName = fileName;
            mListFilePath = listFilePath;
            mFileType = type;
            mCameraIndex = camIdx;
            mProcessIndex = procIdx;
            mCycleStartEnd = cycleTime;
            mOperationShift = shift;
            mSaveStatus = saveStatus;
            mOperationDate = operatingDate;

            mCompositionId = comp;
            mProductTypeId = prod;
            mWorkerName = worker;
            mShiftTable = shiftTable;
            mCycleMstTable = cycleMstTable;
        }
        /// <summary>
        /// 動画・音声保存イベント時のコンストラクタ
        /// 音声登録がない場合の音声保存イベント用
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="camIdx"></param>
        /// <param name="procIdx"></param>
        /// <param name="cycleTime"></param>
        public CmdEventManager(EventManagementThreadStatus status, String fileName, SaveFileType type, int camIdx, int procIdx, DateTime cycleTime)
        {
            mStatus = status;
            mSaveFileName = fileName;
            mFileType = type;
            mCameraIndex = camIdx;
            mProcessIndex = procIdx;
            mDateTime = cycleTime;
        }

        /// <summary>
        /// 動画保存完了時のイベント発行
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        /// <param name="procIdx"></param>
        /// <param name="cycleTime"></param>
        public CmdEventManager(EventManagementThreadStatus status, String fileName, int procIdx, DateTime cycleTime, DataTable shiftTable)
        {
            mStatus = status;
            mSaveFileName = fileName;
            mProcessIndex = procIdx;
            mDateTime = cycleTime;
            mShiftTable = shiftTable;
        }
    }
    public class CmdEventQrReadParam
    {
        public int mDataNo;
        public int mProcessIndex;
        public String mAnnotationInfo;

        public CmdEventQrReadParam(int dataNo, int processIndex, String annotationInfo)
        {
            mDataNo = dataNo;
            mProcessIndex = processIndex;
            mAnnotationInfo = annotationInfo;
        }
    }

    public class ConcatMovieInfo
    {
        /// <summary> 録画ファイル名 </summary>
        public String mVideoFile;
        /// <summary> 録画ファイル名 </summary>
        public List<String> mAviVideoFileList;
        /// <summary> 音声ファイル名 </summary>
        public String mAudioFile;
        /// <summary> 工程インデックス </summary>
        public int mProcessIdx;
        /// <summary> カメラインデックス </summary>
        public int mCameraIdx;
        /// <summary> サイクル開始時刻 </summary>
        public DateTime mCycleTime;
        /// <summary> 録画ファイル保存完了フラグ </summary>
        public Boolean mVideoSavedFlg = false;
        /// <summary> 音声ファイル保存完了フラグ </summary>
        public Boolean mAudioSavedFlg = false;

        public DataTable mShiftTable;

        public ConcatMovieInfo(int procIdx, int camIdx, DateTime cycleTime, String video, List<String> aviVideoFileList, String audio, Boolean videoFlg, Boolean audioFlg, DataTable shiftTable)
        {
            mProcessIdx = procIdx;
            mCameraIdx = camIdx;
            mCycleTime = cycleTime;
            if (!String.IsNullOrEmpty(video) && videoFlg)
            {
                mVideoFile = video;
                mVideoSavedFlg = true;
            }
            if (!String.IsNullOrEmpty(audio))
            {
                mAudioFile = audio;
                mAudioSavedFlg = true;
            }
            mAviVideoFileList = aviVideoFileList;

            mShiftTable = shiftTable;
        }

    }
    public class CycleStartEndDateTime
    {
        public DateTime cycleStart;
        public DateTime cycleEnd;

        public CycleStartEndDateTime(DateTime start, DateTime end)
        {
            cycleStart = start;
            cycleEnd = end;
        }
    }
}

