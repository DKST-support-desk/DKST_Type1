using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnect;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
	public partial class UCOperating_Shift_Regist : UserControl
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバー変数"
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		/// <summary>
		/// 計画稼働シフトテーブルアクセス
		/// </summary>
		private SqlPlanOperatingShiftTbl mSqlPlanOperatingShiftTbl;

		/// <summary>
		/// 計画稼働シフトテーブルデータ
		/// </summary>
		private DataTable mDtPlanOperatingShiftTbl;

		/// <summary>
        ///実績稼働シフトテーブルアクセス
        /// </summary>
        private SqlResultOperatingShiftTbl mSqlResultOperatingShiftTbl;

        /// <summary>
        /// 実績稼働シフトテーブルデータ
        /// </summary>
        private DataTable mDtResultOperatingShiftTbl;

        /// <summary>
		/// 稼働シフト品番タイプ毎生産数テーブルアクセス
		/// </summary>
		private SqlOperatingShiftProductionQuantityTbl mSqlOperatingShiftProductionQuantityTbl;

		/// <summary>
		/// 稼働シフト品番タイプ毎生産数テーブルデータ
		/// </summary>
		private DataTable mDtOperatingShiftProductionQuantityTbl;

        /// <summary>
        /// 稼働シフト除外時間テーブルアクセス
        /// </summary>
        private SqlOperatingShiftExclusionTbl mSqlOperatingShiftExclusionTbl;

        /// <summary>
        /// 稼働シフト除外時間テーブルデータ
        /// </summary>
        private DataTable mDtOperatingShiftExclusionTbl;

        /// <summary>
        /// 稼働シフトパターンテーブルアクセス
        /// </summary>
        private SqlOperatingShiftPatternTbl mSqlOperatingShiftPatternTbl;

        /// <summary>
        /// 稼働シフトパターンテーブルデータ
        /// </summary>
        private DataTable mDtSqlOperatingShiftTbl;

        /// <summary>
        /// 稼働除外パターンテーブルアクセス
        /// </summary>
        private SqlOperatingShiftExclusionPatternTbl mSqlOperatingShiftExclusionPatternTbl;

        /// <summary>
        /// 稼働除外パターンテーブルデータ
        /// </summary>
        private DataTable mDtOperatingShiftExclusionPatternTbl;

		/// <summary>
		/// 現在選択されている稼働日
		/// </summary>
		DateTime mCurTargetDate;
		#endregion "メンバー変数"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region "コンストラクタ"
		/// <summary>
		/// 稼働登録通知画面
		/// </summary>
		public UCOperating_Shift_Regist()
		{
			InitializeComponent();

			// SQL接続クラスインスタンスの取得
			mSqlPlanOperatingShiftTbl = new SqlPlanOperatingShiftTbl(Properties.Settings.Default.ConnectionString_New);
			mSqlResultOperatingShiftTbl = new SqlResultOperatingShiftTbl(Properties.Settings.Default.ConnectionString_New);
			mSqlOperatingShiftProductionQuantityTbl = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
			mSqlOperatingShiftExclusionTbl = new SqlOperatingShiftExclusionTbl(Properties.Settings.Default.ConnectionString_New);
			mSqlOperatingShiftPatternTbl = new SqlOperatingShiftPatternTbl(Properties.Settings.Default.ConnectionString_New);
			mSqlOperatingShiftExclusionPatternTbl = new SqlOperatingShiftExclusionPatternTbl(Properties.Settings.Default.ConnectionString_New);

			ucCalendar.DaysFont =
				new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			
			ucOperating_Shift_Item1.OperationShift = 1;
			ucOperating_Shift_Item2.OperationShift = 2;
			ucOperating_Shift_Item3.OperationShift = 3;

			InitializeComponentData();

			ucOperating_Shift_Calendar_Day_Main1.ExistPlan = false;
			ucOperating_Shift_Calendar_Day_Main1.ExistActual = false;
			ucOperating_Shift_Calendar_Day_Main1.DaysBackColor = System.Drawing.Color.LightGray;
			ucOperating_Shift_Calendar_Day_Main1.DaysForeColor = System.Drawing.Color.Gray;
			ucOperating_Shift_Calendar_Day_Main3.Image = global::PLOSMaintenance.Properties.Resources.StatusGood;
		}
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 計画複製ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlanRegistBtnClicked(object sender, EventArgs e)
		{
            try
            {
				logger.Info("稼働登録 計画複製ボタン押下");

				if (!UpdateShiftData(ucOperating_Shift_Item1) || !UpdateShiftData(ucOperating_Shift_Item2) || !UpdateShiftData(ucOperating_Shift_Item3))
				{
					logger.Warn("複製時のデータ吸い上げに失敗しました");
					return;
				}
				// シフト間データ比較チェック
				else if (!CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item2)
					 || !CheckShiftDate(ucOperating_Shift_Item2, ucOperating_Shift_Item3)
					 || !CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item3))
                {
					logger.Warn("複製時のデータ吸い上げに失敗しました");
					return;
				}

				const int FirstOperationShift = 1;
				const int LastOperationShift = 3;

				FrmCalendar_MultiSelect frm = new FrmCalendar_MultiSelect();
				if (frm.ShowDialog(ParentForm) == DialogResult.OK)
				{
					logger.Info(String.Format("{0}/{1}/{2}", mCurTargetDate.Year, mCurTargetDate.Month, mCurTargetDate.Day) + "の計画を複製します");

					// 選択日付リストを取得
					List<DateTime> selectedDaysList = frm.SelectedDaysList;

					for (int i = 0; i < selectedDaysList.Count; i++)
					{
						// 更新日を取得
						DateTime date = selectedDaysList[i];

						for (int operationShift = FirstOperationShift; operationShift <= LastOperationShift; operationShift++)
						{
							// 計画のコピー元のインデックスを取得
							int srcIdx = mDtPlanOperatingShiftTbl.Rows.IndexOf(
											mDtPlanOperatingShiftTbl.AsEnumerable().Where(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE) == mCurTargetDate)
																				   .Where(x => x.Field<Int32>(PlanOperatingShiftTblColumn.OPERATION_SHIFT) == operationShift)
																				   .FirstOrDefault()
																			  );
							if (srcIdx != -1)
							{
								// コピー元データの取得
								DataRow srcData = mDtPlanOperatingShiftTbl.Rows[srcIdx];

								// 計画のコピー先のインデックスを取得
								int copyIdx = mDtPlanOperatingShiftTbl.Rows.IndexOf(
												mDtPlanOperatingShiftTbl.AsEnumerable().Where(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE) == date)
																						 .Where(x => x.Field<Int32>(PlanOperatingShiftTblColumn.OPERATION_SHIFT) == operationShift)
																					   .FirstOrDefault()
																				   );
								if (copyIdx != -1)
								{
									// データを更新
									DataRow copyData = mDtPlanOperatingShiftTbl.Rows[copyIdx];

									copyData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = srcData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY];
									DateTime startTime = (DateTime)srcData[PlanOperatingShiftTblColumn.START_TIME];
									DateTime endTime = (DateTime)srcData[PlanOperatingShiftTblColumn.END_TIME];

									DateTime copyDataStartDate = mCurTargetDate.Date < startTime.Date ? date.AddDays(1) : date;
									DateTime copyDataEndDate = mCurTargetDate.Date < endTime.Date ? date.AddDays(1) : date;

									copyData[PlanOperatingShiftTblColumn.START_TIME] = new DateTime(copyDataStartDate.Year, copyDataStartDate.Month, copyDataStartDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
									copyData[PlanOperatingShiftTblColumn.END_TIME] = new DateTime(copyDataEndDate.Year, copyDataEndDate.Month, copyDataEndDate.Day, endTime.Hour, endTime.Minute, endTime.Second);
									copyData[PlanOperatingShiftTblColumn.OPERATION_SECOND] = srcData[PlanOperatingShiftTblColumn.OPERATION_SECOND];
									copyData[PlanOperatingShiftTblColumn.USE_FLAG] = srcData[PlanOperatingShiftTblColumn.USE_FLAG];
								}
								else
								{
									// データを追加
									mDtPlanOperatingShiftTbl.Rows.Add(mDtPlanOperatingShiftTbl.NewRow());
									DataRow addData = mDtPlanOperatingShiftTbl.Rows[mDtPlanOperatingShiftTbl.Rows.Count - 1];

									addData[PlanOperatingShiftTblColumn.OPERATION_DATE] = date;
									addData[PlanOperatingShiftTblColumn.OPERATION_SHIFT] = operationShift;
									addData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = srcData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY];

									DateTime startTime = (DateTime)srcData[PlanOperatingShiftTblColumn.START_TIME];
									DateTime endTime = (DateTime)srcData[PlanOperatingShiftTblColumn.END_TIME];

									DateTime addDataStartDate = mCurTargetDate.Date < startTime.Date ? date.AddDays(1) : date;
									DateTime addDataEndDate = mCurTargetDate.Date < endTime.Date ? date.AddDays(1) : date;

									addData[PlanOperatingShiftTblColumn.START_TIME] = new DateTime(addDataStartDate.Year, addDataStartDate.Month, addDataStartDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
									addData[PlanOperatingShiftTblColumn.END_TIME] = new DateTime(addDataEndDate.Year, addDataEndDate.Month, addDataEndDate.Day, endTime.Hour, endTime.Minute, endTime.Second);
									addData[PlanOperatingShiftTblColumn.OPERATION_SECOND] = srcData[PlanOperatingShiftTblColumn.OPERATION_SECOND];
									addData[PlanOperatingShiftTblColumn.USE_FLAG] = srcData[PlanOperatingShiftTblColumn.USE_FLAG];
								}

								// 編集フラグを立てる
								FrmMain.gIsDataChange = true;
							}

							// 除外時間のコピー元になる日付の最初のインデックスを取得
							int exIdx = mDtOperatingShiftExclusionTbl.Rows.IndexOf(
											mDtOperatingShiftExclusionTbl.AsEnumerable().Where(x => x.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE) == mCurTargetDate)
																					.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT) == operationShift)
																					.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) == 0)
																					.FirstOrDefault()
																				);
							if (exIdx != -1)
							{
								// 除外時間のコピー先の最初のインデックスを取得
								int copyExIdx = mDtOperatingShiftExclusionTbl.Rows.IndexOf(
												mDtOperatingShiftExclusionTbl.AsEnumerable().Where(x => x.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE) == date)
																							.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT) == operationShift)
																							.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) == 0)
																						.FirstOrDefault()
																					);
								if (copyExIdx != -1)
								{
                                    for(int idx = 0; idx < 12; idx++)
                                    {
										// データを更新
										// 更新先のデータを取得
										DataRow copyData = mDtOperatingShiftExclusionTbl.Rows[copyExIdx];

										DateTime startTime = (DateTime)mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME];
										DateTime endTime = (DateTime)mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME];

										DateTime copyDataStartDate = mCurTargetDate.Date < startTime.Date ? date.AddDays(1) : date;
										DateTime copyDataEndDate = mCurTargetDate.Date < endTime.Date ? date.AddDays(1) : date;

										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = new DateTime(copyDataStartDate.Year, copyDataStartDate.Month, copyDataStartDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = new DateTime(copyDataEndDate.Year, copyDataEndDate.Month, copyDataEndDate.Day, endTime.Hour, endTime.Minute, endTime.Second);
										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME];
										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK];
										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK];
										copyData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS];

										exIdx++;
										copyExIdx++;
									}
								}
								else
								{
									for(int idx = 0; idx < 12; idx++)
                                    {
										// データを追加
										mDtOperatingShiftExclusionTbl.Rows.Add(mDtOperatingShiftExclusionTbl.NewRow());
										DataRow addData = mDtOperatingShiftExclusionTbl.Rows[mDtOperatingShiftExclusionTbl.Rows.Count - 1];

										addData[ColumnOperatingShiftExclusionTbl.OPERATION_DATE] = date;
										addData[ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT] = operationShift;
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX] = idx;

										DateTime startTime = (DateTime)mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME];
										DateTime endTime = (DateTime)mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME];

										DateTime addDataStartDate = mCurTargetDate.Date < startTime.Date ? date.AddDays(1) : date;
										DateTime addDataEndDate = mCurTargetDate.Date < endTime.Date ? date.AddDays(1) : date;

										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = new DateTime(addDataStartDate.Year, addDataStartDate.Month, addDataStartDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = new DateTime(addDataEndDate.Year, addDataEndDate.Month, addDataEndDate.Day, endTime.Hour, endTime.Minute, endTime.Second);
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME];
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK];
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK];
										addData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS] = mDtOperatingShiftExclusionTbl.Rows[exIdx][ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS];

										exIdx++;
									}
								}

								// 編集フラグを立てる
								FrmMain.gIsDataChange = true;
							}
						}
					}

					logger.Info($"{mCurTargetDate.ToShortDateString()}の計画を複製しました。");
					MessageBox.Show($"{mCurTargetDate.ToShortDateString()}の計画を複製しました。", "複製完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

					ucCalendar.RefreshCalendar();
				}
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// 計画数一覧ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPlanCalenderClicked(object sender, EventArgs e)
		{
            try
            {
				logger.Info("稼働登録 計画数一覧ボタン押下");

				FrmCalendar_Plan frm = new FrmCalendar_Plan();
				frm.ShowDialog();
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

        /// <summary>
		/// 登録ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnActualRegistBtnClicked(object sender, EventArgs e)
		{
			logger.Info("稼働登録 登録ボタン押下");

            try
            {
                //現在の内容で吸い上げを先に実行
                // シフトデータ取得
                if (UpdateShiftData(ucOperating_Shift_Item1) && UpdateShiftData(ucOperating_Shift_Item2) && UpdateShiftData(ucOperating_Shift_Item3))
                {
					// シフト間データ比較チェック
					if(CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item2)
					&& CheckShiftDate(ucOperating_Shift_Item2, ucOperating_Shift_Item3)
					&& CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item3))
                    {
						logger.Info("稼働登録 現在設定されている内容を登録します。よろしいですか。");
						if (DialogResult.Yes == MessageBox.Show("現在設定されている内容を登録します。よろしいですか。", "稼働登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
						{
							logger.Info("Yes押下");

							using (SqlDBConnector db = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New))
							{
								db.Create();
								db.OpenDatabase();

								SqlConnection conn = db.DbConnection;
								using (SqlTransaction transaction = conn.BeginTransaction())
								{
									// Update発行
									if (mSqlPlanOperatingShiftTbl.Upsert(mDtPlanOperatingShiftTbl, transaction))
									{
										// 計画稼働シフトテーブルからの全件取得
										logger.Debug("計画稼働シフトテーブルを更新しました。");
									}
									else
									{
										logger.Warn("計画稼働シフトテーブルの更新に失敗しました。ロールバックします。");
										transaction.Rollback();
										db.CloseDatabase();
										return;
									}
									if (mSqlResultOperatingShiftTbl.Update(mDtResultOperatingShiftTbl, transaction))
									{
										// 実績稼働シフトテーブルからの全件取得
										logger.Debug("実績稼働シフトテーブルを更新しました。");
									}
									else
									{
										logger.Warn("実績稼働シフトテーブルの更新に失敗しました。ロールバックします。");
										transaction.Rollback();
										db.CloseDatabase();
										return;
									}
									if (mSqlOperatingShiftProductionQuantityTbl.Update(mDtOperatingShiftProductionQuantityTbl, transaction))
									{
										// 稼働シフト品番タイプ毎生産数テーブルからの全件取得
										logger.Debug("稼働シフト品番タイプ毎生産数テーブルを更新しました。");
									}
									else
									{
										logger.Warn("稼働シフト品番タイプ毎生産数テーブルの更新に失敗しました。ロールバックします。");
										transaction.Rollback();
										db.CloseDatabase();
										return;
									}
									if (mSqlOperatingShiftExclusionTbl.Upsert(mDtOperatingShiftExclusionTbl, transaction))
									{
										// 稼働シフト除外時間テーブルからの全件取得
										logger.Debug("稼働シフト除外時間テーブルを更新しました。");
									}
									else
									{
										logger.Warn("稼働シフト除外時間テーブルの更新に失敗しました。ロールバックします。");
										transaction.Rollback();
										db.CloseDatabase();
										return;
									}

									transaction.Commit();
									db.CloseDatabase();
									MessageBox.Show("稼働登録しました", "稼働登録", MessageBoxButtons.OK, MessageBoxIcon.Information);

									ucOperating_Shift_Item1.DtQuantityList.Clear();
									ucOperating_Shift_Item2.DtQuantityList.Clear();
									ucOperating_Shift_Item3.DtQuantityList.Clear();
								}
							}
						}
                        else
                        {
							logger.Info("No押下");
							return;
                        }

						InitializeComponentData();

						// 編集フラグを消す
						FrmMain.gIsDataChange = false;
					}
                }
            }
			catch(Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
			}
			//ucOperating_Shift_Item1.RegistOperatingShift(ucOperating_Shift_Item1.TargetDate, 1);
			//ucOperating_Shift_Item2.RegistOperatingShift(ucOperating_Shift_Item2.TargetDate, 1);
			//ucOperating_Shift_Item3.RegistOperatingShift(ucOperating_Shift_Item3.TargetDate, 1);

			//ucOperating_Shift_Item1.RegistOperatingShift(ucOperating_Shift_Item1.TargetDate, 0);
			//ucOperating_Shift_Item2.RegistOperatingShift(ucOperating_Shift_Item2.TargetDate, 0);
			//ucOperating_Shift_Item3.RegistOperatingShift(ucOperating_Shift_Item3.TargetDate, 0);

			ucCalendar.RefreshCalendar();
		}

		/// <summary>
		/// カレンダーの日付が変更されるたび、日付を変更する前に発生します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UcCalendar_SelectChanging(object sender, Events.CalendarEventArgs e)
		{
            try
            {
				logger.Info("日付が選択されました。");

				// シフトデータ入力確認
				if (!UpdateShiftData(ucOperating_Shift_Item1) || !UpdateShiftData(ucOperating_Shift_Item2) || !UpdateShiftData(ucOperating_Shift_Item3))
				{
					e.Cancel = true;
				}
				// シフト間データ比較チェック
				else if (!CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item2)
						|| !CheckShiftDate(ucOperating_Shift_Item2, ucOperating_Shift_Item3)
						|| !CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item3))
				{
					e.Cancel = true;
				}
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// 選択日付変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCalendarSelectChanged(object sender, Events.CalendarEventArgs e)
		{
			if (!e.Selected)
            {
				// 2022.05.31 Yamasaki: 同じ日付を2回たたくとe.Selected == false としてイベントが起きるためreturnさせている。
				return;
            }
			// この時点まで到達してるということはUcCalendar_SelectChangingを突破した後なのでデータは既に吸い上げられているはず
			DateTime CurTargetDate = e.Selected ? e.TargetDate : DateTime.MinValue;
			ucOperating_Shift_Item1.TargetDate = ucOperating_Shift_Item2.TargetDate = ucOperating_Shift_Item3.TargetDate = CurTargetDate;
			mCurTargetDate = CurTargetDate;
			logger.Info("稼働登録 選択日付変更：" + CurTargetDate.ToString("yyyy/MM/dd"));

			// シフトデータ設定
			SetShiftData(ucOperating_Shift_Item1);
			SetShiftData(ucOperating_Shift_Item2);
			SetShiftData(ucOperating_Shift_Item3);

			//// シフトデータ取得
			//if (UpdateShiftData(ucOperating_Shift_Item1))
			//         {
			//             if (UpdateShiftData(ucOperating_Shift_Item2))
			//             {
			//                 if (UpdateShiftData(ucOperating_Shift_Item3))
			//                 {
			//			DateTime CurTargetDate = e.Selected ? e.TargetDate : DateTime.MinValue;
			//			ucOperating_Shift_Item1.TargetDate = ucOperating_Shift_Item2.TargetDate = ucOperating_Shift_Item3.TargetDate = CurTargetDate;
			//			mCurTargetDate = CurTargetDate;
			//			logger.Info("稼働登録通知画面 日付変更 選択日付：" + CurTargetDate.ToString("yyyy/MM/dd"));

			//			// シフトデータ設定
			//			SetShiftData(ucOperating_Shift_Item1);
			//			SetShiftData(ucOperating_Shift_Item2);
			//			SetShiftData(ucOperating_Shift_Item3);
			//		}
			//             }
			//         }
			//else
			//         {
			//	e.TargetDate = ucOperating_Shift_Item1.TargetDate;
			//	e.Selected = true;
			//	OnCalendarSelectChanged(sender, e);
			//}
		}

		/// <summary>
		/// 表示月変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCalendarSelectManthChanged(object sender, EventArgs e)
		{
            try
            {
				ucCalendar.IsChangeManth = true;

				DataTable dtPlan1 = ucOperating_Shift_Item1.PlanOperatingShift;
				DataTable dtPlan2 = ucOperating_Shift_Item2.PlanOperatingShift;
				DataTable dtPlan3 = ucOperating_Shift_Item3.PlanOperatingShift;

				if (!CheckPlan(dtPlan1, ucOperating_Shift_Item1) || !CheckPlan(dtPlan2, ucOperating_Shift_Item2) || !CheckPlan(dtPlan3, ucOperating_Shift_Item3))
				{
					ucCalendar.IsChangeManth = false;
				}

				//実績稼働シフト
				DataTable dtResult1 = ucOperating_Shift_Item1.ActualOperatingShift;
				DataTable dtResult2 = ucOperating_Shift_Item2.ActualOperatingShift;
				DataTable dtResult3 = ucOperating_Shift_Item3.ActualOperatingShift;

				if (!CheckResult(dtResult1, ucOperating_Shift_Item1) || !CheckResult(dtResult2, ucOperating_Shift_Item2) || !CheckResult(dtResult3, ucOperating_Shift_Item3))
				{
					ucCalendar.IsChangeManth = false;
				}

                //稼働シフト除外時間データ(当該日/シフトの12件分)
                DataTable dtExclusionShift1 = ucOperating_Shift_Item1.ExclusionOperatingShift;
                DataTable dtExclusionShift2 = ucOperating_Shift_Item2.ExclusionOperatingShift;
                DataTable dtExclusionShift3 = ucOperating_Shift_Item3.ExclusionOperatingShift;

                if (!CheckExclusion(dtExclusionShift1, ucOperating_Shift_Item1)
					|| !CheckExclusion(dtExclusionShift2, ucOperating_Shift_Item2)
					|| !CheckExclusion(dtExclusionShift3, ucOperating_Shift_Item3))
				{
					ucCalendar.IsChangeManth = false;
				}

				// シフト間データ比較チェック
				if (!CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item2)
				   || !CheckShiftDate(ucOperating_Shift_Item2, ucOperating_Shift_Item3)
				   || !CheckShiftDate(ucOperating_Shift_Item1, ucOperating_Shift_Item3))
                {
					ucCalendar.IsChangeManth = false;
				}
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// 勤怠パターン登録イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPatternRegist(object sender, Events.Operating_Shift_PatternEventArgs e)
		{
            try
            {
				logger.Info("勤怠パターンを登録します。");

				ucOperating_Shift_Item1.RegistOperatingShiftPattern(e.PatternId, e.IsUpdate);
				ucOperating_Shift_Item2.RegistOperatingShiftPattern(e.PatternId, e.IsUpdate);
				ucOperating_Shift_Item3.RegistOperatingShiftPattern(e.PatternId, e.IsUpdate);
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

        /// <summary>
        /// 勤怠パターン設定イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPatternLoad(object sender, Events.Operating_Shift_PatternEventArgs e)
		{
            try
            {
				if (mCurTargetDate < DateTime.Today)
				{
					logger.Warn("過去の日付には勤怠パターンを登録できません。");
					MessageBox.Show("過去の日付には勤怠パターンを登録できません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				logger.Info("勤怠パターンを設定します。");
				SetShiftPattern(e.PatternId, ucOperating_Shift_Item1);
				SetShiftPattern(e.PatternId, ucOperating_Shift_Item2);
				SetShiftPattern(e.PatternId, ucOperating_Shift_Item3);

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// 実績生産数変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProductionQuantityChanged(object sender, EventArgs e)
		{
            try
            {
				DataTable editedTable1 = ucOperating_Shift_Item1.DtProductionQuantity;
				DataTable editedTable2 = ucOperating_Shift_Item2.DtProductionQuantity;
				DataTable editedTable3 = ucOperating_Shift_Item3.DtProductionQuantity;

				if (editedTable1 != null && 0 < editedTable1.Rows.Count)
				{
					logger.Info("シフト1に実績生産数を適用します。");
					UpdateProductionQuantity(ucOperating_Shift_Item1.TargetDate, ucOperating_Shift_Item1.OperationShift, editedTable1);
				}

				if (editedTable2 != null && 0 < editedTable2.Rows.Count)
				{
					logger.Info("シフト2に実績生産数を適用します。");
					UpdateProductionQuantity(ucOperating_Shift_Item2.TargetDate, ucOperating_Shift_Item2.OperationShift, editedTable2);
				}

				if (editedTable3 != null && 0 < editedTable3.Rows.Count)
				{
					logger.Info("シフト3に実績生産数を適用します。");
					UpdateProductionQuantity(ucOperating_Shift_Item3.TargetDate, ucOperating_Shift_Item3.OperationShift, editedTable3);
				}
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

        /// <summary>
        /// 除外区分変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExclusionClassChanged(object sender, EventArgs e)
        {
            try
            {
				ucOperating_Shift_Item1.UpdateExclusionClassList();
				ucOperating_Shift_Item2.UpdateExclusionClassList();
				ucOperating_Shift_Item3.UpdateExclusionClassList();
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}
		#endregion "イベント"

		//********************************************
		//* パブリックメソッド
		//********************************************
		#region "パブリックメソッド"
		public void InitializeComponentData()
        {
			// 計画稼働シフトテーブルからの全件取得
			mDtPlanOperatingShiftTbl = mSqlPlanOperatingShiftTbl.Select();
			// 実績稼働シフトテーブルからの全件取得
			mDtResultOperatingShiftTbl = mSqlResultOperatingShiftTbl.Select();
			// 稼働シフト品番タイプ毎生産数テーブルからの全件取得
			mDtOperatingShiftProductionQuantityTbl = mSqlOperatingShiftProductionQuantityTbl.Select();
			// 稼働シフト除外時間テーブルからの全件取得
			mDtOperatingShiftExclusionTbl = mSqlOperatingShiftExclusionTbl.Select();
			// 稼働シフトパターンテーブルからの全件取得
			mDtSqlOperatingShiftTbl = mSqlOperatingShiftPatternTbl.Select();
			// 稼働除外パターンテーブルからの全件取得
			mDtOperatingShiftExclusionPatternTbl = mSqlOperatingShiftExclusionPatternTbl.Select();

			//ucCalendar.DaysFont =
			//	new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

			ucCalendar.TargetMonth = DateTime.Now;

			//ucOperating_Shift_Item1.OperationShift = 1;
			//ucOperating_Shift_Item2.OperationShift = 2;
			//ucOperating_Shift_Item3.OperationShift = 3;

			ucCalendar.SelectChanging -= UcCalendar_SelectChanging;
			ucCalendar.SelectChanging += UcCalendar_SelectChanging;
			ucCalendar.SelectChanged -= OnCalendarSelectChanged;
			ucCalendar.SelectChanged += OnCalendarSelectChanged;

            ucCalendar.NextManthChanged -= OnCalendarSelectManthChanged;
            ucCalendar.NextManthChanged += OnCalendarSelectManthChanged;

			ucCalendar.PrevMonthChanged -= OnCalendarSelectManthChanged;
			ucCalendar.PrevMonthChanged += OnCalendarSelectManthChanged;

			ucCalendar.SelectedDaysList = new List<DateTime>() { DateTime.Today, };

			ucOperating_Shift_Pattern.PatternRegist -= OnPatternRegist;
			ucOperating_Shift_Pattern.PatternRegist += OnPatternRegist;
			ucOperating_Shift_Pattern.PatternLoad -= OnPatternLoad;
			ucOperating_Shift_Pattern.PatternLoad += OnPatternLoad;

			ucOperating_Shift_Item1.ExclusionClassChanged -= OnExclusionClassChanged;
			ucOperating_Shift_Item1.ExclusionClassChanged += OnExclusionClassChanged;
			ucOperating_Shift_Item2.ExclusionClassChanged -= OnExclusionClassChanged;
			ucOperating_Shift_Item2.ExclusionClassChanged += OnExclusionClassChanged;
			ucOperating_Shift_Item3.ExclusionClassChanged -= OnExclusionClassChanged;
			ucOperating_Shift_Item3.ExclusionClassChanged += OnExclusionClassChanged;

			ucOperating_Shift_Item1.ProductionQuantityChanged -= OnProductionQuantityChanged;
			ucOperating_Shift_Item1.ProductionQuantityChanged += OnProductionQuantityChanged;
			ucOperating_Shift_Item2.ProductionQuantityChanged -= OnProductionQuantityChanged;
			ucOperating_Shift_Item2.ProductionQuantityChanged += OnProductionQuantityChanged;
			ucOperating_Shift_Item3.ProductionQuantityChanged -= OnProductionQuantityChanged;
			ucOperating_Shift_Item3.ProductionQuantityChanged += OnProductionQuantityChanged;

			//ucOperating_Shift_Calendar_Day_Main1.ExistPlan = false;
			//ucOperating_Shift_Calendar_Day_Main1.ExistActual = false;
			//ucOperating_Shift_Calendar_Day_Main1.DaysBackColor = System.Drawing.Color.LightGray;
			//ucOperating_Shift_Calendar_Day_Main1.DaysForeColor = System.Drawing.Color.Gray;
			//ucOperating_Shift_Calendar_Day_Main3.Image = global::PLOSMaintenance.Properties.Resources.StatusGood;

			ucOperating_Shift_Item1.Owner = this;
			ucOperating_Shift_Item2.Owner = this;
			ucOperating_Shift_Item3.Owner = this;

			ucOperating_Shift_Item1.DtOperatingShiftExclusionTbl = mDtOperatingShiftExclusionTbl;
			ucOperating_Shift_Item2.DtOperatingShiftExclusionTbl = mDtOperatingShiftExclusionTbl;
			ucOperating_Shift_Item3.DtOperatingShiftExclusionTbl = mDtOperatingShiftExclusionTbl;
		}
		#endregion "パブリックメソッド"

		//********************************************
		//* プライベートメソッド
		//********************************************
		#region "プライベートメソッド"
		/// <summary>
		/// シフトデータ設定処理
		/// 本クラス内で保持する更新用データテーブルから引数で指定された子コントロールにパラメータを引き渡す
		/// </summary>
		/// <param name="shiftItem">シフト入力コントロール</param>
		private void SetShiftData(UCOperating_Shift_Item shiftItem)
		{
			// 計画稼働シフト
			DataTable planOperatingShift = mDtPlanOperatingShiftTbl.Clone();
            int targetCnt = mDtPlanOperatingShiftTbl.AsEnumerable()
				.Where(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE) == shiftItem.TargetDate)
				.Where(x => x.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT) == shiftItem.OperationShift)
				.Count();
			if(targetCnt > 0)
            {
				 planOperatingShift = mDtPlanOperatingShiftTbl.AsEnumerable()
				.Where(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE) == shiftItem.TargetDate)
				.Where(x => x.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT) == shiftItem.OperationShift)
				.CopyToDataTable();
            }
			else
            {
				////mDtPlanOperatingShiftTbl.Rows.Add(planOperatingShift.NewRow());// データを追加
				//mDtPlanOperatingShiftTbl.Rows.Add(mDtPlanOperatingShiftTbl.NewRow());
				//DataRow addData = mDtPlanOperatingShiftTbl.Rows[mDtPlanOperatingShiftTbl.Rows.Count - 1];

				//addData[PlanOperatingShiftTblColumn.OPERATION_DATE] = shiftItem.TargetDate;
				//addData[PlanOperatingShiftTblColumn.OPERATION_SHIFT] = shiftItem.OperationShift;
				//addData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = 0;

				////DateTime startTime = (DateTime)srcData[PlanOperatingShiftTblColumn.START_TIME];
				////DateTime endTime = (DateTime)srcData[PlanOperatingShiftTblColumn.END_TIME];
				//addData[PlanOperatingShiftTblColumn.START_TIME] = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, 0, 0, 0);
				//addData[PlanOperatingShiftTblColumn.END_TIME] = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, 0, 0, 0);
				//addData[PlanOperatingShiftTblColumn.OPERATION_SECOND] = 0;
				//addData[PlanOperatingShiftTblColumn.USE_FLAG] = false;
			}
			shiftItem.PlanOperatingShift = planOperatingShift;
			// シフト入力有効/無効
			shiftItem.UseShift = targetCnt > 0 ? planOperatingShift.Rows[0].Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG) : false;

			// 実績稼働シフト
			DataTable actualOperatingShift = mDtResultOperatingShiftTbl.Clone();
			targetCnt = mDtResultOperatingShiftTbl.AsEnumerable()
				.Where(x => x.Field<DateTime>(ColumnResultOperatingShiftTbl.OPERATION_DATE) == shiftItem.TargetDate)
				.Where(x => x.Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
				.Count();
			if (targetCnt > 0)
			{
				actualOperatingShift = mDtResultOperatingShiftTbl.AsEnumerable()
					.Where(x => x.Field<DateTime>(ColumnResultOperatingShiftTbl.OPERATION_DATE) == shiftItem.TargetDate)
					.Where(x => x.Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
					.CopyToDataTable();
			}
			shiftItem.ActualOperatingShift = actualOperatingShift;

			// 2022.06.21 日付変更時に最新のサイクル実績を取り直す
			if(!shiftItem.DtQuantityList.Contains(shiftItem.TargetDate))
            {
				DataTable quantityData = mSqlOperatingShiftProductionQuantityTbl.Select(shiftItem.TargetDate, shiftItem.OperationShift);

				if(quantityData != null && 0 < quantityData.Rows.Count)
                {
					foreach (DataRow row in quantityData.Rows)
					{
						// 編成ID
						Guid compositionID = row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID);
						// 品番ID
						Guid productTypeID = row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID);

						// 一意のデータ取得
						int targetDataCount = mDtOperatingShiftProductionQuantityTbl.AsEnumerable()
											.Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == shiftItem.TargetDate)
											.Where(x => x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
											.Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID) == compositionID)
											.Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID) == productTypeID)
											.Count();

						if (0 < targetDataCount)
						{
							// 更新元データのインデックス
							int srcDataIdx = mDtOperatingShiftProductionQuantityTbl.Rows.IndexOf(
										mDtOperatingShiftProductionQuantityTbl.AsEnumerable()
										.Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == shiftItem.TargetDate)
										.Where(x => x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
										.Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID) == compositionID)
										.Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID) == productTypeID)
										.FirstOrDefault());

							// サイクル同期生産数を更新
							int updateCycle = row.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);
							mDtOperatingShiftProductionQuantityTbl.Rows[srcDataIdx][ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE] = updateCycle;
						}
					}
				}
			}

			// 実績生産数
			DataTable actualProductionQuantityData = mDtOperatingShiftProductionQuantityTbl.Clone(); 
			targetCnt = mDtOperatingShiftProductionQuantityTbl.AsEnumerable()
					.Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == shiftItem.TargetDate)
					.Where(x => x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
					.Count();

			if(targetCnt > 0)
            {
				 actualProductionQuantityData = mDtOperatingShiftProductionQuantityTbl.AsEnumerable()
					.Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == shiftItem.TargetDate)
					.Where(x => x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
					.CopyToDataTable();
			}

			// 合計の実績生産数を取得する
            int actualProductionQuantity = 0;
            if (actualProductionQuantityData != null && 0 < actualProductionQuantityData.Rows.Count)
            {
				Boolean isExistProductionQuantity = true;
				for(int i = 0; i< actualProductionQuantityData.Rows.Count; i++)
                {
					if(actualProductionQuantityData.Rows[i][ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] == DBNull.Value)
                    {
						isExistProductionQuantity = false;
						break;
                    }
				}
				// 合計実績が存在する
				if (isExistProductionQuantity)
				{
					actualProductionQuantity = actualProductionQuantityData.AsEnumerable().Sum(x => x.Field<Int32>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY));
				}
				else
				{
					actualProductionQuantity = actualProductionQuantityData.AsEnumerable().Sum(x => x.Field<Int32>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE));
				}
			}
			// 合計の実績生産数を保持させる
			shiftItem.DtProductionQuantity = actualProductionQuantityData;
            shiftItem.ActualProductionQuantity = actualProductionQuantity;


			// 稼働シフト除外時間データ
			//DataTable exclusionOperatingShift = mSqlOperatingShiftExclusionTbl.Select(shiftItem.TargetDate, shiftItem.OperationShift);
			DataTable exclusionOperatingShift = mDtOperatingShiftExclusionTbl.Clone();
			targetCnt = mDtOperatingShiftExclusionTbl.AsEnumerable()
					.Where(x => x.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE) == shiftItem.TargetDate)
					.Where(x => x.Field<int>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
					.Count();
			if (targetCnt > 0)
			{
				exclusionOperatingShift = mDtOperatingShiftExclusionTbl.AsEnumerable()
				   .Where(x => x.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE) == shiftItem.TargetDate)
				   .Where(x => x.Field<int>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
				   .CopyToDataTable();
			}

			shiftItem.ExclusionOperatingShift = exclusionOperatingShift;

			// 本日と選択日の比較で判断をかけさせる
			if(DateTime.Today < shiftItem.TargetDate)
            {
				// 未来の日付(本日 < 選択日)を選択
				// 実績操作×、計画操作○
				shiftItem.SelectDateStatus = SELECT_DATE_STATUS.FUTURE;
            }
			else if (DateTime.Today > shiftItem.TargetDate)
            {
				// 過去の日付(本日 > 選択日)を選択
				// 実績操作○、計画操作×、シフト選択×
				shiftItem.SelectDateStatus = SELECT_DATE_STATUS.PAST;
            }
            else if (DateTime.Today == shiftItem.TargetDate)
			{
				shiftItem.SelectDateStatus = SELECT_DATE_STATUS.TODAY;
			}
        }

		/// <summary>
		/// シフト設定更新処理
		/// 引数で指定された子コントロールからパラメータを引き出し、本クラス内で保持する更新用データテーブルにパラメータを保持する
		/// </summary>
		/// <param name="shiftItem">シフト入力コントロール</param>
		private bool UpdateShiftData(UCOperating_Shift_Item shiftItem)
        {
            #region <計画稼働シフト>
            // 計画稼働シフト
            DataTable dtPlan = shiftItem.PlanOperatingShift;

			int currentPlanDataIdx = mDtPlanOperatingShiftTbl.Rows.IndexOf(
										mDtPlanOperatingShiftTbl.AsEnumerable()
										.Where(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE) == shiftItem.TargetDate)
										.Where(x => x.Field<Int32>(PlanOperatingShiftTblColumn.OPERATION_SHIFT) == shiftItem.OperationShift)
										.FirstOrDefault());

			if (dtPlan != null && dtPlan.Rows.Count > 0)
			{
				dtPlan.Rows[0][PlanOperatingShiftTblColumn.USE_FLAG] = shiftItem.UseShift;
				if (!CheckPlan(dtPlan, shiftItem))
				{
					return false;
				}

				// 読み込んだデータに当該稼働日・稼働シフトがない⇒追加対応
				if (currentPlanDataIdx < 0)
				{
					if (dtPlan.Rows[0].Field<int>(PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY) > 0 && dtPlan.Rows[0].Field<int>(PlanOperatingShiftTblColumn.OPERATION_SECOND) > 0)
					{
						mDtPlanOperatingShiftTbl.ImportRow(dtPlan.Rows[0]);
					}
				}
				// 読み込んだデータに当該稼働日・稼働シフトがある⇒保持データ更新対応
				else
				{
					DataRow data = dtPlan.Rows[0];
					DataRow currentData = mDtPlanOperatingShiftTbl.Rows[currentPlanDataIdx];

					// 生産数
					int dataProductionQuantity = (int)data[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY];
					int currentDataProductionQuantity = (int)currentData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY];
					if (dataProductionQuantity != currentDataProductionQuantity)
					{
						currentData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = dataProductionQuantity;
					}

					// 開始時刻
					DateTime dataStartTime = (DateTime)data[PlanOperatingShiftTblColumn.START_TIME];
					DateTime currentDataStartTime = (DateTime)currentData[PlanOperatingShiftTblColumn.START_TIME];
					if (dataStartTime != currentDataStartTime)
					{
						currentData[PlanOperatingShiftTblColumn.START_TIME] = dataStartTime;
					}

					// 終了時刻
					DateTime dataEndTime = (DateTime)data[PlanOperatingShiftTblColumn.END_TIME];
					DateTime currentDataEndTime = (DateTime)currentData[PlanOperatingShiftTblColumn.END_TIME];
					if (dataEndTime != currentDataEndTime)
					{
						currentData[PlanOperatingShiftTblColumn.END_TIME] = dataEndTime;
					}

					// 稼働時間(秒)
					int dataOperationSecond = (int)data[PlanOperatingShiftTblColumn.OPERATION_SECOND];
					int currentDataOperationSecond = (int)currentData[PlanOperatingShiftTblColumn.OPERATION_SECOND];
					if (dataOperationSecond != currentDataOperationSecond)
					{
						currentData[PlanOperatingShiftTblColumn.OPERATION_SECOND] = dataOperationSecond;
					}
					currentData[PlanOperatingShiftTblColumn.USE_FLAG] = data[PlanOperatingShiftTblColumn.USE_FLAG];
				}
			}
			else
			{
				// Do Nothing
				logger.Info(String.Format("対象の計画データが存在しません。対象稼働日：{0}, 対象シフト：{1}", shiftItem.TargetDate, shiftItem.OperationShift));
			}
			#endregion <計画稼働シフト>



			#region <実績稼働シフト>
			//実績稼働シフト
			DataTable dtResult = shiftItem.ActualOperatingShift;

			int currentResultDataIdx = mDtResultOperatingShiftTbl.Rows.IndexOf(
										mDtResultOperatingShiftTbl.AsEnumerable()
										.Where(x => x.Field<DateTime>(ColumnResultOperatingShiftTbl.OPERATION_DATE) == shiftItem.TargetDate)
										.Where(x => x.Field<Int32>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
										.FirstOrDefault());

			if (dtResult != null && dtResult.Rows.Count > 0)
			{
				if (!CheckResult(dtResult, shiftItem))
				{
					return false;
				}

				// 読み込んだデータに当該稼働日・稼働シフトがない⇒追加対応
				if (currentResultDataIdx < 0)
				{
					mDtResultOperatingShiftTbl.ImportRow(dtResult.Rows[0]);
				}
				// 読み込んだデータに当該稼働日・稼働シフトがある⇒保持データ更新対応
				else
				{
					DataRow currentData = mDtResultOperatingShiftTbl.Rows[currentResultDataIdx];
					if (currentData.Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME) != dtResult.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME))
                    {
						currentData[ColumnResultOperatingShiftTbl.START_TIME] = dtResult.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME);
					}
					if (currentData.Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME) != dtResult.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME))
					{
						currentData[ColumnResultOperatingShiftTbl.END_TIME] = dtResult.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME);
					}
				}
			}
			else
			{
				logger.Info(String.Format("対象の実績データが存在しません。対象稼働日：{0}, 対象シフト：{1}", shiftItem.TargetDate, shiftItem.OperationShift));
			}
			#endregion <実績稼働シフト>

			// 実績生産数
			// MEMO: 画面を実績生産数ダイアログを閉じたイベントで本クラスが持つ実データテーブル(mDtOperatingShiftProductionQuantityTbl)は更新済



			#region <稼働シフト除外時間>
			//稼働シフト除外時間データ(当該日/シフトの12件分)
			DataTable dtExclusionShift = shiftItem.ExclusionOperatingShift;

			if (dtExclusionShift != null && dtExclusionShift.Rows.Count > 0)
            {
				if (!CheckExclusion(dtExclusionShift, shiftItem))
				{
					return false;
				}

				for (int i = 0; i < dtExclusionShift.Rows.Count; i++)
				{
					int exclusionIdx = dtExclusionShift.Rows[i].Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX);
					int currentExclusionDataIdx = mDtOperatingShiftExclusionTbl.Rows.IndexOf(
											mDtOperatingShiftExclusionTbl.AsEnumerable()
											.Where(x => x.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE) == shiftItem.TargetDate)
											.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT) == shiftItem.OperationShift)
											.Where(x => x.Field<Int32>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) == exclusionIdx)
											.FirstOrDefault());

					// 読み込んだデータに当該稼働日・稼働シフトがない⇒追加対応
					if (currentExclusionDataIdx < 0)
					{
						mDtOperatingShiftExclusionTbl.ImportRow(dtExclusionShift.Rows[i]);
						//if ( dtExclusionShift.Rows[i].Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK) > 0)
						//{
						//	mDtOperatingShiftExclusionTbl.ImportRow(dtExclusionShift.Rows[i]);
						//}
					}
					// 読み込んだデータに当該稼働日・稼働シフトがある⇒保持データ更新対応
					else
					{
						DataRow data = dtExclusionShift.Rows[i];
						DataRow currentData = mDtOperatingShiftExclusionTbl.Rows[currentExclusionDataIdx];

						// 除外開始時刻
						if(currentData.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME) != data.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME))
                        {
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = data.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME);
						}
						// 除外終了時刻
						if (currentData.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME) != data.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = data.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME);
						}
						// 除外インデックス
						if (currentData.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) != data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX] = data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX);
						}
						// 除外時間
						if (currentData.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME) != data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME] = data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME);
						}
						// 除外チェック有無
						if (currentData.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK) != data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK] = data.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK);
						}
						// 備考
						if (currentData.Field<String>(ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK) != data.Field<String>(ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK] = data.Field<String>(ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK);
						}
						// 除外区分ID
						if (currentData.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS) != data.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS))
						{
							currentData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS] = data.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS);
						}
					}
				}
            }
			else
			{
				logger.Info(String.Format("対象の除外データが存在しません。対象稼働日：{0}, 対象シフト：{1}", shiftItem.TargetDate, shiftItem.OperationShift));
			}
			#endregion <稼働シフト除外時間>
			return true;
        }

		/// <summary>
		/// 勤怠パターン設定処理
		/// </summary>
		/// <param name="shiftItem">シフト入力コントロール</param>
		private void SetShiftPattern(int patternId, UCOperating_Shift_Item shiftItem)
        {
			try
			{
				// 稼働シフトパターン
				DataTable shiftPattern = mSqlOperatingShiftPatternTbl.Select(patternId, shiftItem.OperationShift);
				if (shiftPattern != null && shiftPattern.Rows.Count > 0)
				{
					// 計画稼働シフト
					DataTable planOperatingShift = mDtPlanOperatingShiftTbl.Clone();
					planOperatingShift.Rows.Add(planOperatingShift.NewRow());
					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.OPERATION_DATE] = shiftItem.TargetDate;

					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.OPERATION_SHIFT] = shiftItem.OperationShift;
					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = shiftPattern.Rows[0].Field<int>(ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY);

					TimeSpan tempStart = shiftPattern.Rows[0].Field<TimeSpan>(ColumnOperatingShiftPatternTbl.START_TIME);
					DateTime startDate = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, tempStart.Hours, tempStart.Minutes, tempStart.Seconds);
                    if (shiftPattern.Rows[0].Field<bool>(ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG))
                    {
						startDate = startDate.AddDays(1);
					}
					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.START_TIME] = startDate;

					TimeSpan tempEnd = shiftPattern.Rows[0].Field<TimeSpan>(ColumnOperatingShiftPatternTbl.END_TIME);
					DateTime endDate = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, tempEnd.Hours, tempEnd.Minutes, tempEnd.Seconds);
					if (shiftPattern.Rows[0].Field<bool>(ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG))
					{
						endDate = endDate.AddDays(1);
					}
					else if (tempEnd < tempStart)
					{
						endDate = endDate.AddDays(1);
					}
					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.END_TIME] = endDate;

					planOperatingShift.Rows[0][PlanOperatingShiftTblColumn.OPERATION_SECOND] = shiftPattern.Rows[0].Field<int>(ColumnOperatingShiftPatternTbl.OPERATION_SECOND);

					shiftItem.PlanOperatingShift = planOperatingShift;
					// シフト使用設定を強制ON
					//shiftItem.UseShift = true;

					// 稼働除外パターン
					DataTable exclusionOperatingShiftPattern = mSqlOperatingShiftExclusionPatternTbl.Select(patternId, shiftItem.OperationShift);
					DataTable exclusionOperatingShift = mDtOperatingShiftExclusionTbl.Clone();

					for (int i = 0; i < exclusionOperatingShiftPattern.Rows.Count; i++)
					{

						exclusionOperatingShift.Rows.Add(exclusionOperatingShift.NewRow());
						TimeSpan tempExcStart = exclusionOperatingShiftPattern.Rows[i].Field<TimeSpan>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME);
						TimeSpan tempExcEnd = exclusionOperatingShiftPattern.Rows[i].Field<TimeSpan>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME);
						// 稼働日
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.OPERATION_DATE] = shiftItem.TargetDate;
						// 稼働シフト
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT] = shiftItem.OperationShift;
						// 稼働除外インデックス
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX] = i;

						// 除外開始時刻
						if (exclusionOperatingShiftPattern.Rows[i].Field<Boolean>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG))
						{
							exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = new DateTime(shiftItem.TargetDate.AddDays(1).Year, shiftItem.TargetDate.AddDays(1).Month, shiftItem.TargetDate.AddDays(1).Day, tempExcStart.Hours, tempExcStart.Minutes, tempExcStart.Seconds);
						}
						else
						{
							exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, tempExcStart.Hours, tempExcStart.Minutes, tempExcStart.Seconds);
						}

						// 除外終了時刻
						if (exclusionOperatingShiftPattern.Rows[i].Field<Boolean>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG))
						{
							exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = new DateTime(shiftItem.TargetDate.AddDays(1).Year, shiftItem.TargetDate.AddDays(1).Month, shiftItem.TargetDate.AddDays(1).Day, tempExcEnd.Hours, tempExcEnd.Minutes, tempExcEnd.Seconds);
						}
						else
						{
							exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = new DateTime(shiftItem.TargetDate.Year, shiftItem.TargetDate.Month, shiftItem.TargetDate.Day, tempExcEnd.Hours, tempExcEnd.Minutes, tempExcEnd.Seconds);
						}

						// 除外チェック有無
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK] = exclusionOperatingShiftPattern.Rows[i].Field<int>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK);

						// 備考
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK] = exclusionOperatingShiftPattern.Rows[i].Field<String>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK);

						// 除外区分ID
						exclusionOperatingShift.Rows[i][ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS] = exclusionOperatingShiftPattern.Rows[i].Field<Guid>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID);

					}
					shiftItem.ExclusionOperatingShift = exclusionOperatingShift;

					//DataTable exclusionShiftPattern = mSqlOperatingShiftExclusionPatternTbl.Select(patternId, shiftItem.OperationShift);
				}
				else
				{
					logger.Error("稼働シフトパターンデータがありません。");
				}
				//shiftItem.ExclusionOperatingShift = ;
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + ", " + ex.StackTrace);
            }
		}

        /// <summary>
		/// 実績生産数更新処理
		/// </summary>
		/// <param name="targetDate">選択日</param>
		/// <param name="operationShift">稼働シフト</param>
		/// <param name="editedList">変更後リスト</param>
		private void UpdateProductionQuantity(DateTime targetDate, int operationShift, DataTable editedDataTable)
        {
			//for(int i = 0; i < editedList.Count; i++)
			//         {
			//	DataTable editedDataTable = editedList[i];
			//}

			for (int j = 0; j < editedDataTable.Rows.Count; j++)
            {
				DataRow editedData = editedDataTable.Rows[j];
				Guid compositionId = editedData.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID);
				Guid productTypeId = editedData.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID);
				int editedProductionQuantity = 0;
				if (editedData[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] == DBNull.Value)
                {
					editedProductionQuantity = editedData.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);
				}
				else
				{
					editedProductionQuantity = editedData.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);
				}

				// インデックスを取得
				int currentDataIdx = mDtOperatingShiftProductionQuantityTbl.Rows.IndexOf(
								        mDtOperatingShiftProductionQuantityTbl.AsEnumerable().Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == targetDate)
									   		                   	                            .Where(x => x.Field<Int32>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == operationShift)
																					        .Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID) == compositionId)
																					        .Where(x => x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID) == productTypeId)																						 
																					        .FirstOrDefault()
																					        );

				if (currentDataIdx > -1)
                {
					DataRow currentData = mDtOperatingShiftProductionQuantityTbl.Rows[currentDataIdx];
					if (editedData[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] == DBNull.Value)
                    {
						int currentProductionQuantity = currentData.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);

						// 生産数が違う場合は更新
						//if (editedProductionQuantity != currentProductionQuantity)
						{
							currentData[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] = editedProductionQuantity;
						}
					}
					else
					{
						int currentProductionQuantity = currentData.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);

						// 生産数が違う場合は更新
						//if (editedProductionQuantity != currentProductionQuantity)
						{
							currentData[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] = editedProductionQuantity;
						}
					}
						
				}
                else
                {
					// この時点でデータがないこと自体が基本的にありえないため、エラーログを出力し、スルー
					logger.Error(String.Format("更新対象の実績生産数データが存在しません。対象日：{0}, 対象シフト：{1}, 対象編成ID：{2}, 対象品番タイプID：{3}", targetDate, operationShift, compositionId, productTypeId));
					// targetDate, operationShift, compositionId, productTypeIdをログ出力する

					//// データを追加
					//mDtOperatingShiftProductionQuantityTbl.Rows.Add(mDtOperatingShiftProductionQuantityTbl.NewRow());
					//DataRow addData = mDtOperatingShiftProductionQuantityTbl.Rows[mDtOperatingShiftProductionQuantityTbl.Rows.Count - 1];

					//addData[ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE] = targetDate;
					//addData[ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT] = operationShift;
					//addData[ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID] = compositionId;
					//addData[ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID] = productTypeId;
					//addData[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] = editedProductionQuantity;
				}
			}
		}

		/// <summary>
		/// 計画稼働シフトの値チェック
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		private bool CheckPlan(DataTable dt, UCOperating_Shift_Item shiftItem)
        {
			TimeSpan beginRange = new TimeSpan(0, 0, 0);
			TimeSpan endRange = new TimeSpan(23, 59, 59);

			foreach (DataRow row in dt.Rows)
            {
				Boolean useFlg = row.Field<Boolean?>(PlanOperatingShiftTblColumn.USE_FLAG) != null ? row.Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG) : false;
				if (!useFlg)
                {
					continue;
                }
				// 定時稼働時間
				int? operationSecond = row[PlanOperatingShiftTblColumn.OPERATION_SECOND] as int?;
                if (operationSecond == null)
                {
					logger.Warn("定時稼働時間の入力値が異常です。");
					MessageBox.Show("定時稼働時間の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
                {
					operationSecond = (int)operationSecond;
					if (DateTime.Today <= shiftItem.TargetDate && shiftItem.OperationSecondProp == "")
                    {
						logger.Warn("定時稼働時間が入力されていません。");
						MessageBox.Show("定時稼働時間が入力されていません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					if(operationSecond < 0 || 99999 < operationSecond)
                    {
						logger.Warn("定時稼働時間が範囲外です。定時稼働時間は0以上99999以下の数値のみ入力できます。");
						MessageBox.Show("定時稼働時間が範囲外です。\n定時稼働時間は0以上99999以下の数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
                }

				// 計画生産数
				int? productionQuantity = row[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] as int?;
				if (productionQuantity == null)
				{
					logger.Warn("計画生産数の入力値が異常です。");
					MessageBox.Show("計画生産数の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
				{
					productionQuantity = (int)productionQuantity;

					if (DateTime.Today <= shiftItem.TargetDate && shiftItem.PlanProductionQuantityProp == "")
					{
						logger.Warn("計画生産数が入力されていません。");
						MessageBox.Show("計画生産数が入力されていません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					if (productionQuantity < 0 || 99999 < productionQuantity)
					{
						logger.Warn("計画生産数が範囲外です。計画生産数は0以上99999以下の数値のみ入力できます。");
						MessageBox.Show("計画生産数が範囲外です。\n計画生産数は0以上99999以下の数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}

				// 計画稼働開始時刻
				DateTime? start = row[PlanOperatingShiftTblColumn.START_TIME] as DateTime?;
				if (start == null)
				{
					logger.Warn("計画稼働開始時刻の入力値が異常です。");
					MessageBox.Show("計画稼働開始時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
				{
					TimeSpan startTime = start.Value.TimeOfDay;

					if (startTime < beginRange || endRange < startTime)
					{
						logger.Warn("計画稼働開始時刻の入力値が範囲外です。計画稼働開始時刻は00:00から23:59までの数値のみ入力できます。");
						MessageBox.Show("計画稼働開始時刻の入力値が範囲外です。\n計画稼働開始時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}

				// 計画稼働終了時刻
				DateTime? end = row[PlanOperatingShiftTblColumn.END_TIME] as DateTime?;
				if (end == null)
				{
					logger.Warn("計画稼働終了時刻の入力値が異常です。");
					MessageBox.Show("計画稼働終了時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
				{
					TimeSpan endTime = end.Value.TimeOfDay;

					if (endTime < beginRange || endRange < endTime)
					{
						logger.Warn("計画稼働終了時刻の入力値が範囲外です。計画稼働終了時刻は00:00から23:59までの数値のみ入力できます。");
						MessageBox.Show("計画稼働終了時刻の入力値が範囲外です。\n計画稼働終了時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}

				if(start.Value >= end.Value)
                {
					logger.Warn("計画稼働終了時刻が計画稼働開始時刻より過去に設定されています。");
					MessageBox.Show("計画稼働終了時刻が計画稼働開始時刻より過去に設定されています。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
            }

			return true;
        }

		/// <summary>
		/// 実績稼働シフトの値チェック
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		private bool CheckResult(DataTable dt, UCOperating_Shift_Item shiftItem)
        {
			if (shiftItem.ActualOperatingShift != null && 0 < shiftItem.ActualOperatingShift.Rows.Count)
			{
				TimeSpan beginRange = new TimeSpan(0, 0, 0);
				TimeSpan endRange = new TimeSpan(23, 59, 0);

				foreach (DataRow row in dt.Rows)
				{
                    if (shiftItem.PlanOperatingShift != null && 0 < shiftItem.PlanOperatingShift.Rows.Count)
                    {
						Boolean useFlg = shiftItem.PlanOperatingShift.Rows[0].Field<Boolean?>(PlanOperatingShiftTblColumn.USE_FLAG) != null ?
									 shiftItem.PlanOperatingShift.Rows[0].Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG) : false;
						if (!useFlg)
						{
							continue;
						}
					}
                    else
                    {
						continue;
                    }

					// 実績稼働開始時刻
					DateTime? start = row[ColumnResultOperatingShiftTbl.START_TIME] as DateTime?;
					if (start == null)
					{
						logger.Warn("実績稼働開始時刻の入力値が異常です。");
						MessageBox.Show("実績稼働開始時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					else
					{
						TimeSpan startTime = start.Value.TimeOfDay;

						if (startTime < beginRange || endRange < startTime)
						{
							logger.Warn("実績稼働開始時刻が範囲外です。実績稼働開始時刻は00:00から23:59までの数値のみ入力できます。");
							MessageBox.Show("実績稼働開始時刻が範囲外です。\n実績稼働開始時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}

					// 実績稼働終了時刻
					DateTime? end = row[ColumnResultOperatingShiftTbl.END_TIME] as DateTime?;
					if (end == null)
					{
						logger.Warn("実績稼働終了時刻の入力値が異常です。");
						MessageBox.Show("実績稼働終了時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					else
					{
						TimeSpan endTime = end.Value.TimeOfDay;

						if (endTime < beginRange || endRange < endTime)
						{
							logger.Warn("実績稼働終了時刻が範囲外です。実績稼働終了時刻は00:00から23:59までの数値のみ入力できます。");
							MessageBox.Show("実績稼働終了時刻が範囲外です。\n実績稼働終了時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}
					
					if (shiftItem.TlpActualWorkTime && start.Value >= end.Value)
					{
						logger.Warn("実績稼働終了時刻が実績稼働開始時刻より過去に設定されています。");
						MessageBox.Show("実績稼働終了時刻が実績稼働開始時刻より過去に設定されています。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 除外区分の値チェック
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		private bool CheckExclusion(DataTable dt, UCOperating_Shift_Item shiftItem)
        {
			TimeSpan beginRange = new TimeSpan(0, 0, 0);
			TimeSpan endRange = new TimeSpan(23, 59, 0);

			foreach (DataRow item in dt.Rows)
			{
				if (shiftItem.PlanOperatingShift != null && 0 < shiftItem.PlanOperatingShift.Rows.Count)
				{
					Boolean useFlg = shiftItem.PlanOperatingShift.Rows[0].Field<Boolean?>(PlanOperatingShiftTblColumn.USE_FLAG) != null ?
								 shiftItem.PlanOperatingShift.Rows[0].Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG) : false;
					if (!useFlg)
					{
						continue;
					}
				}
				else
				{
					continue;
				}

				// 除外チェックがついている場合、判定の実施
				if (item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK) > 0)
				{
					// 除外区分開始時刻
					DateTime? start = item[ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] as DateTime?;
					if (start == null)
					{
						logger.Warn("除外区分開始時刻の入力値が異常です。");
						MessageBox.Show("除外区分開始時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					else
					{
						TimeSpan startTime = start.Value.TimeOfDay;

						if (startTime < beginRange || endRange < startTime)
						{
							logger.Warn("除外区分開始時刻が範囲外です。除外区分開始時刻は00:00から23:59までの数値のみ入力できます。");
							MessageBox.Show("除外区分開始時刻が範囲外です。\n除外区分開始時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}

					// 除外区分終了時刻
					DateTime? end = item[ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] as DateTime?;
					if (end == null)
					{
						logger.Warn("除外区分終了時刻の入力値が異常です。");
						MessageBox.Show("除外区分終了時刻の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					else
					{
						TimeSpan endTime = end.Value.TimeOfDay;

						if (endTime < beginRange || endRange < endTime)
						{
							logger.Warn("除外区分終了時刻が範囲外です。除外区分終了時刻は00:00から23:59までの数値のみ入力できます。");
							MessageBox.Show("除外区分終了時刻が範囲外です。\n除外区分終了時刻は00:00から23:59までの数値のみ入力できます。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}

					// 除外時間が0以下の場合、エラーとして戻させる
					if (item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME) <= 0)
					{
						logger.Warn(String.Format("除外終了時刻が除外開始時刻より前に設定されています。対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
						MessageBox.Show(String.Format("除外終了時刻が除外開始時刻より前に設定されています。\n対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}

					int a = shiftItem.PlanOperatingShift.Rows.Count;
					int b = shiftItem.ActualOperatingShift.Rows.Count;
					// 実績登録がない
					if(!shiftItem.TlpActualWorkTime)
					{
						// 除外開始時刻が計画の稼働開始時刻より早い場合、エラーとする
						if (start.Value < shiftItem.PlanOperatingShift.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME))
						{
							logger.Warn(String.Format("除外開始時刻が計画の稼働開始時刻より前に設定されています。対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
							MessageBox.Show(String.Format("除外開始時刻が計画の稼働開始時刻より前に設定されています。\n対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
						// 除外終了時刻が計画の稼働終了時刻より遅い場合、エラーとする
						if (shiftItem.PlanOperatingShift.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME) < end.Value)
						{
							logger.Warn(String.Format("除外終了時刻が計画の稼働終了時刻より後に設定されています。対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
							MessageBox.Show(String.Format("除外終了時刻が計画の稼働終了時刻より後に設定されています。\n対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}
					// 実績登録がある
					else
					{
						// 除外開始時刻が実績の稼働開始時刻より早い場合、エラーとする
						if (start.Value < shiftItem.ActualOperatingShift.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME))
						{
							logger.Warn(String.Format("除外開始時刻が実績の稼働開始時刻より前に設定されています。対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
							MessageBox.Show(String.Format("除外開始時刻が実績の稼働開始時刻より前に設定されています。\n対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
						// 除外終了時刻が実績の稼働終了時刻より遅い場合、エラーとする
						if (shiftItem.ActualOperatingShift.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME) < end.Value)
						{
							logger.Warn(String.Format("除外終了時刻が実績の稼働終了時刻より後に設定されています。対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
							MessageBox.Show(String.Format("除外終了時刻が実績の稼働終了時刻より後に設定されています。\n対象除外設定番号：{0}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}

					foreach (DataRow itemTmp in shiftItem.ExclusionOperatingShift.Rows)
					{
						if (itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK) > 0)
						{
							// 上で見ているインデックスはとばす
							if (item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) == itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX))
							{
								continue;
							}
							// 他の除外開始時間 < 当該除外開始時間 < 他の除外終了時間　はNGとして返す
							if (itemTmp.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME) < item.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME)
								&& item.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME) < itemTmp.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME))
							{
								logger.Warn(String.Format("除外開始時刻が他の除外時刻と重複しています。対象除外設定番号：{0}、重複除外設定番号：{1}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1, itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
								MessageBox.Show(String.Format("除外開始時刻が他の除外時刻と重複しています。\n対象除外設定番号：{0}\n重複除外設定番号：{1}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1, itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return false;
							}
							// 他の除外開始時間 < 当該除外開始時間 < 他の除外終了時間　はNGとして返す
							if (itemTmp.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME) < item.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME)
								&& item.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME) < itemTmp.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME))
							{
								logger.Warn(String.Format("除外終了時刻が他の除外時刻と重複しています。対象除外設定番号：{0}、重複除外設定番号：{1}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1, itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1));
								MessageBox.Show(String.Format("除外終了時刻が他の除外時刻と重複しています。\n対象除外設定番号：{0}\n重複除外設定番号：{1}", item.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1, itemTmp.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX) + 1), "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return false;
							}
						}
					}
				}
			}

			return true;
        }

		/// <summary>
		/// シフト間の計画・実績領域の時刻を比較
		/// </summary>
		/// <param name="shiftItem1"></param>
		/// <param name="shiftItem2"></param>
		/// <param name="shiftItem3"></param>
		/// <returns></returns>
		private bool CheckShiftDate(UCOperating_Shift_Item shiftItem1, UCOperating_Shift_Item shiftItem2)
        {
			// 計画稼働シフトデータ
			DataTable dtPlanShift1 = shiftItem1.PlanOperatingShift;
			DataTable dtPlanShift2 = shiftItem2.PlanOperatingShift;

			// データの有無を確認
			if (dtPlanShift1 != null && 0 < dtPlanShift1.Rows.Count && dtPlanShift2 != null && 0 < dtPlanShift2.Rows.Count)
            {
				// シフト使用フラグの確認
				if (dtPlanShift1.Rows[0].Field<bool>(PlanOperatingShiftTblColumn.USE_FLAG) && dtPlanShift2.Rows[0].Field<bool>(PlanOperatingShiftTblColumn.USE_FLAG))
				{
					// 右のシフトの計画稼働開始時刻の方が早い場合
					if(dtPlanShift2.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME) <= dtPlanShift1.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME))
					{
						logger.Warn($"シフト間の計画稼働開始時刻に異常があります。シフト{dtPlanShift2.Rows[0].Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。");
						MessageBox.Show($"シフト間の計画稼働開始時刻に異常があります。\nシフト{dtPlanShift2.Rows[0].Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。",
							"エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}

					// 右のシフトの計画稼働終了時刻の方が早い場合
					if (dtPlanShift2.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME) <= dtPlanShift1.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME))
                    {
						logger.Warn($"シフト間の計画稼働終了時刻に異常があります。シフト{dtPlanShift2.Rows[0].Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。");
						MessageBox.Show($"シフト間の計画稼働終了時刻に異常があります。\nシフト{dtPlanShift2.Rows[0].Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。",
							"エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}

					// 左のシフトの計画稼働終了時刻と右のシフトの計画稼働開始時刻が被っている場合
					if (dtPlanShift2.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME) <= dtPlanShift1.Rows[0].Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME))
					{
						logger.Warn("シフト間の計画稼働時刻が重なっています。シフト間で時刻が重ならないように入力してください。");
						MessageBox.Show("シフト間の計画稼働時刻が重なっています。\nシフト間で時刻が重ならないように入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}

                    // 実績稼働シフトデータ
					// 比較対象の実績がどちらも有効であればチェック
                    if (shiftItem1.TlpActualWorkTime && shiftItem2.TlpActualWorkTime)
                    {
						DataTable dtResultShift1 = shiftItem1.ActualOperatingShift;
						DataTable dtResultShift2 = shiftItem2.ActualOperatingShift;
						// 右のシフトの実績稼働開始時刻の方が早い場合
						if (dtResultShift2.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME) <= dtResultShift1.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME))
						{
							logger.Warn($"シフト間の実績稼働開始時刻に異常があります。シフト{dtResultShift2.Rows[0].Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。");
							MessageBox.Show($"シフト間の実績稼働開始時刻に異常があります。\nシフト{dtResultShift2.Rows[0].Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。",
								"エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}

						// 右のシフトの実績稼働終了時刻の方が早い場合
						if (dtResultShift2.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME) <= dtResultShift1.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME))
						{
							logger.Warn($"シフト間の実績稼働終了時刻に異常があります。シフト{dtResultShift2.Rows[0].Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。");
							MessageBox.Show($"シフト間の実績稼働終了時刻に異常があります。\nシフト{dtResultShift2.Rows[0].Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT)}の方が遅い時刻になるように入力してください。",
								"エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}

						// 左のシフトの実績稼働終了時刻と右のシフトの実績稼働開始時刻が被っている場合
						if (dtResultShift2.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME) <= dtResultShift1.Rows[0].Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME))
						{
							logger.Warn("シフト間の実績稼働時刻が重なっています。シフト間で時刻が重ならないように入力してください。");
							MessageBox.Show("シフト間の実績稼働時刻が重なっています。シフト間で時刻が重ならないように入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}
				}
			}

			return true;
		}
		#endregion "プライベートメソッド"
	}
}
