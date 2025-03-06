using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using log4net;

namespace PLOSDeviceConnectionManager
{
    public class XmlFileManagement
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 設定XMLのパス(実行ファイルと同階層を規定とする)
        /// </summary>
        private const String XML_PATH = "./Application.xml";

        /// 設定XMLファイルを読み込む
        /// </summary>
        /// <param name="innerValues"></param>
        /// <returns></returns>
        public static Boolean ReadParamEnvironmentXml(out AppSetting appSetting)
        {
            StreamReader sr = null;
            try
            {
                String xmlPath = XML_PATH;

                if (!File.Exists(xmlPath))
                {
                    appSetting = new AppSetting();
                    logger.Error("設定ファイルが存在しません。");
                    return false;
                }

                sr = new StreamReader(xmlPath, Encoding.Default);
                XmlSerializer serializer = new XmlSerializer(typeof(AppSetting));

                appSetting = (AppSetting)serializer.Deserialize(sr);

                return true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                appSetting = null;
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        public static Boolean WriteParamEnvironmentXml(AppSetting appSetting)
        {
            StreamWriter sw = null;
            try
            {
                // ファイルパスの生成
                String xmlPath = XML_PATH;

                sw = new StreamWriter(xmlPath, false, Encoding.Default);
                XmlSerializer serializer = new XmlSerializer(typeof(AppSetting));
                serializer.Serialize(sw, appSetting);

                sw.Close();

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }

    /// <summary>
    /// XML構造クラス
    /// </summary>
    [XmlRoot(ElementName = "ApplicationSetting")]
    public class AppSetting
    {
        [XmlElement("System")]
        public AppSettingSystemSection SystemParam;

        [XmlElement("WebCam")]
        public AppSettingWebCamSection WebcamParam;

        [XmlElement("Dio")]
        public AppSettingDioSection DioParam;

        public AppSetting() { }
        public AppSetting(AppSettingSystemSection system, AppSettingWebCamSection webcam, AppSettingDioSection dioParam)
        {
            this.SystemParam = system;
            this.WebcamParam = webcam;
            this.DioParam = dioParam;
        }
    }
    public class AppSettingSystemSection
    {
        /// <summary>
        /// DB接続認証情報
        /// </summary>
        [XmlElement("DBConnect")]
        String dbConnect;
        public String DbConnect
        {
            get { return this.dbConnect; }
            set { this.dbConnect = value; }
        }

        /// <summary>
        /// 定周期タイマー監視間隔(ミリ秒)
        /// </summary>
        [XmlElement("TimeInterval")]
        int timeInterval;
        public int TimeInterval
        {
            get { return this.timeInterval; }
            set { this.timeInterval = value; }
        }

        /// <summary>
        /// 稼働シフト情報更新時刻
        /// </summary>
        [XmlElement("UpdateShiftInfo")]
        String updateShiftInfo;
        public String UpdateShiftInfo
        {
            get { return this.updateShiftInfo; }
            set { this.updateShiftInfo = value; }
        }

        /// <summary>
        /// ファイル削除閾値(GB単位)
        /// </summary>
        [XmlElement("DeleteFileThreshold")]
        int deleteFileThreshold;
        public int DeleteFileThreshold
        {
            get { return this.deleteFileThreshold; }
            set { this.deleteFileThreshold = value; }
        }

        /// <summary>
        /// カメラ接続最大数
        /// </summary>
        [XmlElement("CameraMaxNum")]
        int cameraMaxNum;
        public int CameraMaxNum
        {
            get { return this.cameraMaxNum; }
            set { this.cameraMaxNum = value; }
        }

        /// <summary>
        /// カメラ接続最大数
        /// </summary>
        [XmlElement("QrReaderMaxNum")]
        int qrReaderMaxNum;
        public int QrReaderMaxNum
        {
            get { return this.qrReaderMaxNum; }
            set { this.qrReaderMaxNum = value; }
        }

        /// <summary>
        /// GPU使用設定
        /// 0:GPU使用無し
        /// 1:GPU使用有り
        /// </summary>
        [XmlElement("UseGpu")]
        int useGpu;
        public int UseGpu
        {
            get { return this.useGpu; }
            set { this.useGpu = value; }
        }

        /// <summary>
        /// フォントカラー
        /// </summary>
        [XmlElement("TextFontColor")]
        String textFontColor;
        public String TextFontColor
        {
            get { return this.textFontColor; }
            set { this.textFontColor = value; }
        }

        public AppSettingSystemSection() { }
    }
    public class AppSettingWebCamSection
    {

        /// <summary>
        /// サイクル動画作成用テンポラリフォルダ
        /// </summary>
        [XmlElement("TempDataPath")]
        String tempFilePath;
        public String TempFilePath
        {
            get { return this.tempFilePath; }
            set { this.tempFilePath = value; }
        }

        /// <summary>
        /// サイクル動画フォルダ
        /// </summary>
        [XmlElement("CycleVideoPath")]
        String cycleVideoPath;
        public String CycleVideoPath
        {
            get { return this.cycleVideoPath; }
            set { this.cycleVideoPath = value; }
        }

        /// <summary>
        /// 素材動画削除
        /// </summary>
        [XmlElement("DeleteFile")]
        int deleteFile;
        public int DeleteFile
        {
            get { return this.deleteFile; }
            set { this.deleteFile = value; }
        }

        /// <summary>
        /// 素材動画撮影方式
        /// </summary>
        [XmlElement("ShowStream")]
        int showStream;
        public int ShowStream
        {
            get { return this.showStream; }
            set { this.showStream = value; }
        }
        public AppSettingWebCamSection() { }
    }
    public class AppSettingDioSection
    {

        /// <summary>
        /// DIOデバイス名
        /// </summary>
        [XmlElement("DeviceName")]
        String deviceName;
        public String DeviceName
        {
            get { return this.deviceName; }
            set { this.deviceName = value; }
        }
        /// <summary>
        /// ブザー鳴動時間(秒)
        /// </summary>
        [XmlElement("BuzzerRingingTime")]
        String buzzerRingingTime;
        public String BuzzerRingingTime
        {
            get { return this.buzzerRingingTime; }
            set { this.buzzerRingingTime = value; }
        }


        public AppSettingDioSection() { }
    }
}
