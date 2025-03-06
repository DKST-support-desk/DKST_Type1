using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskCommon;
using log4net;
using System.Text.RegularExpressions;
using System.Data;
using System.Runtime.InteropServices;

namespace PLOSDeviceConnectionManager.Thread
{
    public enum DataSaveThreadStatus
    {
        /// <summary> 起動 </summary>
        INIT,
        /// <summary> 終了 </summary>
        TERM,
        /// <summary> サイクル </summary>
        CONCAT,
    }

    /// <summary>
    /// メモリ展開するデータの管理クラス
    /// </summary>
    public class DataCtrlThread : WorkerThread
    {
        #region <Field>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int mThreadIndex = -1;

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


        #endregion

        #region <Property>
        public Boolean IsSavingMovie { get; private set; }
        #endregion

        #region <Constructor>

        public DataCtrlThread(int cycleTime, int threadIdx)
        {
            this.CycleTime = cycleTime;
            mThreadIndex = threadIdx;
        }

        #endregion

        #region <Method>

        protected override void Proc()
        {
            base.Proc();
        }

        protected override void QueueEvent(object obj)
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is CmdDataSave)
            {
                switch (((CmdDataSave)obj).mStatus)
                {
                    case DataSaveThreadStatus.INIT:
                        break;
                    case DataSaveThreadStatus.TERM:
                        break;
                    case DataSaveThreadStatus.CONCAT:

                        if (((CmdDataSave)obj).mNeedToSave)
                        {
                            ConcatMovie(((CmdDataSave)obj).mCycleStartTime,
                                ((CmdDataSave)obj).mCycleEndTime,
                                ((CmdDataSave)obj).mVideoFileName,
                                ((CmdDataSave)obj).mVideoFileNameList,
                                ((CmdDataSave)obj).mAudioFileName,
                                ((CmdDataSave)obj).mCameraIndex,
                                ((CmdDataSave)obj).mProcessIndex,
                                ((CmdDataSave)obj).mShiftTable);
                        }
                        else
                        {
                            logger.Debug(String.Format("結合処理不要のため、生成元ファイルを削除。工程インデックス：{0}, カメラインデックス：{1}", ((CmdDataSave)obj).mProcessIndex, ((CmdDataSave)obj).mCameraIndex));
                            DeleteFile(((CmdDataSave)obj).mVideoFileName);
                            DeleteFile(((CmdDataSave)obj).mAudioFileName);
                            foreach (String item in ((CmdDataSave)obj).mVideoFileNameList)
                            {
                                DeleteFile(item);
                            }
                        }
                        break;
                }
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


        /// <summary>
        /// 現象動画の結合処理
        /// </summary>
        /// <param name="cycleStartDateTime">当該サイクルの開始時刻</param>
        /// <param name="videoFile">当該サイクルの素材動画</param>
        /// <param name="audioFile">当該サイクルの素材音声</param>
        /// <param name="camIdx">取得したカメラのカメラインデックス</param>
        /// <param name="processIdx">当該サイクルの工程インデックス</param>
        /// <returns></returns>
        private Boolean ConcatMovie(DateTime cycleStartDateTime, DateTime cycleEndDateTime, String videoFile, List<String> videoFileList, String audioFile, int camIdx, int processIdx, DataTable shiftTable)
        {
            Boolean ret = false;
            Boolean isInputTimeText = false;
            IsSavingMovie = true;
            String gpuUseCmd = "";
            try
            {
                String videoNameTemp = String.Format(Program.CYCLE_VIDEO_FILE_NAME_FORMAT, cycleStartDateTime.ToString("yyyyMMdd_HHmmss"), processIdx + 1, camIdx + 1);
                String tempDir = Program.AppSetting.WebcamParam.CycleVideoPath;
                String cycleVideoName = System.IO.Path.Combine(tempDir, videoNameTemp);

                if (videoFileList != null && videoFileList.Count > 0)
                {
                    logger.Info(String.Format("【ConcatMovie】現象動画要結合素材生成。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}", processIdx, camIdx, cycleStartDateTime));

                    String tempVideoFileListName = String.Format("FILELIST_{0}_Process{1}_Cam{2}.txt", cycleStartDateTime.ToString("yyyyMMdd_HHmmss"), processIdx + 1, camIdx + 1);
                    String tempVideoFileListDir = Program.AppSetting.WebcamParam.TempFilePath;
                    String tempVideoFileList = System.IO.Path.Combine(tempVideoFileListDir, tempVideoFileListName);
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(tempVideoFileList, true, Encoding.GetEncoding("shift_jis")))
                    {
                        foreach (String item in videoFileList)
                        {
                            logger.Info(String.Format("【ConcatMovie】現象動画要結合素材。対象ファイル名：{0},", item));
                            sw.WriteLine(String.Format("file \'{0}\'", item));
                        }
                        sw.Close();
                    }
                    using (var process = new Process())
                    {
                        // 外部プロセスを生成し、ffmepgを立ち上げて現象動画の作成を実施する
                        process.StartInfo.FileName = "ffmpeg";
                        if (Program.AppSetting.SystemParam.UseGpu > 0)
                        {
                            // GPU使用設定あり
                            gpuUseCmd = "h264_nvenc";
                        }
                        else
                        {
                            // GPU使用設定なし
                            gpuUseCmd = "libx264";
                        }
                        process.StartInfo.Arguments = String.Format("-f concat -safe 0 -i {0} -c:v copy -map 0:v:0 -vcodec {1}  -y {2}", tempVideoFileList, gpuUseCmd, videoFile);
                        // ffmpeg -f concat -safe 0 -i mylist.txt -c:v copy -map 0:v:0 -vcodec libx264 -pix_fmt yuv420p -y output.mp4

                        // コマンドプロンプトを表示しない
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        logger.Info(String.Format("【ConcatMovie】現象動画要結合素材生成開始。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}", processIdx, camIdx, cycleStartDateTime));
                        // コマンド実行
                        var flag = process.Start();

                        // コマンド終了まで待機
                        process.WaitForExit();
                        if (process.ExitCode.ToString() != "0")
                        {
                            logger.Error(String.Format("【ConcatMovie】現象動画要結合素材生成に失敗。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}, エラーコード：{3}\n発行コマンド：{4} {5}",
                                processIdx, camIdx, cycleStartDateTime, process.ExitCode, process.StartInfo.FileName, process.StartInfo.Arguments));
                        }
                        else
                        {
                            // 生成に成功、素材の削除
                            if (Program.AppSetting.WebcamParam.DeleteFile > 0)
                            {
                                foreach (String item in videoFileList)
                                {
                                    DeleteFile(item);
                                }
                            }
                            DeleteFile(tempVideoFileList);
                            logger.Info(String.Format("【ConcatMovie】現象動画要結合素材生成終了。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}", processIdx, camIdx, cycleStartDateTime));
                        }
                    }
                }

                using (var process = new Process())
                {
                    // 外部プロセスを生成し、ffmepgを立ち上げて現象動画の作成を実施する
                    process.StartInfo.FileName = "ffmpeg";
                    if (Program.AppSetting.SystemParam.UseGpu > 0)
                    {
                        // GPU使用設定あり
                        gpuUseCmd = "h264_nvenc";
                    }
                    else
                    {
                        // GPU使用設定なし
                        gpuUseCmd = "libx264";
                    }
                    String fontColor = Program.AppSetting.SystemParam.TextFontColor;
                    if (fontColor.Equals(String.Empty))
                    {
                        fontColor = "Red";
                    }

                    if (audioFile.Equals(Program.NO_MIC_USE))
                    {
                        // マイクの使用設定がない場合、音声抜きの動画作成コマンドを作成する
                        process.StartInfo.Arguments = String.Format("-i {0} -c:v copy -map 0:v:0 -vcodec {1} -pix_fmt yuv420p -filter_complex \"drawtext = fontfile =/ path / to / fontfile:text = \'{2} - {3}\':fontsize=32:fontcolor={4}:x=600:y=670 \" -y {5}", videoFile, gpuUseCmd, cycleStartDateTime.ToString("yyyy.MM.dd_HH;mm;ss"), cycleEndDateTime.ToString("yyyy.MM.dd_HH;mm;ss"), fontColor, cycleVideoName);
                        //process.StartInfo.Arguments = $@"-i {videoFile} -c:v copy -map 0:v:0 -vcodec libx264  -filter_complex \"drawtext = fontfile =/ path / to / fontfile:text = \\'{cycleDateTime.ToString("yyyy.MM.dd_HH;mm;ss")}\\':fontsize = 32:fontcolor = white:x = 900:y = 650 \\" -y {cycleVideoName}";
                        //process.StartInfo.Arguments = String.Format("-i {0} -c:v copy -map 0:v:0 -vcodec {1} -pix_fmt yuv420p -y {2}", videoFile, gpuUseCmd, cycleVideoName);
                    }
                    else
                    {
                        // マイクの使用設定がある場合、音声ありの動画作成コマンドを作成する
                        process.StartInfo.Arguments = String.Format("-i {0} -i {1} -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -vcodec {2} -pix_fmt yuv420p -filter_complex \"drawtext = fontfile =/ path / to / fontfile:text = \'{3} - {4}\':fontsize=32:fontcolor={5}:x=600:y=670 \" -y {6}", videoFile, audioFile, gpuUseCmd, cycleStartDateTime.ToString("yyyy.MM.dd_HH;mm;ss"), cycleEndDateTime.ToString("yyyy.MM.dd_HH;mm;ss"), fontColor, cycleVideoName);
                        //process.StartInfo.Arguments = $@"-i {videoFile} -i {audioFile} -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -vcodec libx264 -y {cycleVideoName}";
                        //process.StartInfo.Arguments = String.Format("-i {0} -i {1} -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -vcodec {2} -pix_fmt yuv420p -y {3}", videoFile, audioFile, gpuUseCmd, cycleVideoName);
                    }

                    // コマンドプロンプトを表示しない
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    logger.Info(String.Format("【ConcatMovie】現象動画生成開始。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}", processIdx, camIdx, cycleStartDateTime));
                    // コマンド実行
                    var flag = process.Start();

                    // コマンド終了まで待機
                    process.WaitForExit();
                    if (process.ExitCode.ToString() != "0")
                    {
                        logger.Error(String.Format("【ConcatMovie】現象動画生成に失敗。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}, エラーコード：{3}\n発行コマンド：{4} {5}",
                            processIdx, camIdx, cycleStartDateTime, process.ExitCode, process.StartInfo.FileName, process.StartInfo.Arguments));
                        cycleVideoName = "";
                    }
                    else
                    {
                        // 生成に成功、素材の削除
                        if (Program.AppSetting.WebcamParam.DeleteFile > 0)
                        {
                            DeleteFile(videoFile);
                            DeleteFile(audioFile);
                        }

                        //isInputTimeText = true;
                        isInputTimeText = false;
                        logger.Info(String.Format("【ConcatMovie】現象動画生成終了。対象工程インデックス：{0}, 対象カメラインデックス：{1}, 対象サイクル開始時刻：{2}", processIdx, camIdx, cycleStartDateTime));
                    }
                }

                // イベント発行、当該サイクルデータへのファイル名書き込み機能
                Program.eventManagementThread.AddEvent(new CmdEventManager(EventManagementThreadStatus.RECV_MOVIE_CREATED, cycleVideoName, processIdx, cycleStartDateTime, shiftTable));
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            IsSavingMovie = false;
            return ret;
        }

        // ファイル削除
        private void DeleteFile(String filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    logger.Warn(String.Format("【DeleteFile】対象のファイルを削除しました。削除対象：{0}", filePath));
                }
                else
                {
                    logger.Warn(String.Format("【DeleteFile】削除対象のファイルが存在しません。検索対象：{0}", filePath));
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【ERROR】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
            }
        }

        #endregion
    }


    public class CmdDataSave
    {
        public DataSaveThreadStatus mStatus;
        public DateTime mCycleStartTime;
        public DateTime mCycleEndTime;
        public String mVideoFileName;
        public List<String> mVideoFileNameList;
        public String mAudioFileName;
        public int mCameraIndex;
        public int mProcessIndex;
        public Boolean mNeedToSave;
        public DataTable mShiftTable;

        /// <summary> デフォルトのコンストラクタ </summary>
        public CmdDataSave(DataSaveThreadStatus status)
        {
            mStatus = status;
        }
        /// <summary> 結合イベント(CONCAT)時のコンストラクタ </summary>
        public CmdDataSave(DataSaveThreadStatus status, DateTime cycleStartDateTime, DateTime cycleEndDateTime, String videoFile, List<String> videoFileNameList, String audioFile, int camIdx, int processIdx, Boolean needSave, DataTable shiftTable)
        {
            mStatus = status;
            mCycleStartTime = cycleStartDateTime;
            mCycleEndTime = cycleEndDateTime;
            mVideoFileName = videoFile;
            mVideoFileNameList = videoFileNameList;
            mAudioFileName = audioFile;
            mCameraIndex = camIdx;
            mProcessIndex = processIdx;
            mNeedToSave = needSave;
            mShiftTable = shiftTable;
        }
    }
}
