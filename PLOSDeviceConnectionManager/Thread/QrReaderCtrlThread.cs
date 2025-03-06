using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using System.IO.Ports;
using System.Data;
using DBConnect.SQL;
using log4net;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum QrReaderCtrlThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> 終了 </summary>
        RETRY,
    }
    /// <summary>
    /// QRリーダ通信管理スレッドクラス
    /// </summary>
    class QrReaderCtrlThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        int mThreadIndex = -1;
        private SerialPort mSerialPort;
        private DataRow mDataRow;
        private Boolean mIsAliveQrReader = false;
        #endregion

        #region <Property>
        public Boolean IsAliveQrReader { get { return this.mIsAliveQrReader; } }
        #endregion

        #region <Constructor>

        public QrReaderCtrlThread(int cycleTime, int threadIndex)
        {
            this.CycleTime = cycleTime;
            this.mThreadIndex = threadIndex;
            mSerialPort = new SerialPort();
            // 読み取り許可フラグ
            mSerialPort.DtrEnable = true;
            // SHIFT_JISでの読み取り設定
            mSerialPort.Encoding = System.Text.Encoding.GetEncoding(932);
            // 読み取り末尾の設定
            mSerialPort.NewLine = Environment.NewLine;
            mSerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
        }

        #endregion

        #region <Method>

        /// <summary>
        /// 定周期処理
        /// </summary>
        protected override void Proc()
        {
            CheckQrAlive();
            base.Proc();
        }

        /// <summary>
        /// イベント受信処理
        /// </summary>
        protected override void QueueEvent(object obj)
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is CmdQrReaderCtrl)
            {
                switch (((CmdQrReaderCtrl)obj).mStatus)
                {
                    case QrReaderCtrlThreadStatus.INIT:
                        mDataRow = ((CmdQrReaderCtrl)obj).mQrDataRow;
                        if (!InitQrReader())
                        {
                            AddEvent(new CmdQrReaderCtrl(QrReaderCtrlThreadStatus.RETRY));
                        }
                        break;
                    case QrReaderCtrlThreadStatus.RETRY:
                        if (!InitQrReader())
                        {
                            AddEvent(new CmdQrReaderCtrl(QrReaderCtrlThreadStatus.RETRY));
                        }
                        break;
                    case QrReaderCtrlThreadStatus.TERM:
                        TermQrReader();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// QRリーダの接続する
        /// </summary>
        /// <returns></returns>
        private Boolean InitQrReader()
        {
            Boolean ret = false;
            try
            {

                int portNo = mDataRow.Field<int>(ColumnReaderDeviceMst.PORT_NO);
                mSerialPort.PortName = String.Format("COM{0}", portNo);
                if (CheckQrAlive() && mIsAliveQrReader)
                {

                    mSerialPort.Open();
                    System.Threading.Thread.Sleep(1000);
                    ret = mSerialPort.IsOpen;
                    if (ret)
                    {
                        logger.Debug(String.Format("QRリーダ接続完了。ポート名：{0}, ボーレート：{1}, データビット長：{2}, パリティチェック：{3}, ストップビット：{4}, ハンドシェイク設定：{5}, DTR設定：{6}",
                            mSerialPort.PortName, mSerialPort.BaudRate, mSerialPort.DataBits, mSerialPort.Parity, mSerialPort.StopBits, mSerialPort.Handshake, mSerialPort.DtrEnable));
                    }
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
        /// QRリーダを切断する
        /// </summary>
        private void TermQrReader()
        {
            mSerialPort.Close();
        }

        /// <summary>
        /// QRリーダで読みトリガ発生したときのイベントメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int data1 = -1;
            int data2 = -1;
            String data3 = "";
            String readLine = mSerialPort.ReadLine();
            logger.Debug(String.Format("QRリーダCOM番号：{0}, 読み取りデータ：{1}", mSerialPort.PortName, readLine));

            if (CheckAnnotationFormat(readLine, out data1, out data2, out data3))
            {
                CmdEventQrReadParam param = new CmdEventQrReadParam(data1, data2, data3);
                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.READ_QR, param));
            }

        }

        /// <summary>
        /// QRコードフォーマットチェックを行う
        /// </summary>
        /// <param name="readLine">QRで読み込んだ情報</param>
        /// <param name="data1">データ識別子</param>
        /// <param name="data2">工程インデックス</param>
        /// <param name="data3">アノテーション情報</param>
        /// <returns></returns>
        private Boolean CheckAnnotationFormat(String readLine, out int data1, out int data2, out String data3)
        {
            Boolean ret = false;
            data1 = -1;
            data2 = -1;
            data3 = "";
            try
            {
                String[] split = readLine.Split(',');
                if (split.Length != 3)
                {
                    // QRのフォーマット異常
                    logger.Warn(String.Format("QR読み取り伝文異常。受信伝文：{0}",readLine));
                    ret = false;
                }
                else
                {
                    Boolean parse1 = int.TryParse(split[0], out data1);
                    Boolean parse2 = int.TryParse(split[1], out data2);
                    data3 = split[2];
                    ret = (parse1 && parse2);
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }


        private Boolean CheckQrAlive()
        {
            Boolean ret = false;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if(ports.Length >0)
                {

                    // 取得したシリアル・ポート名を出力する
                    foreach (string port in ports)
                    {
                        if (mSerialPort.PortName.Equals(port))
                        {
                            mIsAliveQrReader = true;
                            break;
                        }
                        else
                        {
                            mIsAliveQrReader = false;
                            break;
                        }
                    }
                }
                else
                {
                    mIsAliveQrReader = false;
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
        #endregion

    }
    /// <summary>
    /// QRリーダ通信スレッドコマンドクラス
    /// </summary>
    public class CmdQrReaderCtrl
    {
        public QrReaderCtrlThreadStatus mStatus;
        public DataRow mQrDataRow;
        public CmdQrReaderCtrl(QrReaderCtrlThreadStatus status)
        {
            mStatus = status;
        }
        public CmdQrReaderCtrl(QrReaderCtrlThreadStatus status, DataRow qrDataRow)
        {
            mStatus = status;
            mQrDataRow = qrDataRow;
        }
    }
}
