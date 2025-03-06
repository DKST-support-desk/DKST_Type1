using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnect.SQL;
using log4net;
using TskCommon;

namespace PLOSDeviceConnectionManager.Thread
{
    public class DataDeleteCtrl : WorkerThread
    {

        #region <Field>
        /// <summary> 削除が実行されるファイル総量のしきい値(GB) </summary>
        private int thresholdFileSize = 0;

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SqlLineInfoMst mSqlLineInfoMst;
        private SqlCycleResultTbl mSqlCycleResultTbl;
        private SqlPlanOperatingShiftTbl mSqlPlanOperatingShiftTbl;
        private SqlOperatingShiftProductionQuantityTbl mSqlOperatingShiftProductionQuantityTbl;
        private SqlResultOperatingShiftTbl mSqlResultOperatingShiftTbl;
        private SqlOperatingShiftExclusionTbl mSqlOperatingShiftExclusionTbl;
        private int mShiftUpdateTimeMinute;
        private int mShiftUpdateTimeHour;
        private bool mIsEnableDeleteFlag;

        private DateTime mPrevUpdateDay;
        #endregion


        #region <Property>

        public static int Files { get; private set; }
        public static int AllFiles { get; private set; }

        #endregion

        #region <Constructor>

        public DataDeleteCtrl(int cycleTime)
        {
            this.CycleTime = cycleTime;
            mSqlLineInfoMst = new SqlLineInfoMst(Program.AppSetting.SystemParam.DbConnect);

            mSqlCycleResultTbl = new SqlCycleResultTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlPlanOperatingShiftTbl = new SqlPlanOperatingShiftTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlOperatingShiftProductionQuantityTbl = new SqlOperatingShiftProductionQuantityTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlResultOperatingShiftTbl = new SqlResultOperatingShiftTbl(Program.AppSetting.SystemParam.DbConnect);
            mSqlOperatingShiftExclusionTbl = new SqlOperatingShiftExclusionTbl(Program.AppSetting.SystemParam.DbConnect);

            // シフト更新時刻の作成用の時分定義
            String[] splitShiftUpdateTime = Program.AppSetting.SystemParam.UpdateShiftInfo.Split(':');
            int.TryParse(splitShiftUpdateTime[0], out mShiftUpdateTimeHour);
            int.TryParse(splitShiftUpdateTime[1], out mShiftUpdateTimeMinute);

            // ファイル削除閾値
            thresholdFileSize = Program.AppSetting.SystemParam.DeleteFileThreshold;
        }

        #endregion

        #region <Method>

        protected override void Proc()
        {
            DeleteCycleData();
            DeleteMovieFile();
            base.Proc();
        }

        protected override void QueueEvent(object obj)
        {
            if (obj == null)
            {
                return;
            }
        }

        /// <summary>
        /// サイクル実績テーブルを削除する(定時削除)
        /// </summary>
        /// <returns></returns>
        private Boolean DeleteCycleData()
        {
            Boolean ret = false;
            try
            {
                DateTime shiftUpdateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, mShiftUpdateTimeHour, mShiftUpdateTimeMinute, 0);

                // 
                if (mIsEnableDeleteFlag && DateTime.Now > shiftUpdateTime)
                {
                    DataTable dt = mSqlLineInfoMst.Select();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        logger.Info(String.Format("【DeleteCycleData】"));

                        // 削除対象の稼働日を確定する(設定日以前の稼働日を持つデータを削除する)
                        double cycleDelSpan = dt.Rows[0].Field<int>(ColumnLineInfoMst.RESULT_SAVE_SPAN) * (-1);
                        DateTime deleteDt = shiftUpdateTime.AddDays(cycleDelSpan);

                        // 実績削除
                        // サイクル実績の定時削除
                        mSqlCycleResultTbl.Delete(deleteDt);
                        // 実績稼働シフトテーブルの削除
                        mSqlResultOperatingShiftTbl.Delete(deleteDt);
                        // 稼働シフト品番ライプ毎生産数テーブルの定時削除
                        mSqlOperatingShiftProductionQuantityTbl.Delete(deleteDt);

                        // 計画削除
                        // 計画稼働シフトテーブルの削除
                        mSqlPlanOperatingShiftTbl.Delete(deleteDt);
                        // 稼働シフト除外時間テーブルの削除
                        mSqlOperatingShiftExclusionTbl.Delete(deleteDt);

                        mIsEnableDeleteFlag = false;
                        mPrevUpdateDay = DateTime.Today;
                    }

                }
                // 現在の日付が前回更新日付を超えていれば再更新する
                if (DateTime.Today > mPrevUpdateDay.Date && !mIsEnableDeleteFlag)
                {
                    logger.Info(String.Format("【DeleteCycleData】日付更新。対象サイクル実績削除フラグON"));
                    mIsEnableDeleteFlag = true;
                }

                ret = true;
            }
            catch (Exception ex)
            {
                // TODO: エラーログ
                logger.Error(String.Format("【DeleteCycleData】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 現象動画を常時削除する
        /// </summary>
        /// <returns></returns>
        private Boolean DeleteMovieFile()
        {
            Boolean ret = false;
            try
            {
                // フォルダ内に内包する合計サイズを取得する
                long fileSize = 0;
                List<String> fileList = new List<String>(System.IO.Directory.GetFiles(Program.AppSetting.WebcamParam.CycleVideoPath));

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Program.AppSetting.WebcamParam.CycleVideoPath);

                foreach (var file in dirInfo.GetFiles())
                {
                    fileSize += file.Length;
                }
                double fileSizeGb = fileSize / Math.Pow(1024, 3);

                // ファイルサイズのしきい値を超えているかを判断し、こえている場合は古いファイルを削除する
                if (fileSizeGb > thresholdFileSize)
                {
                    DataTable dt = mSqlLineInfoMst.Select();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        double movieDelSpan = dt.Rows[0].Field<int>(ColumnLineInfoMst.MOVIE_SAVE_SPAN) * (-1);
                        DateTime deleteDt = DateTime.Now.AddDays(movieDelSpan);

                        foreach (var file in dirInfo.GetFiles())
                        {
                            if (file.LastWriteTime < deleteDt)
                            {
                                // 対象ファイルを削除
                                file.Delete();

                                // 対象ファイルに紐づくサイクル実績データのフラグを変更する
                                String[] splitInfo = System.IO.Path.GetFileNameWithoutExtension(file.FullName).Split('_');
                                int year = -1;
                                int month = -1;
                                int day = -1;
                                int hour = -1;
                                int minute = -1;
                                int second = -1;
                                int processIdx = -1;
                                String date = splitInfo[1];
                                String time = splitInfo[2];
                                String process = splitInfo[3].Replace("Process", "");

                                if (int.TryParse(date.Substring(0, 4), out year)
                                    && int.TryParse(date.Substring(4, 2), out month)
                                    && int.TryParse(date.Substring(6, 2), out day)
                                    && int.TryParse(time.Substring(0, 2), out hour)
                                    && int.TryParse(time.Substring(2, 2), out minute)
                                    && int.TryParse(time.Substring(2, 2), out second)
                                    && int.TryParse(process, out processIdx))
                                {
                                    DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
                                    DateTime dateTimeStart = dateTime.AddMilliseconds(0);
                                    DateTime dateTimeEnd = dateTime.AddMilliseconds(999);
                                    mSqlCycleResultTbl.Update(dateTimeStart, dateTimeEnd, processIdx - 1);
                                }


                            }
                        }
                    }

                }


                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("【DeleteMovieFile】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 起動時、素材動画フォルダの内容物を全削除する
        /// </summary>
        /// <returns></returns>
        public static Boolean DeleteTempFile()
        {
            Boolean ret = false;
            try
            {
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Program.AppSetting.WebcamParam.TempFilePath);
                Files = dirInfo.GetFiles().Length;
                AllFiles = dirInfo.GetFiles().Length;
                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();
                    Files -= 1;
                }

                ret = true;
            }
            catch(Exception ex)
            {
                logger.Error(String.Format("【DeleteMovieFile】{0}, 詳細：{1}", ex.Message, ex.StackTrace));
                ret = false;
            }
            return ret;
        }

        #endregion
    }
}
