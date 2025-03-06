using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using DBConnect.SQL;
using System.Data;
using log4net;
using System.Threading;

namespace PLOSDeviceConnectionManager.Thread
{

    public enum DioCtrlThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> DIO通知 </summary>
        STATUS_RECV,
        /// <summary> 直開始 </summary>
        SHIFT_START,
        /// <summary> 直終了 </summary>
        SHIFT_END,
        /// <summary> QR読込 </summary>
        READ_QR,
    }
    /// <summary>
    /// トリガタイプ種別
    /// </summary>
    public enum TrigerType
    {
        /// <summary> ONエッジ </summary>
        ON_EDGE = 1,
        /// <summary> AND </summary>
        AND,
        /// <summary> ONメモリ </summary>
        ON_MEMORY,
        /// <summary> OFFエッジ </summary>
        OFF_EDGE,
        /// <summary> OR </summary>
        OR,
    }
    /// <summary>
    /// DIOコントローラ通信管理スレッドクラス
    /// 本スレッド内からさらにDIOと定期通信するスレッドを起こし、定時監視を行う
    /// 本スレッドは他スレッドとのイベント送受信、ソフト上のトリガ管理、CT発行などを実施する
    /// </summary>
    public class DioCtrlThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary> DIOの入出力点数(I/Oとも16点ずつ固定) </summary>
        private const int BIT_NUM = 16;
        /// <summary> ブザー鳴動時間(秒) </summary>
        private double buzzerRingingTime = 0;

        /// <summary> DIO通信スレッドのインスタンス </summary>
        private DioThread mDioThread;
        /// <summary> 現在の直の開始時刻 </summary>
        private DateTime mOperatingShiftStartTime;
        /// <summary> 現在の直の終了時刻 </summary>
        private DateTime mOperatingShiftEndTime;

        /// <summary> 稼働登録テーブル接続インスタンス </summary>
        private SqlWorkingStatus mSqlWorkingStatus;
        /// <summary> 取得した稼働登録情報 </summary>
        private DataTable mWorkingStatusTbl;

        /// <summary> 前回取得したDIOの入力ビット数値 </summary>
        private ushort mPrevInputStatus = 0;
        /// <summary> 今回取得したDIOの入力ビット数値 </summary>
        private ushort mThisInputStatus = 0;

        /// <summary> 直稼働中の是非(True:直稼働中) </summary>
        private Boolean mIsOperatingShift = false;

        /// <summary> 実センサの入力状態リスト(外部参照用にpublic static化) </summary>
        public static readonly List<HardSensorStatus> listHardSensorStatus = new List<HardSensorStatus>();
        /// <summary> 実センサの出力状態リスト(外部参照用にpublic static化) </summary>
        public static readonly List<HardOutputStatus> listHardOutputStatus = new List<HardOutputStatus>();
        /// <summary> トリガの発行状態リスト(外部参照用にpublic static化) </summary>
        public static readonly List<TriggerIssuanceStatus> listTriggerIssuanceStatus = new List<TriggerIssuanceStatus>();
        /// <summary> オンメモリ状態リスト(外部参照用にpublic static化) </summary>
        public static readonly List<Boolean> listOnMemoryStatus = new List<Boolean>();

        /// <summary> 内部センサの状態リスト </summary>
        //public static readonly List<InnerSensorStatus> mListInnerSensorStatus = new List<InnerSensorStatus>();
        /// <summary> 内部センサの状態の工程ごと管理リスト(入力16点のリストを工程数分リストとして保持) </summary>
        public static readonly List<List<InnerSensorStatus>> mListInnerSensorStatusByProcessIndex = new List<List<InnerSensorStatus>>();

        /// <summary> 【DI情報】実センサ状態保持クラス </summary>
        public class HardSensorStatus
        {
            /// <summary> (実センサ状態保持クラス)インデックス(DIO接点番号と同) </summary>
            public int mIndex = -1;
            /// <summary> (実センサ状態保持クラス)前回入力ビット状態  </summary>
            public Boolean mPrevBitStatus = false;
            /// <summary> (実センサ状態保持クラス)今回入力ビット状態  </summary>
            public Boolean mThisBitStatus = false;
            /// <summary> (実センサ状態保持クラス)入力ビット変化時刻 </summary>
            public DateTime mChangedDateTime;
            public HardSensorStatus(int index)
            {
                mIndex = index;
            }
        }

        /// <summary> 【DI情報】内部センサ状態保持クラス </summary>
        public class InnerSensorStatus
        {
            /// <summary> (内部センサ状態保持クラス)インデックス(DIO接点番号と同) </summary>
            public int mIndex = -1;
            /// <summary> (内部センサ状態保持クラス)入力ビット状態 </summary>
            public Boolean mInnerBitStatus = false;
            /// <summary> (内部センサ状態保持クラス)入力ビット変化時刻 </summary>
            public DateTime mChangedDateTime;
            public InnerSensorStatus(int index)
            {
                mIndex = index;
                mInnerBitStatus = false;
            }
        }

        /// <summary> 【DI情報】トリガ発行状態保持クラス </summary>
        public class TriggerIssuanceStatus
        {
            /// <summary> (トリガ発行状態保持クラス)インデックス(工程インデックスと同義) </summary>
            public int mIndex = -1;
            /// <summary> (トリガ発行状態保持クラス)トリガ発行許可 </summary>
            public Boolean mTriggerIssuancePermission = false;
            /// <summary> (トリガ発行状態保持クラス)トリガ認識時刻 </summary>
            public DateTime mChangedDateTime;
            public TriggerIssuanceStatus(int index)
            {
                mIndex = index;
                mTriggerIssuancePermission = true;
            }
        }

        /// <summary> 出力状態保持クラス </summary>
        public class HardOutputStatus
        {
            /// <summary> (出力状態保持クラス)インデックス(DIO接点番号と同) </summary>
            public int mIndex = -1;
            /// <summary> (出力状態保持クラス)出力ビット状態 </summary>
            public Boolean mOutputBitStatus = false;
            /// <summary> (出力状態保持クラス)出力ビット変化時刻 </summary>
            public DateTime mChangedDateTime;
            public HardOutputStatus(int index)
            {
                mIndex = index;
            }
        }

        #endregion

        #region <Property>
        #endregion

        #region <Constructor>

        public DioCtrlThread(int cycleTime, out Boolean isInit)
        {
            isInit = false;

            this.CycleTime = cycleTime;
            if (Double.TryParse(Program.AppSetting.DioParam.BuzzerRingingTime, out buzzerRingingTime))
            {
                // Do Nothing
            }
            else
            {
                logger.Error("ブザー鳴動時間設定異常");
                isInit = false;
                return;
            }

            // 各状態リストのクリア
            listHardSensorStatus.Clear();
            //mListInnerSensorStatus.Clear();
            listHardOutputStatus.Clear();
            mListInnerSensorStatusByProcessIndex.Clear();

            // 実センサ状態リスト、内部センサ状態リストの初期化(16点固定)
            for (int i = 0; i < BIT_NUM; i++)
            {
                listHardSensorStatus.Add(new HardSensorStatus(i));
                //mListInnerSensorStatus.Add(new InnerSensorStatus(i));
                listHardOutputStatus.Add(new HardOutputStatus(i));
            }

            // DIO実通信スレッドの起動
            mDioThread = new DioThread(Program.AppSetting.SystemParam.TimeInterval, Program.AppSetting.DioParam.DeviceName, this);
            if (!mDioThread.InitDevice())
            {
                logger.Error("DIOデバイス初期化失敗");
                isInit = false;
                return;
            }
            mSqlWorkingStatus = new SqlWorkingStatus(Program.AppSetting.SystemParam.DbConnect);

            pr = ThreadPriority.Highest;

            //#endif
            isInit = true;

        }

        #endregion

        #region <Method>

        #region <OverRide>
        /// <summary>
        /// 定周期処理
        /// </summary>
        protected override void Proc()
        {
            if (mIsOperatingShift)
            {
                ChangeInnerSensorStatus();
            }
            MonitorBuzzerOff();
            base.Proc();
        }

        /// <summary>
        /// イベント受信処理
        /// </summary>
        /// <param name="obj"></param>
        protected override void QueueEvent(object obj)
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is CmdDioCtrl)
            {
                switch (((CmdDioCtrl)obj).mStatus)
                {
                    // 起動
                    case DioCtrlThreadStatus.INIT:
                        mDioThread.Start();
                        break;
                    // 終了
                    case DioCtrlThreadStatus.TERM:
                        mDioThread.ExitDevice();
                        mDioThread.Stop();
                        break;
                    // DIO通知受信
                    case DioCtrlThreadStatus.STATUS_RECV:
                        mThisInputStatus = ((CmdDioCtrl)obj).mInputStatus;
                        UpdateInputBitStatus();
                        break;
                    // 直開始通知受信
                    case DioCtrlThreadStatus.SHIFT_START:
                        mIsOperatingShift = true;
                        // 最初のサイクルデータを詰みこむ。
                        mOperatingShiftStartTime = ((CmdDioCtrl)obj).mShiftDateTime.mShiftStartDateTime;
                        mOperatingShiftEndTime = ((CmdDioCtrl)obj).mShiftDateTime.mShiftEndDateTime;

                        listTriggerIssuanceStatus.Clear();
                        listOnMemoryStatus.Clear();

                        List<List<InnerSensorStatus>> cloneListInnerSensorStatusByProcIdx = new List<List<InnerSensorStatus>>(mListInnerSensorStatusByProcessIndex);
                        int cloneCount = cloneListInnerSensorStatusByProcIdx.Count;
                        mListInnerSensorStatusByProcessIndex.Clear();

                        // 直の編成、品番タイプ、工程インデックスからとれるセンサの接点番号等を取得する
                        mWorkingStatusTbl = mSqlWorkingStatus.SelectProcessInfo();
                        if (mWorkingStatusTbl != null && mWorkingStatusTbl.Rows.Count > 0)
                        {
                            if (cloneCount > 0)
                            {
                                for (int i = 0; i < mWorkingStatusTbl.Rows.Count; i++)
                                {
                                    listTriggerIssuanceStatus.Add(new TriggerIssuanceStatus(i));
                                    listOnMemoryStatus.Add(false);

                                    if(cloneCount > i)
                                    {
                                        mListInnerSensorStatusByProcessIndex.Add(cloneListInnerSensorStatusByProcIdx[i]);
                                    }
                                    else
                                    {
                                        // 各工程ごとに管理する内部センサ状態
                                        List<InnerSensorStatus> listTemp = new List<InnerSensorStatus>();
                                        for (int ii = 0; ii < BIT_NUM; ii++)
                                        {
                                            listTemp.Add(new InnerSensorStatus(ii));
                                        }
                                        mListInnerSensorStatusByProcessIndex.Add(listTemp);
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < mWorkingStatusTbl.Rows.Count; i++)
                                {
                                    listTriggerIssuanceStatus.Add(new TriggerIssuanceStatus(i));
                                    listOnMemoryStatus.Add(false);

                                    // 各工程ごとに管理する内部センサ状態
                                    List<InnerSensorStatus> listTemp = new List<InnerSensorStatus>();
                                    for (int ii = 0; ii < BIT_NUM; ii++)
                                    {
                                        listTemp.Add(new InnerSensorStatus(ii));
                                    }
                                    mListInnerSensorStatusByProcessIndex.Add(listTemp);
                                }
                            }
                        }
                        else
                        {
                            logger.Warn(String.Format("DIO管理クラスのSHIFT_STARTにて、直情報の取得に失敗しました。"));
                        }
                        break;
                    // 直終了通知受信
                    case DioCtrlThreadStatus.SHIFT_END:
                        mIsOperatingShift = false;
                        for (int i = 0; i < listOnMemoryStatus.Count; i++)
                        {
                            listOnMemoryStatus[i] = false;
                        }
                        break;
                    // QR読込通知
                    case DioCtrlThreadStatus.READ_QR:
                        if (((CmdDioCtrl)obj).mBuzzerStatus == BuzzerRingStatus.ALL)
                        {
                            BuzzerOnList(((CmdDioCtrl)obj).mBuzzerBitList);
                        }
                        else if (((CmdDioCtrl)obj).mBuzzerStatus == BuzzerRingStatus.INDIVIDUAL)
                        {
                            BuzzerOn(((CmdDioCtrl)obj).mBuzzerBit);
                        }
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// 【DI状態取得】
        /// センサ状態を取得し、サイクルタイムの開始を通知する
        /// </summary>
        /// <returns></returns>
        private Boolean ChangeInnerSensorStatus()
        {
            Boolean ret = false;
            int dioInputNoTrigger1 = -1;
            int dioInputNoTrigger2 = -1;

            try
            {
                //　現在時刻
                DateTime triggerIssuanceTime = DateTime.Now;
                if (mWorkingStatusTbl == null || mWorkingStatusTbl.Rows.Count < 1)
                {
                    ret = false;
                }
                else
                {
                    for (int processIndex = 0; processIndex < mWorkingStatusTbl.Rows.Count; processIndex++)
                    {
                        // 機器の有効化状態の取得
                        Boolean enable = mWorkingStatusTbl.Rows[processIndex].Field<Boolean?>(ColumnStandardValProductTypeProcessMst.USE_FLAG) != null ? mWorkingStatusTbl.Rows[processIndex].Field<Boolean>(ColumnStandardValProductTypeProcessMst.USE_FLAG) : false;

                        // トリガタイプの取得
                        var temp = mWorkingStatusTbl.Rows[processIndex].Field<int?>(ColumnDataCollectionTriggerMst.TRIGGER_TYPE);
                        int triggerType = temp != null ? (int)temp : -1;

                        // 入力接点番号の取得(DB値1～16をこの時点で計算し、0～15に変換)
                        temp = mWorkingStatusTbl.Rows[processIndex].Field<int?>(SqlWorkingStatus.INPUT_SENSOR1) - 1;
                        dioInputNoTrigger1 = temp != null ? (int)temp : -1;
                        temp = mWorkingStatusTbl.Rows[processIndex].Field<int?>(SqlWorkingStatus.INPUT_SENSOR2) - 1;
                        dioInputNoTrigger2 = temp != null ? (int)temp : -1;

                        if (enable && triggerType > 0)
                        {
                            switch (triggerType)
                            {
                                // ONエッジ
                                case (int)TrigerType.ON_EDGE:
                                    SingleEdgeChange(dioInputNoTrigger1, processIndex, triggerIssuanceTime, TrigerType.ON_EDGE, false, true, false);
                                    break;
                                // AND
                                case (int)TrigerType.AND:
                                    MultiEdgeChange(dioInputNoTrigger1, dioInputNoTrigger2, processIndex, triggerIssuanceTime, TrigerType.AND);
                                    break;
                                // ONメモリ
                                case (int)TrigerType.ON_MEMORY:
                                    MultiEdgeChangeOnMemory(dioInputNoTrigger1, dioInputNoTrigger2, processIndex, triggerIssuanceTime, TrigerType.ON_MEMORY);
                                    break;
                                // OFFエッジ
                                case (int)TrigerType.OFF_EDGE:
                                    SingleEdgeChange(dioInputNoTrigger1, processIndex, triggerIssuanceTime, TrigerType.OFF_EDGE, true, false, true);
                                    break;
                                // OR
                                case (int)TrigerType.OR:
                                    MultiEdgeChange(dioInputNoTrigger1, dioInputNoTrigger2, processIndex, triggerIssuanceTime, TrigerType.OR);
                                    break;
                                default:
                                    break;
                            }
                        }

                        DateTime changedTriggerTime = listTriggerIssuanceStatus[processIndex].mChangedDateTime;

                        var tempInterval = mWorkingStatusTbl.Rows[processIndex].Field<decimal?>(ColumnDataCollectionTriggerMst.INTERVAL_FILTER);
                        double triggerFilterMiliSecond = tempInterval != null ? (double)tempInterval : -1;
                        // (現在時刻 - トリガ発行時刻) > トリガ間フィルタ秒
                        if (triggerFilterMiliSecond > -1 && ((triggerIssuanceTime - changedTriggerTime).TotalSeconds > triggerFilterMiliSecond))
                        {
                            listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission = true;
                        }
                    }
                    ret = true;
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
        /// 【ブザー通知処理】
        /// 特定のbitの出力点をONする
        /// </summary>
        /// <param name="bitNo">ブザーを鳴らす出力接点</param>
        /// <returns></returns>
        private Boolean BuzzerOn(int bitNo)
        {
            Boolean ret = false;
            try
            {
                listHardOutputStatus[bitNo].mChangedDateTime = DateTime.Now;
                listHardOutputStatus[bitNo].mOutputBitStatus = true;
                if (mDioThread.OutputBitStatusByBitNo((short)bitNo, 1))
                {
                    // Do Nothing(正常)
                }
                else
                {
                    logger.Error(String.Format("BIT出力点のONに失敗。対象ビットNo:{0}", bitNo));
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
        /// 【ブザー通知処理】
        /// 複数点のbitの出力をONする
        /// </summary>
        /// <param name="listBitNo">ブザーを鳴らす出力接点のリスト</param>
        /// <returns></returns>
        private Boolean BuzzerOnList(List<int> listBitNo)
        {
            Boolean ret = false;
            try
            {
                foreach (var bitNo in listBitNo)
                {
                    BuzzerOn(bitNo);
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
        /// 【ブザーOFF監視（定周期）】
        /// 定周期で出力全点を確認し、ONから一定時間が経過したフラグをOFFにする
        /// </summary>
        /// <returns></returns>
        private Boolean MonitorBuzzerOff()
        {
            Boolean ret = false;
            try
            {
                DateTime monitorTime = DateTime.Now;

                for (int i = 0; i < listHardOutputStatus.Count; i++)
                {
                    if ((monitorTime - listHardOutputStatus[i].mChangedDateTime).TotalSeconds > buzzerRingingTime)
                    {
                        listHardOutputStatus[i].mOutputBitStatus = false;
                        mDioThread.OutputBitStatusByBitNo((short)i, 0);
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
        /// 取得した入力ビットの状態が前回と比べて変化しているかをチェックする
        /// </summary>
        private void UpdateInputBitStatus()
        {
            // 前回値と今回値の排他論理和(XOR)をとる
            int xorTemp = mPrevInputStatus ^ mThisInputStatus;
            short shortXor = (short)xorTemp;

            // XORした結果と今回値をBit配列化(short型で16ビット分)
            BitArray xorBitArray = new BitArray(BitConverter.GetBytes(shortXor));
            BitArray thisInputBitArrat = new BitArray(BitConverter.GetBytes(mThisInputStatus));

            for (int i = 0; i < xorBitArray.Length; i++)
            {
                if (xorBitArray[i])
                {
                    listHardSensorStatus[i].mPrevBitStatus = listHardSensorStatus[i].mThisBitStatus;
                    listHardSensorStatus[i].mThisBitStatus = listHardSensorStatus[i].mThisBitStatus ? false : true;
                    listHardSensorStatus[i].mChangedDateTime = DateTime.Now;
                }
            }

            // 前回値の更新(今回値に上書き)
            mPrevInputStatus = mThisInputStatus;
        }

        /// <summary>
        /// 単一点トリガのトリガ種類判定処理(ONエッジ、OFFエッジ)
        /// </summary>
        /// <param name="inputNo">入力接点</param>
        /// <param name="processIndex">工程インデックス</param>
        /// <param name="nowTime">現在時刻</param>
        /// <param name="innerSensorStatus">切り替える内部センサ状態(ONエッジ：true, OFFエッジ：false)</param>
        /// <param name="prevJudgeStatus">前回の状態の正判定値</param>
        /// <param name="thisJudgeStatus">今回の状態の正判定値</param>
        /// <param name="innerJudgeStatus">内部状態の正判定値</param>
        /// <returns></returns>
        private Boolean SingleEdgeChange(int inputNo, int processIndex, DateTime nowTime, TrigerType trigerType, Boolean prevJudgeStatus, Boolean thisJudgeStatus, Boolean innerJudgeStatus)
        {
            Boolean ret = false;
            Boolean innerSensorStatus = false;
            try
            {
                switch (trigerType)
                {
                    case TrigerType.ON_EDGE:
                        innerSensorStatus = true;
                        break;
                    case TrigerType.OFF_EDGE:
                        innerSensorStatus = false;
                        break;
                    default:
                        logger.Warn(String.Format("【WARN】指定外トリガタイプ受信。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        return false;
                }
                if (inputNo < 0)
                {
                    // 入力接点の取得に失敗しているため、続きをやらない
                    logger.Warn(String.Format("【WARN】入力接点の取得に失敗。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    return false;
                }

                // 実センサーの前回信号状態
                Boolean prevHardStatus = listHardSensorStatus[inputNo].mPrevBitStatus;
                // 実センサーの今回信号状態
                Boolean thisHardStatus = listHardSensorStatus[inputNo].mThisBitStatus;
                // 内部センサーの信号状態
                Boolean innerBitStatus = mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus;

                // 前回OFF && 今回ON && 内部センサOFF
                if (prevHardStatus.Equals(prevJudgeStatus) && thisHardStatus.Equals(thisJudgeStatus) && innerBitStatus.Equals(innerJudgeStatus))
                {
                    DateTime changedTime = listHardSensorStatus[inputNo].mChangedDateTime;
                    var tempTime = mWorkingStatusTbl.Rows[processIndex].Field<decimal?>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1);
                    double filterMiliSecond = tempTime != null ? (double)tempTime : int.MaxValue;

                    // (現在時刻 - ビット変化した時刻) > 連続信号排除(チャタリング防止)フィルタ時間
                    if ((nowTime - changedTime).TotalSeconds > filterMiliSecond)
                    {
                        logger.Info(String.Format("【SingleEdgeChange】工程インデックス：{0}, 受信トリガタイプ：{1}, チャタ防フィルタクリア 内部センサ：{2}, 入力点：{3} ", processIndex, trigerType, (innerSensorStatus ? "ON" : "OFF"), inputNo));
                        // 内部センサを指定された状態に切り替え、内部センサ変化時刻を現在時刻に更新する
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = innerSensorStatus;
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;

                        // トリガ発行許可の確認
                        if (listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission)
                        {
                            // CT開始トリガを発行し、トリガ発行時刻を更新してトリガ発行許可をfalseにする。
                            listTriggerIssuanceStatus[processIndex].mChangedDateTime = nowTime;
                            listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission = false;

                            logger.Info(String.Format("【SingleEdgeChange】CT更新イベント発行。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                            Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_DI, processIndex, nowTime));
                        }
                        else
                        {
                            logger.Info(String.Format("【SingleEdgeChange】トリガ認識時間外時刻。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        }
                    }
                    else
                    {
                        //logger.Info(String.Format("【SingleEdgeChange】連続信号排除フィルタ時間。工程インデックス：{0}, 受信トリガタイプ：{1},　検知時間(秒)：{2}", processIndex, trigerType, ((nowTime - changedTime).TotalSeconds)));
                    }
                }
                // 前回ON && 今回OFF && 内部センサON
                else if (prevHardStatus.Equals(!prevJudgeStatus) && thisHardStatus.Equals(!thisJudgeStatus) && innerBitStatus.Equals(!innerJudgeStatus))
                {
                    logger.Info(String.Format("【SingleEdgeChange】工程インデックス：{0}, 受信トリガタイプ：{1}, チャタ防フィルタクリア 内部センサ：{2}, 入力点：{3} ", processIndex, trigerType, (innerSensorStatus ? "OFF" : "ON"), inputNo));
                    // 内部センサを逆転させ、内部センサ変化時刻を現在時刻に更新する
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = !innerSensorStatus;
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;
                }
                else
                {
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
        /// 複数点トリガのトリガ種類判定処理(AND,OR)
        /// </summary>
        /// <param name="inputNo1">センサ1の接点番号</param>
        /// <param name="inputNo2">センサ2の接点番号</param>
        /// <param name="processIndex">工程インデックス</param>
        /// <param name="nowTime">現在時刻</param>
        /// <param name="trigerType"></param>
        /// <returns></returns>
        private Boolean MultiEdgeChange(int inputNo1, int inputNo2, int processIndex, DateTime nowTime, TrigerType trigerType)
        {
            Boolean ret = false;
            Boolean sensor1Status = false;
            Boolean sensor2Status = false;
            try
            {
                switch (trigerType)
                {
                    case TrigerType.AND:
                    case TrigerType.OR:
                        break;
                    default:
                        logger.Warn(String.Format("【WARN】指定外トリガタイプ受信。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        return false;
                }
                if (inputNo1 < 0)
                {
                    // 入力接点の取得に失敗しているため、続きをやらない
                    logger.Warn(String.Format("【WARN】入力接点1の取得に失敗。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    return false;
                }
                if (inputNo2 < 0)
                {
                    // 入力接点の取得に失敗しているため、続きをやらない
                    logger.Warn(String.Format("【WARN】入力接点2の取得に失敗。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    return false;
                }

                Boolean ret1 = MultiEdgeChangeIndividual(inputNo1, processIndex, nowTime, ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1, out sensor1Status);
                Boolean ret2 = MultiEdgeChangeIndividual(inputNo2, processIndex, nowTime, ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2, out sensor2Status);
                if ((ret1 && ret2))
                {
                    // センサ1の内部センサ状態
                    Boolean input1Status = mListInnerSensorStatusByProcessIndex[processIndex][inputNo1].mInnerBitStatus;
                    // センサ2の内部センサ状態
                    Boolean input2Status = mListInnerSensorStatusByProcessIndex[processIndex][inputNo2].mInnerBitStatus;
                    // 指定工程のトリガ発行許可状態
                    Boolean processTriggerIssuance = listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission;
                    // センサ1とセンサ2の同時確認ステータス
                    Boolean multiTriggerStatus = false;

                    switch (trigerType)
                    {
                        case TrigerType.AND:
                            multiTriggerStatus = (input1Status && input2Status);
                            break;
                        case TrigerType.OR:
                            multiTriggerStatus = (input1Status || input2Status);
                            break;
                        default:
                            // 指定外につき、falseとする
                            multiTriggerStatus = false;
                            break;
                    }
                    // トリガ発行許可の確認
                    if (sensor1Status || sensor2Status)
                    {
                        if (multiTriggerStatus && processTriggerIssuance)
                        {
                            // CT開始トリガを発行し、トリガ発行時刻を更新してトリガ発行許可をfalseにする。
                            listTriggerIssuanceStatus[processIndex].mChangedDateTime = nowTime;
                            listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission = false;

                            logger.Info(String.Format("【MultiEdgeChange】CT更新イベント発行。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                            //Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_DI));
                            Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_DI, processIndex, nowTime));

                        }
                        else if (multiTriggerStatus && !processTriggerIssuance)
                        {
                            logger.Info(String.Format("【MultiEdgeChange】トリガ認識時間外時刻。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        }
                    }
                    ret = true;
                }
                else
                {
                    if (!ret1)
                    {
                        // センサ1点目の接点情報更新異常
                        logger.Warn(String.Format("【MultiEdgeChange】入力接点1の接点情報更新異常。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    }
                    if (!ret2)
                    {
                        // センサ2点目の接点情報更新異常
                        logger.Warn(String.Format("【MultiEdgeChange】入力接点2の接点情報更新異常。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    }
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【MultiEdgeChange】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 複数点トリガのトリガ種類判定処理(ONメモリ)
        /// </summary>
        /// <param name="inputNo1">センサ1の接点番号</param>
        /// <param name="inputNo2">センサ2の接点番号</param>
        /// <param name="processIndex">工程インデックス</param>
        /// <param name="nowTime">現在時刻</param>
        /// <param name="trigerType"></param>
        /// <returns></returns>
        private Boolean MultiEdgeChangeOnMemory(int inputNo1, int inputNo2, int processIndex, DateTime nowTime, TrigerType trigerType)
        {
            Boolean ret = false;
            Boolean sensor1Status = false;
            Boolean sensor2Status = false;
            try
            {
                switch (trigerType)
                {
                    case TrigerType.ON_MEMORY:
                        break;
                    default:
                        logger.Warn(String.Format("【MultiEdgeChangeOnMemory】指定外トリガタイプ受信。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        return false;
                }
                if (inputNo1 < 0)
                {
                    // 入力接点の取得に失敗しているため、続きをやらない
                    logger.Error(String.Format("【MultiEdgeChangeOnMemory】入力接点1の取得に失敗。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    return false;
                }
                if (inputNo2 < 0)
                {
                    // 入力接点の取得に失敗しているため、続きをやらない
                    logger.Error(String.Format("【MultiEdgeChangeOnMemory】入力接点2の取得に失敗。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    return false;
                }

                Boolean ret1 = MultiEdgeChangeIndividualOnMemory(inputNo1, processIndex, nowTime, out sensor1Status);
                Boolean ret2 = MultiEdgeChangeIndividual(inputNo2, processIndex, nowTime, ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2, out sensor2Status);
                if (ret1 && ret2)
                {
                    // オンメモリの状態
                    Boolean onMemoryStatus = listOnMemoryStatus[processIndex];
                    // センサ2の内部センサ状態
                    Boolean input2Status = mListInnerSensorStatusByProcessIndex[processIndex][inputNo2].mInnerBitStatus;
                    // 指定工程のトリガ発行許可状態
                    Boolean processTriggerIssuance = listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission;
                    // センサ1とセンサ2の同時確認ステータス
                    Boolean multiTriggerStatus = false;

                    switch (trigerType)
                    {
                        case TrigerType.ON_MEMORY:
                            multiTriggerStatus = (onMemoryStatus && input2Status);
                            break;
                        default:
                            // 指定外につき、falseとする
                            multiTriggerStatus = false;
                            break;
                    }

                    // トリガ発行許可の確認
                    if (sensor1Status || sensor2Status)
                    {
                        if (multiTriggerStatus && processTriggerIssuance)
                        {
                            // CT開始トリガを発行し、トリガ発行時刻を更新してトリガ発行許可をfalseにする。またON当該工程のONメモリを下げる
                            // TODO: CT追加イベントの発行
                            listTriggerIssuanceStatus[processIndex].mChangedDateTime = nowTime;
                            listOnMemoryStatus[processIndex] = false;
                            listTriggerIssuanceStatus[processIndex].mTriggerIssuancePermission = false;

                            logger.Info(String.Format("【MultiEdgeChangeOnMemory】CT更新イベント発行。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                            //Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_DI));
                            Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_DI, processIndex, nowTime));
                        }
                        else if (!processTriggerIssuance)
                        {
                            logger.Info(String.Format("【MultiEdgeChangeOnMemory】トリガ認識時間外時刻。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                        }
                    }

                    ret = true;
                }
                else
                {
                    if (!ret1)
                    {
                        // センサ1点目の接点情報更新異常
                        logger.Warn(String.Format("【MultiEdgeChangeOnMemory】入力接点1の接点情報更新異常。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    }
                    if (!ret2)
                    {
                        // センサ2点目の接点情報更新異常
                        logger.Warn(String.Format("【MultiEdgeChangeOnMemory】入力接点2の接点情報更新異常。工程インデックス：{0}, 受信トリガタイプ：{1}", processIndex, trigerType));
                    }
                    ret = false;
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
        /// 複数点トリガのトリガ種類判定処理をするための各個点ごとチェック処理
        /// </summary>
        /// <param name="inputNo">入力接点</param>
        /// <param name="processIndex">工程インデックス</param>
        /// <param name="nowTime">現在時刻</param>
        /// <param name="columnNameExclusionFilter"></param>
        /// <returns></returns>
        private Boolean MultiEdgeChangeIndividual(int inputNo, int processIndex, DateTime nowTime, String columnNameExclusionFilter, out Boolean sensourStatus)
        {
            Boolean ret = false;
            sensourStatus = false;
            try
            {
                // 実センサーの前回信号状態
                Boolean prevHardStatus = listHardSensorStatus[inputNo].mPrevBitStatus;
                // 実センサーの今回信号状態
                Boolean thisHardStatus = listHardSensorStatus[inputNo].mThisBitStatus;
                // 内部センサーの信号状態
                Boolean innerBitStatus = mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus;
                // 前回OFF && 今回ON && 内部センサOFF
                if (!prevHardStatus && thisHardStatus && !innerBitStatus)
                {
                    DateTime changedTime = listHardSensorStatus[inputNo].mChangedDateTime;
                    var tempTime = mWorkingStatusTbl.Rows[processIndex].Field<decimal?>(columnNameExclusionFilter);
                    double filterMiliSecond = tempTime != null ? (double)tempTime : int.MaxValue;

                    // (現在時刻 - ビット変化した時刻) > 連続信号排除(チャタリング防止)フィルタ時間
                    if ((nowTime - changedTime).TotalSeconds > filterMiliSecond)
                    {
                        logger.Info(String.Format("【MultiEdgeChangeIndividual】工程インデックス：{0}, チャタ防フィルタクリア 内部センサON, 入力点：{1}", processIndex, inputNo));
                        // 内部センサをONに切り替え、内部センサ変化時刻を現在時刻に更新する
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = true;
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;
                        sensourStatus = true;
                    }
                }
                // 前回ON && 今回OFF && 内部センサON
                else if (prevHardStatus && !thisHardStatus && innerBitStatus)
                {
                    logger.Info(String.Format("【MultiEdgeChangeIndividual】工程インデックス：{0}, チャタ防フィルタクリア 内部センサOFF, 入力点：{1}", processIndex, inputNo));
                    // 内部センサをOFFに切り替え、内部センサ変化時刻を現在時刻に更新する
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = false;
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;
                }

                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【MultiEdgeChangeIndividual】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// オンメモリのトリガ種類判定するためのセンサ1チェック処理
        /// </summary>
        /// <param name="inputNo"></param>
        /// <param name="processIndex"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        private Boolean MultiEdgeChangeIndividualOnMemory(int inputNo, int processIndex, DateTime nowTime, out Boolean sensourStatus)
        {
            Boolean ret = false;
            sensourStatus = false;
            try
            {
                // 実センサーの前回信号状態
                Boolean prevHardStatus = listHardSensorStatus[inputNo].mPrevBitStatus;
                // 実センサーの今回信号状態
                Boolean thisHardStatus = listHardSensorStatus[inputNo].mThisBitStatus;
                // 内部センサーの信号状態
                Boolean innerBitStatus = mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus;
                // 前回OFF && 今回ON && 内部センサOFF
                if (!prevHardStatus && thisHardStatus && !innerBitStatus)
                {
                    DateTime changedTime = listHardSensorStatus[inputNo].mChangedDateTime;
                    var tempTime = mWorkingStatusTbl.Rows[processIndex].Field<decimal?>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1);
                    double filterMiliSecond = tempTime != null ? (double)tempTime : int.MaxValue;

                    // (現在時刻 - ビット変化した時刻) > 連続信号排除(チャタリング防止)フィルタ時間
                    if ((nowTime - changedTime).TotalSeconds > filterMiliSecond)
                    {
                        logger.Info(String.Format("【MultiEdgeChangeIndividualOnMemory】工程インデックス：{0}, チャタ防フィルタクリア　内部センサ&オンメモリON, 入力点：{1}", processIndex, inputNo));
                        // 内部センサと対象工程のオンメモリをONに切り替え、内部センサ変化時刻を現在時刻に更新する
                        listOnMemoryStatus[processIndex] = true;
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = true;
                        mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;
                        sensourStatus = true;
                    }
                }
                // 前回ON && 今回OFF && 内部センサON
                else if (prevHardStatus && !thisHardStatus && innerBitStatus)
                {
                    logger.Info(String.Format("【MultiEdgeChangeIndividualOnMemory】工程インデックス：{0}, 内部センサ OFF, 入力点：{1}", processIndex, inputNo));
                    // 内部センサをOFFに切り替え、内部センサ変化時刻を現在時刻に更新する
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mInnerBitStatus = false;
                    mListInnerSensorStatusByProcessIndex[processIndex][inputNo].mChangedDateTime = nowTime;
                }

                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【MultiEdgeChangeIndividualOnMemory】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }


        #endregion
    }

    /// <summary>
    /// DIOコントローラ通信スレッドコマンドクラス
    /// </summary>
    public class CmdDioCtrl
    {
        public DioCtrlThreadStatus mStatus;
        public ushort mInputStatus = 0;
        /// <summary> 直開始・終了時刻 </summary>
        public CmdDioCtrlShiftTime mShiftDateTime;
        public int mBuzzerBit = 0;
        public List<int> mBuzzerBitList = new List<int>();
        public BuzzerRingStatus mBuzzerStatus;


        public CmdDioCtrl(DioCtrlThreadStatus status)
        {
            mStatus = status;
        }
        /// <summary>
        /// DIO受信時の処理クラスコンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="inputStatus"></param>
        public CmdDioCtrl(DioCtrlThreadStatus status, ushort inputStatus)
        {
            mStatus = status;
            mInputStatus = inputStatus;
        }
        /// <summary>
        /// QR読み込み時、単一ブザー発報処理クラスコンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="buzzerBit"></param>
        /// <param name="buzzerStatus"></param>
        public CmdDioCtrl(DioCtrlThreadStatus status, int buzzerBit, BuzzerRingStatus buzzerStatus)
        {
            mStatus = status;
            mBuzzerBit = buzzerBit;
            mBuzzerStatus = buzzerStatus;
        }
        /// <summary>
        /// QR読み込み時、複数ブザー発報処理クラスコンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="buzzerBit"></param>
        /// <param name="buzzerStatus"></param>
        public CmdDioCtrl(DioCtrlThreadStatus status, List<int> buzzerBitList, BuzzerRingStatus buzzerStatus)
        {
            mStatus = status;
            mBuzzerBitList = buzzerBitList;
            mBuzzerStatus = buzzerStatus;
        }
        /// <summary>
        /// 直開始通知受信時の処理コンストラクタ
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftTime"></param>
        public CmdDioCtrl(DioCtrlThreadStatus status, CmdDioCtrlShiftTime shiftTime)
        {
            mStatus = status;
            mShiftDateTime = shiftTime;
        }
    }

    /// <summary>
    /// 直開始・終了時刻情報クラス
    /// </summary>
    public class CmdDioCtrlShiftTime
    {
        /// <summary> 直開始時刻 </summary>
        public DateTime mShiftStartDateTime;
        /// <summary> 直終了時刻 </summary>
        public DateTime mShiftEndDateTime;
        public CmdDioCtrlShiftTime(DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            mShiftStartDateTime = shiftStartDateTime;
            mShiftEndDateTime = shiftEndDateTime;
        }
    }

    public enum BuzzerRingStatus
    {
        /// <summary> 全ブザー対象 </summary>
        ALL,
        /// <summary> 各個ブザー対象 </summary>
        INDIVIDUAL,
    }
}
