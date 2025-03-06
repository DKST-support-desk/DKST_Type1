using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using log4net;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum MicCtrlThreadStatus
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
    /// マイク通信管理スレッドクラス
    /// </summary>
    public class MicCtrlThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary> 音声サンプリングレート </summary>
        private const int SAMPLING_RATE = 48000;
        /// <summary> 自スレッド番号 </summary>
        private int mThreadIndex = -1;
        /// <summary> デバイス番号 </summary>
        private int mDeviceNo = -1;

        /// <summary> 録音機器デバイスID </summary>
        private String mMicDeviceId = "";

        /// <summary> 自スレッド可動是非 </summary>
        private Boolean mThreadAlive = false;

        /// <summary> 自スレッドに登録されている録音機器が存在するか否か </summary>
        private Boolean mIsAudioDeviceRegisted = false;

        /// <summary> 録音状態にあるか否か </summary>
        //private List<Boolean> mListIsFileWriteStop = new List<Boolean>();

        /// <summary> 工程ごとに保持する録音機器の使用設定(0:不使用,1:使用,2:異常時のみ)。リスト数は工程数と一致する </summary>
        private List<int> mListCameraUsingStatusByProcess = new List<int>();

        /// <summary> 音声取得イベント設定 </summary>
        private WaveInEvent mWaveIn;
        /// <summary> 音声データストリーム </summary>
        //private WaveFileWriter mWaveWriter;
        /// <summary> 音声デバイス一覧 </summary>
        private MMDeviceCollection mDevices;
        /// <summary> 工程ごとに保持する音声ファイルストリーム </summary>
        private Dictionary<int, WaveFileWriter> mDicAudioWriteStreamOnProcess = new Dictionary<int, WaveFileWriter>();
        /// <summary> 工程ごとに保持する録音開始時刻 </summary>
        private Dictionary<int, DateTime> mDicCycleDatetimeOnProcess = new Dictionary<int, DateTime>();
        private DateTime mCycleTimeNoDevice;
        private Boolean isStopWrite = false;

        public class FileWriteStatus
        {
            /// <summary> 録音の停止依頼 </summary>
            public Boolean stmStopOrder;
            /// <summary> 録音の停止指示(EndReadメソッドから発行) </summary>
            public Boolean stmIsStopWrite;

            public FileWriteStatus(Boolean stopOrder, Boolean stopWrite)
            {
                stmStopOrder = stopOrder;
                stmIsStopWrite = stopWrite;
            }
        }

        private delegate void EndCheckDelegate(WaveFileWriter vw, int prcessIdx, DateTime cycleStartDate, DateTime cycleEndDate, int camUseStatus);

        #endregion

        #region <Property>
        #endregion

        #region <Constructor>

        public MicCtrlThread(int cycleTime, int threadIndex)
        {
            this.CycleTime = cycleTime;
            this.mThreadIndex = threadIndex;
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
                else if (obj is CmdMicCtrl)
                {
                    switch (((CmdMicCtrl)obj).mMicStatus)
                    {
                        case MicCtrlThreadStatus.INIT:

                            mMicDeviceId = ((CmdMicCtrl)obj).mMicId;
                            // 録音機器のデバイスID有無により、この先の録音設定を変える
                            if (String.IsNullOrEmpty(mMicDeviceId))
                            {
                                mIsAudioDeviceRegisted = false;
                            }
                            else
                            {
                                // 録音機器の情報を取得し、メンバ保持する
                                if (GetAllAudioDevice(mMicDeviceId, out mIsAudioDeviceRegisted))
                                {
                                    //InitAllAudioEvent();
                                }
                                else
                                {
                                    logger.Debug(String.Format("【INIT】マイク情報取得失敗。スレッドインデックス：{0}, マイクデバイスID：{1}", mThreadIndex, mMicDeviceId));
                                }
                            }
                            mThreadAlive = true;

                            break;
                        case MicCtrlThreadStatus.TERM:
                            break;
                        case MicCtrlThreadStatus.SHIFT_START:
                            if (mThreadAlive)
                            {
                                mListCameraUsingStatusByProcess = ((CmdMicCtrl)obj).mCamSaveConfig;
                                if (mIsAudioDeviceRegisted)
                                {

                                    mWaveIn = InitAudioDevice();
                                    // 録音ファイルストリームの初期化を行う
                                    if (InitAllAudioFileStream(((CmdMicCtrl)obj).mCycleDateTime))
                                    {
                                        mWaveIn.StartRecording();
                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("【SHIFT_START】このスレッドが管理するマイクの初期化失敗。スレッドインデックス：{0}, マイクデバイスID：{1}", mThreadIndex, mMicDeviceId));
                                    }
                                }
                                else
                                {
                                    // TODO: 本スレッドが管理担当するオーディオデバイスが存在しない
                                    mDicCycleDatetimeOnProcess.Clear();
                                    for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                                    {
                                        mDicCycleDatetimeOnProcess.Add(i, ((CmdMicCtrl)obj).mCycleDateTime);
                                    }

                                    mCycleTimeNoDevice = ((CmdMicCtrl)obj).mCycleDateTime;
                                }
                            }
                            break;
                        case MicCtrlThreadStatus.SHIFT_END:
                            if (mThreadAlive)
                            {
                                if (mWaveIn != null)
                                {
                                    mWaveIn.StopRecording();
                                }
                                if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                                {
                                    for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                                    {
                                        int camUseStatus = mListCameraUsingStatusByProcess[i];
                                        // 録音設定の有無チェック
                                        if (mIsAudioDeviceRegisted)
                                        {
                                            if (mListCameraUsingStatusByProcess[i] > 0)
                                            {
                                                DateTime start = mDicCycleDatetimeOnProcess[i];
                                                DateTime nextStart = ((CmdMicCtrl)obj).mCycleDateTime;

                                                EndCheckDelegate dlgt = new EndCheckDelegate(this.DelegateEndMethod);
                                                IAsyncResult asyncResult = dlgt.BeginInvoke(mDicAudioWriteStreamOnProcess[i], i, start, nextStart, camUseStatus, null, null);
                                                dlgt.EndInvoke(asyncResult);
                                            }
                                        }
                                        else
                                        {
                                            if (mListCameraUsingStatusByProcess[i] > 0)
                                            {
                                                logger.Debug(String.Format("【SHIFT_END】音声なしイベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, サイクル開始時刻：{2}", mThreadIndex, i, mDicCycleDatetimeOnProcess[i].ToString("HH:mm.ss.fff")));
                                                CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(mDicCycleDatetimeOnProcess[i], ((CmdMicCtrl)obj).mCycleDateTime);
                                                //Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_AUDIO_SAVE, Program.NO_MIC_USE, SaveFileType.AUDIO, mThreadIndex, i, mCycleTimeNoDevice));
                                                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_AUDIO_SAVE, Program.NO_MIC_USE, SaveFileType.AUDIO, mThreadIndex, i, cycleTime, 0, camUseStatus));
                                            }
                                        }
                                    }
                                }
                                if (mWaveIn != null)
                                {
                                    mWaveIn.Dispose();
                                }
                                isStopWrite = false;

                            }
                            mThreadAlive = false;
                            break;
                        case MicCtrlThreadStatus.CYCLE:
                            if (mThreadAlive)
                            {
                                int processIdx = ((CmdMicCtrl)obj).mProcessIdx;
                                if(processIdx < 0)
                                {
                                    logger.Debug(String.Format("【CYCLE】 工程インデックスが初期化されていません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                }
                                else
                                {
                                    if (mListCameraUsingStatusByProcess != null && mListCameraUsingStatusByProcess.Count > 0)
                                    {
                                        int camUseStatus = mListCameraUsingStatusByProcess[processIdx];
                                        if (mIsAudioDeviceRegisted)
                                        {
                                            if (camUseStatus > 0)
                                            {
                                                logger.Debug(String.Format("【CYCLE】停止処理発行。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                                mWaveIn.StopRecording();

                                                DateTime start = mDicCycleDatetimeOnProcess[processIdx];
                                                DateTime nextStart = ((CmdMicCtrl)obj).mCycleDateTime;
                                                logger.Debug(String.Format("【CYCLE】停止処理完了。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));

                                                DelegateEndMethod(mDicAudioWriteStreamOnProcess[processIdx], processIdx, start, nextStart, camUseStatus);

                                                InitAudioFileStream(nextStart, processIdx);
                                                mWaveIn.StartRecording();
                                                mDicCycleDatetimeOnProcess[processIdx] = ((CmdMicCtrl)obj).mCycleDateTime;
                                                isStopWrite = false;
                                            }
                                        }
                                        else
                                        {
                                            if (camUseStatus > 0)
                                            {
                                                DateTime start = mDicCycleDatetimeOnProcess[processIdx];
                                                //DateTime start = mCycleTimeNoDevice;
                                                DateTime nextStart = ((CmdMicCtrl)obj).mCycleDateTime;

                                                //logger.Debug(String.Format("【CYCLE】 録音なし設定。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, mCycleTimeNoDevice.ToString("HH:mm.ss.fff")));
                                                logger.Debug(String.Format("【CYCLE】 録音なし設定。スレッドインデックス：{0}, 工程インデックス：{1}, 次サイクル開始時刻：{2}", mThreadIndex, processIdx, nextStart.ToString("HH:mm.ss.fff")));

                                                CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(start, nextStart);
                                                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_AUDIO_SAVE, Program.NO_MIC_USE, SaveFileType.AUDIO, mThreadIndex, processIdx, cycleTime, 0, camUseStatus));
                                                //mCycleTimeNoDevice = ((CmdMicCtrl)obj).mCycleDateTime;
                                                mDicCycleDatetimeOnProcess[processIdx] = ((CmdMicCtrl)obj).mCycleDateTime;
                                            }
                                            else
                                            {
                                                logger.Debug(String.Format("【CYCLE】 工程に使用するマイクが初期化されていません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("【CYCLE】 工程に使用するマイク設定がありません。スレッドインデックス：{0}, 工程インデックス：{1}", mThreadIndex, processIdx));
                                    }
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
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// オーディオ機器情報の取得
        /// </summary>
        /// <param name="micId"></param>
        /// <returns></returns>
        private Boolean GetAllAudioDevice(String micId, out Boolean existMic)
        {
            Boolean ret = false;
            existMic = false;
            try
            {
                int allDeviceCount = WaveInEvent.DeviceCount;

                // 接続しているオーディオ情報の全件取得
                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                mDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                if (micId != null && micId != String.Empty)
                {
                    for (int i = 0; i < allDeviceCount; i++)
                    {
                        String deviceIdOnMasterUpper = micId.ToUpper();
                        String deviceIdUpper = mDevices[i].ID.ToUpper();

                        if (deviceIdOnMasterUpper.Contains(deviceIdUpper))
                        {
                            mDeviceNo = i;
                            break;
                        }
                    }
                    if (mDeviceNo < 0)
                    {
                        existMic = false;
                    }
                    else
                    {
                        existMic = true;
                    }
                }
                else
                {
                    if (micId == null)
                    {
                        logger.Error(String.Format("【ERROR】指定した音声デバイスIDがNULLです"));
                        return false;
                    }
                    else if (micId == String.Empty)
                    {
                        logger.Error(String.Format("【ERROR】指定した音声デバイスIDが空文字です"));
                    }
                    existMic = false;
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

        ///// <summary>
        ///// このスレッドで管理する全音声取得イベントの初期化を行う
        ///// </summary>
        ///// <returns></returns>
        //private Boolean InitAllAudioEvent(DateTime cycleStartTime)
        //{
        //    Boolean ret = false;
        //    try
        //    {
        //        mDicCycleDatetimeOnProcess.Clear();
        //        for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
        //        {
        //            if (mListCameraUsingStatusByProcess[i] > 0)
        //            {
        //                WaveInEvent waveIn = InitAudioDevice();
        //            }
        //            mDicCycleDatetimeOnProcess.Add(i, cycleStartTime);
        //        }
        //        ret = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
        //        ret = false;
        //    }
        //    return ret;
        //}

        ///// <summary>
        ///// このスレッドで管理する特定工程の音声取得イベントの初期化を行う
        ///// </summary>
        ///// <param name="fileDateTime">サイクルの開始時刻</param>
        ///// <param name="processIdx">対象の工程インデックス</param>
        ///// <returns></returns>
        //private Boolean InitAudioEventByProcess(int processIdx)
        //{
        //    Boolean ret = false;
        //    try
        //    {
        //        if (mListCameraUsingStatusByProcess[processIdx] > 0)
        //        {
        //            WaveInEvent waveIn = InitAudioDevice();
        //        }
        //        ret = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
        //        ret = false;
        //    }
        //    return ret;
        //}

        /// <summary>
        /// オーディオ機器情報の初期化
        /// </summary>
        /// <returns></returns>
        private WaveInEvent InitAudioDevice()
        {
            WaveInEvent waveInEvent;
            try
            {
                waveInEvent = new WaveInEvent();
                waveInEvent.DeviceNumber = mDeviceNo;
                waveInEvent.WaveFormat = new WaveFormat(SAMPLING_RATE, WaveIn.GetCapabilities(mDeviceNo).Channels);
                waveInEvent.DataAvailable += WriteAudioEvent;
                waveInEvent.RecordingStopped += RecordingStopped;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                waveInEvent = null;
            }
            return waveInEvent;
        }

        /// <summary>
        /// このスレッドで管理する全ファイルストリームの初期化を行う
        /// </summary>
        /// <param name="fileDateTime">サイクルの開始時刻</param>
        /// <returns></returns>
        private Boolean InitAllAudioFileStream(DateTime fileDateTime)
        {
            Boolean ret = false;
            try
            {
                mDicAudioWriteStreamOnProcess.Clear();
                mDicCycleDatetimeOnProcess.Clear();
                for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                {

                    if (mListCameraUsingStatusByProcess[i] > 0)
                    {
                        WaveFileWriter wfwRet = InitAudioFileStreamProcess(fileDateTime, i);

                        mDicAudioWriteStreamOnProcess.Add(i, wfwRet);
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
        private Boolean InitAudioFileStream(DateTime fileDateTime, int processIdx)
        {
            Boolean ret = false;
            try
            {
                if (mListCameraUsingStatusByProcess[processIdx] > 0)
                {
                    WaveFileWriter wfwRet = InitAudioFileStreamProcess(fileDateTime, processIdx);
                    mDicAudioWriteStreamOnProcess[processIdx] = wfwRet;
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
        /// オーディオファイルストリームの初期化を行う
        /// </summary>
        /// <param name="fileDateTime"></param>
        /// <param name="processidx"></param>
        private WaveFileWriter InitAudioFileStreamProcess(DateTime fileDateTime, int processidx)
        {
            WaveFileWriter wfwRet = null;
            try
            {
                String audioNameTemp = String.Format(Program.TEMP_AUDIO_FILE_NAME_FORMAT, fileDateTime.ToString("yyyyMMdd_HHmmss"), processidx + 1, mThreadIndex + 1);
                String tempDir = Program.AppSetting.WebcamParam.TempFilePath;
                String audioName = System.IO.Path.Combine(tempDir, audioNameTemp);
                wfwRet = new WaveFileWriter(audioName, mWaveIn.WaveFormat);
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                wfwRet = null;
            }
            return wfwRet;
        }

        /// <summary>
        /// 録音停止イベントのイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordingStopped(object sender, StoppedEventArgs e)
        {
            try
            {
                logger.Debug(String.Format("【RecordingStopped】録音停止イベント受信"));
                isStopWrite = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// オーディオ取得時、ファイル書込みイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WriteAudioEvent(object sender, WaveInEventArgs args)
        {
            try
            {
                for (int i = 0; i < mListCameraUsingStatusByProcess.Count; i++)
                {
                    //この録音機器を使用する対象工程か否かを取り、対象工程ならば書き込みをする
                    if (mListCameraUsingStatusByProcess[i] > 0)
                    {
                        if (mWaveIn != null)
                        {
                            mDicAudioWriteStreamOnProcess[i].Write(args.Buffer, 0, args.BytesRecorded);
                            mDicAudioWriteStreamOnProcess[i].Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        private void DelegateEndMethod(WaveFileWriter wfw, int processIdx, DateTime cycleStartTime, DateTime cycleEndTime, int saveStatus)
        {
            try
            {
                String fileName = wfw.Filename;
                logger.Debug(String.Format("【DelegateEndMethod】音声ファイル保存。スレッドインデックス：{0}, 工程インデックス：{1}, ファイル名：{2}", mThreadIndex, processIdx, fileName));
                DateTime tempStart = DateTime.Now;
                while (true)
                {
                    //if (mDicIsFileWriteStopOnProcess[processIdx].stmIsStopWrite)
                    if (isStopWrite)
                    {
                        wfw.Flush();
                        wfw.Close();
                        wfw.Dispose();
                        wfw = null;

                        // 現在取り込んでいるビデオストリームは出力イベントを発行して書き込みさせる
                        logger.Debug(String.Format("【DelegateEndMethod】音声ファイル保存完了。イベント発行。スレッドインデックス：{0}, 工程インデックス：{1}, ファイル名：{2}", mThreadIndex, processIdx, fileName));
                        CycleStartEndDateTime cycleTime = new CycleStartEndDateTime(cycleStartTime, cycleEndTime);
                        Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_AUDIO_SAVE, fileName, SaveFileType.AUDIO, mThreadIndex, processIdx, cycleTime, 0, saveStatus));

                        break;
                    }
                    if(DateTime.Now.Ticks - tempStart.Ticks > TimeSpan.TicksPerSecond * 5)
                    {
                        logger.Debug(String.Format("【DelegateEndMethod】音声ファイル強制保存。スレッドインデックス：{0}, 工程インデックス：{1}, ファイル名：{2}", mThreadIndex, processIdx, fileName));
                        isStopWrite = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        #endregion

    }

    /// <summary>
    /// マイク通信スレッドコマンドクラス
    /// </summary>
    public class CmdMicCtrl
    {
        /// <summary> スレッドステータス </summary>
        public MicCtrlThreadStatus mMicStatus;
        /// <summary> 録音機器のデバイスID </summary>
        public String mMicId;
        /// <summary> 自スレッドの可動是非 </summary>
        public Boolean mAliveStatus;
        /// <summary> サイクル開始時刻 </summary>
        public DateTime mCycleDateTime;
        /// <summary> 各工程のカメラ使用設定(標準値マスタ) </summary>
        public List<int> mCamSaveConfig;
        /// <summary> 工程インデックス </summary>
        public int mProcessIdx;


        /// <summary> デフォルトのコンストラクタ </summary>
        public CmdMicCtrl(MicCtrlThreadStatus status)
        {
            mMicStatus = status;
        }

        /// <summary> INIT時のイベントコンストラクタ </summary>
        public CmdMicCtrl(MicCtrlThreadStatus status, String micId, Boolean alive)
        {
            mMicStatus = status;
            mMicId = micId;
            mAliveStatus = alive;
        }
        /// <summary> SHIFT_START時のイベントコンストラクタ </summary>
        public CmdMicCtrl(MicCtrlThreadStatus status, DateTime shiftStartTime, List<int> camSaveConfig)
        {
            mMicStatus = status;
            mCycleDateTime = shiftStartTime;
            mCamSaveConfig = camSaveConfig;
        }
        /// <summary> CYCLE時のイベントコンストラクタ </summary>
        public CmdMicCtrl(MicCtrlThreadStatus status, DateTime cycleStartTime, int processIdx)
        {
            mMicStatus = status;
            mCycleDateTime = cycleStartTime;
            mProcessIdx = processIdx;
        }
        /// <summary> SHIFT_END時のイベントコンストラクタ </summary>
        public CmdMicCtrl(MicCtrlThreadStatus status, DateTime cycleEndtTime)
        {
            mMicStatus = status;
            mCycleDateTime = cycleEndtTime;
        }

    }
}
