using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using CdioCs;
using log4net;
using System.Threading;

namespace PLOSDeviceConnectionManager.Thread
{
    public class DioThread : WorkerThread
    {

        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary> DIOの0ポート目 </summary>
        private const short PORT_0 = 0;
        /// <summary> DIOの1ポート目 </summary>
        private const short PORT_1 = 1;

        /// <summary> DIO通信クラスのインスタンス </summary>
        private Cdio mCdio = new Cdio();
        /// <summary> DIO制御スレッドクラスのインスタンス </summary>
        private DioCtrlThread mDioCtrlThread;
        /// <summary> DIOデバイス名 </summary>
        private String mDeviceName = "";
        /// <summary> DIOデバイスID </summary>
        private short mDeviceId = 0;

        #endregion

        #region <Property>

        public static Boolean DioStatus { get; private set; }
        #endregion

        #region <Constructor>

        public DioThread(int cycleTime, String deviceName, DioCtrlThread dioCtrlThread)
        {
            this.CycleTime = cycleTime;
            mDeviceName = deviceName;
            mDioCtrlThread = dioCtrlThread;
            pr = ThreadPriority.Highest;

        }

        #endregion

        #region <Method>

        #region <OverRide>
        /// <summary>
        /// 定周期処理
        /// </summary>
        protected override void Proc()
        {
            if (GetInputBitStatus())
            {
                DioStatus = true;
            }
            else
            {
                logger.Warn(String.Format("【Proc】DIOデバイスエラー検知。終了→初期化します"));
                DioStatus = false;
                bool exit = ExitDevice();
                bool init = InitDevice();
                if(exit && init)
                {
                    DioStatus = true;
                    logger.Debug(String.Format("【Proc】DIOデバイス再接続完了"));
                }
            }
            base.Proc();
        }
        /// <summary>
        /// キューイベント処理
        /// </summary>
        /// <param name="obj">イベントキュー</param>
        protected override void QueueEvent(object obj)
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is CmdDioCtrl)
            {

            }
        }

        #endregion

        /// <summary>
        /// デバイスの初期化
        /// </summary>
        /// <returns>初期化成功の是非</returns>
        public Boolean InitDevice()
        {
            Boolean ret = false;
            try
            {
                int dioInitRet = mCdio.Init(mDeviceName, out mDeviceId);
                if (dioInitRet > 0)
                {
                    // エラー発生。ログ出力メッセージの取得
                    string errMsg;
                    mCdio.GetErrorString(dioInitRet, out errMsg);
                    logger.Warn(String.Format("【InitDevice】DIOエラー：{0}：{1}", dioInitRet, errMsg));
                    ret = false;
                }
                else
                {
                    // 初期化成功
                    ret = true;
                }
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
        /// デバイスの終了
        /// </summary>
        /// <returns>終了処理成功の是非</returns>
        public Boolean ExitDevice()
        {
            Boolean ret = false;
            try
            {
                int dioInitRet = mCdio.Exit(mDeviceId);
                if (dioInitRet > 0)
                {
                    // エラー発生。ログ出力メッセージの取得
                    string errMsg;
                    mCdio.GetErrorString(dioInitRet, out errMsg);
                    logger.Warn(String.Format("【ExitDevice】DIOエラー：{0}：{1}", dioInitRet, errMsg));
                    ret = false;
                }
                else
                {
                    // 初期化成功
                    ret = true;
                }
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
        /// 入力ビットの全点情報を取得して数値化したのち、DIO制御スレッドへイベント通知
        /// </summary>
        /// <param name="allBitNum"></param>
        /// <returns></returns>
        private Boolean GetInputBitStatus()
        {
            Boolean ret = false;
            byte portByte0 = 0;
            byte portByte1 = 0;
            int allBitNum = 0;

            try
            {
                Boolean getBitStatusPort0 = GetInputBitStatusByPort(PORT_0, out portByte0);
                Boolean getBitStatusPort1 = GetInputBitStatusByPort(PORT_1, out portByte1);

                if (!getBitStatusPort0 || !getBitStatusPort1)
                {
                    ret = false;
                }
                else
                {
                    int byteLeftShift = portByte1 << 8;
                    allBitNum = byteLeftShift + portByte0;

                    if (ushort.MaxValue < allBitNum)
                    {
                        // ERROR
                        logger.Error(String.Format("【GetInputBitStatus】DIOデータエラー。取得値：{0}, 判定地：{1}", allBitNum, ushort.MaxValue));
                        ret = false;
                    }
                    else
                    {
                        mDioCtrlThread.AddEvent(new CmdDioCtrl(DioCtrlThreadStatus.STATUS_RECV, (ushort)allBitNum));
                        ret = true;
                    }

                }


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
        /// 指定のポートのポート状態を取得する
        /// </summary>
        /// <param name="portNum">ポート番号</param>
        /// <param name="portByte">取得されたBit状態を保持したbyte値</param>
        /// <returns></returns>
        private Boolean GetInputBitStatusByPort(short portNum, out byte portByte)
        {
            Boolean ret = false;
            portByte = 0;

            try
            {
                int dioGetInputByteRet = mCdio.InpByte(mDeviceId, portNum, out portByte);
                if (dioGetInputByteRet > 0)
                {
                    // エラー発生。ログ出力メッセージの取得
                    string errMsg;
                    mCdio.GetErrorString(dioGetInputByteRet, out errMsg);
                    logger.Warn(String.Format("【GetInputBitStatusByPort】DIOエラー：{0}：{1}", dioGetInputByteRet, errMsg));
                    ret = false;
                }
                else
                {
                    ret = true;
                }
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
        /// 指定したビット番号の位置に指定したON,OFFを渡す
        /// </summary>
        /// <param name="BitNo"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public Boolean OutputBitStatusByBitNo(short BitNo, byte Data)
        {
            Boolean ret = false;
            try
            {
                int dioOutputBitStatusByPortRet = mCdio.OutBit(mDeviceId, BitNo, Data);
                if (dioOutputBitStatusByPortRet > 0)
                {
                    // エラー発生。ログ出力メッセージの取得
                    string errMsg;
                    mCdio.GetErrorString(dioOutputBitStatusByPortRet, out errMsg);
                    logger.Warn(String.Format("【OutputBitStatusByBitNo】DIOエラー：{0}：{1}", dioOutputBitStatusByPortRet, errMsg));
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        #endregion


    }

    /// <summary>
    /// DIOコントローラ通信スレッドコマンドクラス
    /// </summary>
    public class CmdDio
    {

    }
}
