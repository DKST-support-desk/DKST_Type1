using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using DBConnect;

namespace PLOSEventControllerSub
{
    public class PLOSEventManager : IDisposable
	{
		private class CycleTimeTareget
		{
			public Guid CompositionId { get; private set; }
			public int ProcessIdx { get; private set; }
			public DateTime EventTime { get; private set; }
			public int IsProductionQuantityCounter { get; private set; }
			public Decimal IntervalFilter { get; private set; }
			public CycleTimeTareget(Guid compositionId, int processIdx, DateTime eventTime, int isProductionQuantityCounter, Decimal intervalFilter)
			{
				CompositionId = compositionId;
				ProcessIdx = processIdx;
				EventTime = eventTime;
				IsProductionQuantityCounter = isProductionQuantityCounter;
				IntervalFilter = intervalFilter;
			}
		}

		#region Property
		public String ConnectionString { get; set; }

		public Uri VideoRetrievalWCFAddress { get; set; }
		public Uri DioServiceWCFAddress { get; set; }
		public Uri WebcamServiceWCFAddress { get; set; }
		public int QROKPortNo { get; set; }
		public int QROKDelayMs { get; set; }
		public int QRNGPortNo { get; set; }
		public int QRNGDelayMs { get; set; }
		public int QRAssignPortDelayMs { get; set; }

		public int CycleVideoMarginMinutes { get; set; }
		public int RawVideoSecond { get; set; }

		public String CycleVideoPath { get; set; }

		public String RowVideoPath { get; set; }

		public double MonitoringTimerInterval 
		{
			get { return _MonitoringTimer.Interval; }
			set { _MonitoringTimer.Interval = value; }
		}
		private System.Timers.Timer _MonitoringTimer = new System.Timers.Timer();
		private Boolean _BreakMonitoring = false;

		#endregion

		#region Constructors and Destructor
		public PLOSEventManager()
		{
			_MonitoringTimer.Elapsed += OnMonitoringTimerElapsed;
			//_MonitoringTimer.Interval = Properties.Settings.Default.MonitoringTimerInterval;
			_MonitoringTimer.AutoReset = false;

		}

		#endregion

		#region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMonitoringTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Stop Timer
			_MonitoringTimer.Stop();

			MonitoringElapsed();

			if (!_BreakMonitoring)
			{
				// (Re)Start Timer
				_MonitoringTimer.Start();
			}
		}

		#endregion

		///////////////////////////////////////////
		///
		#region HandleDIEvent

		public void HandleDIEvent()
		{
			SqlDBConnector dbConnector = new SqlDBConnector(ConnectionString);
			VideoRetrievalContWCFSender videoSender = new VideoRetrievalContWCFSender(VideoRetrievalWCFAddress);

			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				List<CycleTimeTareget> CycleTimeTaregetList = new List<CycleTimeTareget>();

				// Event_TblからEventType = 0: DIO & 処理状態(ProcessingState) = 0:未処理 のレコードをすべて処理
				// DeviceId を条件に入れることが可能だが、取りこぼし対策として未処理のデータを全て処理する
				DataTable dtUnProcessingEvent = QueryUnProcessingEventTbl(dbConnector, 0);

				foreach (var eventItem in dtUnProcessingEvent.AsEnumerable())
				{
					Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] DI入力読込 {eventItem.Field<DateTime>("EventTime")} {eventItem.Field<Guid>("DeviceId")} {eventItem.Field<Int32>("EventDetail")}");
				}

				// CompositionResults_Tblから編成ID(CompositionId)を取得
				Guid CompositionId = GetCompositionId(dbConnector);

				// 現状の品番タイプに紐づいたトリガーセンサーを取得
				DataTable dtCurrentTriggerSensor = QueryCurrentTriggerSensor(dbConnector, CompositionId);

				// Step2 現状の品番タイプに紐づいたトリガーセンサー に指定されたDeviceIdと
				// 同一の状態変更ありのEvent_Tblデータが存在するか?
				var ChangedTriggerDeviceList =
					(from TriggerSensorItem in dtCurrentTriggerSensor.AsEnumerable()
					 join EventItem in dtUnProcessingEvent.AsEnumerable()
						 on TriggerSensorItem.Field<Guid>("DeviceId") equals EventItem.Field<Guid>("DeviceId")
					 select new
					 {
						 DeviceId = EventItem.Field<Guid>("DeviceId"),
						 EventTime = EventItem.Field<DateTime>("EventTime"),
						 EventTimeSub = EventItem.Field<Int32>("EventTimeSub"),
						 EventDetail = EventItem.Field<Int32>("EventDetail"),
						 ProcessIdx = TriggerSensorItem.Field<Int32>("ProcessIdx"),
						 SensorChatteringFilter = TriggerSensorItem.Field<Decimal>("SensorChatteringFilter"),
						 TriggerType = TriggerSensorItem.Field<Int32>("TriggerType"),
						 IsProductionQuantityCounter = TriggerSensorItem.Field<Int32>("IsProductionQuantityCounter"),
						 IntervalFilter = TriggerSensorItem.Field<Decimal>("IntervalFilter"),
					 }).Where(x => 
						!IsChatteringData(
							dbConnector
							, x.DeviceId
							, x.SensorChatteringFilter
							, x.EventTime
							, x.EventTimeSub
							, x.EventDetail)).OrderBy(x => x.EventTime).ThenBy(x => x.EventTimeSub);

				foreach (var triggerDeviceItem in ChangedTriggerDeviceList.AsEnumerable())
				{
					switch (triggerDeviceItem.TriggerType)
					{
						case 1:
							// ONエッジ
							if (triggerDeviceItem.EventDetail == 1)
							{
								CycleTimeTaregetList.Add(
									new CycleTimeTareget(
											CompositionId,
											triggerDeviceItem.ProcessIdx,
											triggerDeviceItem.EventTime,
											triggerDeviceItem.IsProductionQuantityCounter,
											triggerDeviceItem.IntervalFilter));
							}
							break;
						case 2:
							// AND
							if(triggerDeviceItem.EventDetail == 1)
							{
								// 対デバイスの過去データを検索して、ONの場合は処理継続、OFFの場合は条件適合外として破棄
								var PairSensorItem =
									(
									from sensorItem in dtCurrentTriggerSensor.AsEnumerable()
									where sensorItem.Field<Int32>("ProcessIdx") == triggerDeviceItem.ProcessIdx
										&& sensorItem.Field<Guid>("DeviceId") != triggerDeviceItem.DeviceId
									select sensorItem
									).FirstOrDefault();

								if (PairSensorItem != null)
								{
									// 最新の対デバイスを検索
									var PreviousPairSensorEvent = QueryPreviousEventTbl(
														dbConnector, 0, triggerDeviceItem.EventTime
														, triggerDeviceItem.EventTimeSub
														, (Guid)PairSensorItem.Field<Guid>("DeviceId"));

									if(PreviousPairSensorEvent != null
									   &&
									   (PreviousPairSensorEvent.Field<Int32>("EventDetail") == 1)
									)
									{
										CycleTimeTaregetList.Add(
											new CycleTimeTareget(
													CompositionId,
													triggerDeviceItem.ProcessIdx,
													triggerDeviceItem.EventTime,
													triggerDeviceItem.IsProductionQuantityCounter,
													triggerDeviceItem.IntervalFilter));
									}
								}
							}
							break;
						case 3:
							// ONメモリ
							// 第1(保持)デバイスの過去データと第2(トリガ)バイスの過去データを検索,
							// 第1(保持)デバイスのONの発生 以降に 第2(トリガ)デバイスのONが存在しない場合、条件適合
							if (triggerDeviceItem.EventDetail == 1)
							{
								// 第1(保持)デバイス
								var HoldSensorItem =
									(
									from sensorItem in dtCurrentTriggerSensor.AsEnumerable()
									where sensorItem.Field<Int32>("ProcessIdx") == triggerDeviceItem.ProcessIdx
									 && sensorItem.Field<Guid>("DeviceId") != triggerDeviceItem.DeviceId
									 && sensorItem.Field<Int32>("SensorIdx") == 0
									select sensorItem
									).FirstOrDefault();

								// 第2(トリガ)デバイス
								var TriggerSensorItem =
									(
									from sensorItem in dtCurrentTriggerSensor.AsEnumerable()
									where sensorItem.Field<Int32>("ProcessIdx") == triggerDeviceItem.ProcessIdx
										&& sensorItem.Field<Guid>("DeviceId") == triggerDeviceItem.DeviceId
										&& sensorItem.Field<Int32>("SensorIdx") == 1
									select sensorItem
									).FirstOrDefault();

								if (TriggerSensorItem != null && HoldSensorItem != null)
								{
									// 第1(保持)デバイスのONを検索
									var PreviousHoldSensorEvent = QueryPreviousEventTbl(
														dbConnector, 0, triggerDeviceItem.EventTime
														, triggerDeviceItem.EventTimeSub, 1
														, (Guid)HoldSensorItem.Field<Guid>("DeviceId"));

									// 第2(トリガ)デバイスのONを検索
									var PreviousTriggerSensorEvent = QueryPreviousEventTbl(
														dbConnector, 0, triggerDeviceItem.EventTime
														, triggerDeviceItem.EventTimeSub, 1
														, (Guid)TriggerSensorItem.Field<Guid>("DeviceId"));

									if (
									    (
									     PreviousHoldSensorEvent != null && PreviousTriggerSensorEvent != null
										 &&
										 (
										  (
										   (DateTime)PreviousHoldSensorEvent.Field<DateTime>("EventTime") >
										   (DateTime)PreviousTriggerSensorEvent.Field<DateTime>("EventTime")
										  )
										  ||
										  (
										   (DateTime)PreviousHoldSensorEvent.Field<DateTime>("EventTime") == 
										   (DateTime)PreviousTriggerSensorEvent.Field<DateTime>("EventTime")
										   && 
										   (int)PreviousHoldSensorEvent.Field<Int32>("EventTimeSub") >
										   (int)PreviousTriggerSensorEvent.Field<Int32>("EventTimeSub")
										  )
										 )
										)
										||
										PreviousHoldSensorEvent != null && PreviousTriggerSensorEvent == null
									)
									{
										CycleTimeTaregetList.Add(
												new CycleTimeTareget(
														CompositionId,
														triggerDeviceItem.ProcessIdx,
														triggerDeviceItem.EventTime,
														triggerDeviceItem.IsProductionQuantityCounter,
														triggerDeviceItem.IntervalFilter));
									}
								}
							}
							break;
						case 4:
							// OFFエッジ
							if (triggerDeviceItem.EventDetail == 0)
							{
								CycleTimeTaregetList.Add(
									new CycleTimeTareget(
											CompositionId,
											triggerDeviceItem.ProcessIdx,
											triggerDeviceItem.EventTime,
											triggerDeviceItem.IsProductionQuantityCounter,
											triggerDeviceItem.IntervalFilter));
							}
							break;
						case 5:
							// OR
							if (triggerDeviceItem.EventDetail == 1)
							{
								var PairSensorItem =
									(
									from sensorItem in dtCurrentTriggerSensor.AsEnumerable()
									where sensorItem.Field<Int32>("ProcessIdx") == triggerDeviceItem.ProcessIdx
										&& sensorItem.Field<Guid>("DeviceId") != triggerDeviceItem.DeviceId
									select sensorItem
									).FirstOrDefault();

								if (PairSensorItem != null)
								{
									// 最新の対デバイスを検索
									var PreviousPairSensorEvent = QueryPreviousEventTbl(
														dbConnector, 0, triggerDeviceItem.EventTime
														, triggerDeviceItem.EventTimeSub
														, (Guid)PairSensorItem.Field<Guid>("DeviceId"));

									if ((PreviousPairSensorEvent?.Field<Int32>("EventDetail") ?? 0) == 0)
									{
										CycleTimeTaregetList.Add(
											new CycleTimeTareget(
													CompositionId,
													triggerDeviceItem.ProcessIdx,
													triggerDeviceItem.EventTime,
													triggerDeviceItem.IsProductionQuantityCounter,
													triggerDeviceItem.IntervalFilter));
									}
								}
							}
							break;
					}
				}

				foreach (var cycleTimeTargetItem in CycleTimeTaregetList)
				{
					// Step2 トリガ間フィルタ(秒)対応
					var dtIntervalRange = QueryRangeCycleTimeResultsTbl(
							dbConnector, cycleTimeTargetItem.CompositionId, cycleTimeTargetItem.ProcessIdx,
							cycleTimeTargetItem.EventTime.AddSeconds(-(double)cycleTimeTargetItem.IntervalFilter),
							cycleTimeTargetItem.EventTime);
					// Step2トリガ動作でのトリガ間隔フィルタを確認
					if (dtIntervalRange.Rows.Count == 0)
					{
						FinishPreviousCycle(
							dbConnector,
							CompositionId,
							cycleTimeTargetItem.ProcessIdx,
							cycleTimeTargetItem.EventTime);

						StartNewCycle(
							dbConnector,
							CompositionId,
							cycleTimeTargetItem.ProcessIdx,
							cycleTimeTargetItem.EventTime);
					}
				}

				// Event_Tblの処理が終わったレコードは処理状態 = 2:処理済へ変更
				foreach (var eventItem in dtUnProcessingEvent.AsEnumerable())
				{
					UpdateEventTblStatus(
						dbConnector,
						eventItem.Field<DateTime>("EventTime"),
						eventItem.Field<Guid>("DeviceId"),
						eventItem.Field<Int32>("EventTimeSub"),
						2);
				}

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}
		}
		#endregion

		#region HandleQREvent

		/// <summary>
		/// QRコード読込イベント
		/// </summary>
		public void HandleQREvent()
		{
			DioServiceContWCFSender dioSender = new DioServiceContWCFSender(DioServiceWCFAddress);
			dioSender.QROKPortNo = QROKPortNo;
			dioSender.QROKDelayMs = QROKDelayMs;
			dioSender.QRNGPortNo = QRNGPortNo;
			dioSender.QRNGDelayMs = QRNGDelayMs;
			dioSender.QRAssignPortDelayMs = QRAssignPortDelayMs;
			WebcamServiceContWCFSender webcamSender = new WebcamServiceContWCFSender(WebcamServiceWCFAddress);

			SqlDBConnector dbConnector = new SqlDBConnector(ConnectionString);
			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				// Event_TblからEventType = 1: QR & 処理状態(ProcessingState) = 0:未処理 のレコードをすべて処理
				// DeviceId を条件に入れることが可能だが、取りこぼし対策として未処理のデータを全て処理する
				DataTable dtEventTbl = QueryUnProcessingEventTbl(dbConnector, 1);

				// CompositionResults_Tblから編成ID(CompositionId)を取得
				Guid CompositionId = GetCompositionId(dbConnector);

				// Event_TblからEventType = 1: QR & 処理状態 = 0:未処理 のレコードをすべて処理
				foreach (var eventItem in dtEventTbl.AsEnumerable())
				{
					//if (CompositionId != Guid.Empty)
					{
						// 直間時はQR入力対応の処理は行わない

						String qrCode = eventItem.Field<String>("EventData");
						Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR読込 {qrCode}");
						// QR種別(区切)コード1(区切)コード2(区切)
						// 99,00000000-0000-0000-0000-000000000000,99,XXXXXXXXX
						string[] qrCodeArray = qrCode.Split(','); //カンマ区切りで配列に入れる
						int iQRType;
						int iProcessIdx;

						if (qrCodeArray.Length >= 3 && qrCodeArray[0].Length == 2 
						  && qrCodeArray[2].Length == 2 && int.TryParse(qrCodeArray[0], out iQRType))
						{
							Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別={iQRType}");
							Guid GuidCode1;
							try
							{
								GuidCode1 = new Guid(qrCodeArray[1]);
							}
							catch (Exception ex)
							{
								Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] Guidコード異常");

								UpdateEventTblStatus(
									dbConnector,
									eventItem.Field<DateTime>("EventTime"),
									eventItem.Field<Guid>("DeviceId"),
									eventItem.Field<Int32>("EventTimeSub"),
									2);
								return;
							}

							// QR種別を判定
							switch (iQRType)
							{
								case 1:
									// 1:編成変更
									if (ValidateComposition(dbConnector, GuidCode1))
									{
										Guid LastCompositionId = GetCompositionId(dbConnector);
										DataTable dtProcess = QueryCompositionProcessMst(dbConnector, LastCompositionId);

										foreach (var processItem in dtProcess.AsEnumerable())
										{
											// サイクルを完了
											FinishPreviousCycle(
												dbConnector,
												LastCompositionId,
												processItem.Field<Int32>("ProcessIdx"),
												eventItem.Field<DateTime>("EventTime"));
										}

										// 編成変更
										InsertCompositionResults(dbConnector, GuidCode1, eventItem.Field<DateTime>("EventTime"));

										// Step2 品番タイプ変更 品番引継
										RestoreProductTypeResults(dbConnector
											, eventItem.Field<DateTime>("EventTime")
											, eventItem.Field<DateTime>("EventTime"));

										CompositionId = GetCompositionId(dbConnector);
										dtProcess = QueryCompositionProcessMst(dbConnector, CompositionId, true);

										foreach (var processItem in dtProcess.AsEnumerable())
										{
											StartNewCycle(
												dbConnector,
												CompositionId,
												processItem.Field<Int32>("ProcessIdx"),
												eventItem.Field<DateTime>("EventTime"));
										}

										// Step2 アクティブカメラ変更
										ChangeCameraActive(dbConnector, GuidCode1);

										// Step2 対応済 作業者終了
										FinishWorkerResults(dbConnector, eventItem.Field<DateTime>("EventTime"));
										// Step2 対応済 作業者"未設定"(という名前の作業者)を必要工程に割当てる
										ShiftChangedWorkerResults(dbConnector, eventItem.Field<DateTime>("EventTime"));

										dioSender.QROKDO();
									}
									else
									{
										Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 編成変更 編成コード データ異常 / {GuidCode1}");
										dioSender.QRNGDO();
									}
									break;
								case 2:
									// 2:品番タイプ変更
									//		コード1 = 品番タイプID
									//		コード2 = 工程インデックス
									//		コード2が"**"の場合は全工程への展開とする
									if (ValidateProductType(
											dbConnector,
											GuidCode1))
									{
										if (int.TryParse(qrCodeArray[2], out iProcessIdx))
										{
											if (ValidateCompositionProcess(
													dbConnector,
													GetCompositionId(dbConnector), 
													iProcessIdx))
											{
												FinishPreviousCycle(
													dbConnector,
													GetCompositionId(dbConnector),
													iProcessIdx,
													eventItem.Field<DateTime>("EventTime"));

												InsertProductTypeResults(
													dbConnector,
													GuidCode1,
													GetCompositionId(dbConnector),
													iProcessIdx,
													eventItem.Field<DateTime>("EventTime"));

												StartNewCycle(
													dbConnector,
													CompositionId,
													iProcessIdx,
													eventItem.Field<DateTime>("EventTime"));

												dioSender.RequestDO(
													GetDOPort(dbConnector,
														GuidCode1,
														GetCompositionId(dbConnector),
														iProcessIdx),
													QRAssignPortDelayMs);
											}
											else
											{
												Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 品番変更 工程 データ異常 / {GuidCode1}");
												dioSender.QRNGDO();
											}
										}
										else if (qrCodeArray[2] == "**")
										{
											// 品番切替(一括)
											DataTable dtProcess = QueryCompositionProcessMst(dbConnector, CompositionId);

											foreach (var processItem in dtProcess.AsEnumerable())
											{
												// サイクルを完了
												FinishPreviousCycle(
													dbConnector,
													GetCompositionId(dbConnector),
													processItem.Field<Int32>("ProcessIdx"),
													eventItem.Field<DateTime>("EventTime"));

												InsertProductTypeResults(
													dbConnector,
													GuidCode1,
													GetCompositionId(dbConnector),
													processItem.Field<Int32>("ProcessIdx"),
													eventItem.Field<DateTime>("EventTime"));
											}

											dtProcess = QueryCompositionProcessMst(dbConnector, CompositionId, true);

											foreach (var processItem in dtProcess.AsEnumerable())
											{
												StartNewCycle(
													dbConnector,
													GetCompositionId(dbConnector),
													processItem.Field<Int32>("ProcessIdx"),
													eventItem.Field<DateTime>("EventTime"));
											}

											// Step2 アクティブカメラ変更
											ChangeCameraActive(dbConnector, GetCompositionId(dbConnector));

											dioSender.QROKDO();
										}
										else
										{
											Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 品番変更 品番コード データ異常 / {GuidCode1}");
											Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 品番変更 工程インデックスデータ異常");
											dioSender.QRNGDO();
										}
									}
									else
									{
										Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 品番変更 品番コード データ異常 / {GuidCode1}");
										dioSender.QRNGDO();
									}
									break;
								case 3:
									// 3:作業者(工程毎)変更
									//		コード1 = 作業者ID
									//		コード2 = 工程インデックス
									if (ValidateWorker(dbConnector, GuidCode1))
									{
										if (int.TryParse(qrCodeArray[2], out iProcessIdx))
										{
											InsertWorkerResults(dbConnector, GuidCode1, GetCompositionId(dbConnector), iProcessIdx, eventItem.Field<DateTime>("EventTime"));
											dioSender.RequestDO(
												GetDOPort(dbConnector,
													GuidCode1,
													GetCompositionId(dbConnector),
													iProcessIdx),
												QRAssignPortDelayMs);
										}
										else
										{
											Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 作業者変更 工程インデックスデータ異常");
											dioSender.QRNGDO();
										}
									}
									else
									{
										Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QR種別 作業者変更 作業者コード データ異常 / {GuidCode1}");
										dioSender.QRNGDO();
									}
									break;
								default:
									break;
							}
						}
						else
						{
							Common.Logger.Instance.WriteTraceLog($"[HandleQREvent] QRデータ異常");
							dioSender.QRNGDO();
						}
					}
					// Event_Tblの処理が終わったレコードは処理状態 = 2:処理済へ変更
					UpdateEventTblStatus(
						dbConnector,
						eventItem.Field<DateTime>("EventTime"),
						eventItem.Field<Guid>("DeviceId"),
						eventItem.Field<Int32>("EventTimeSub"),
						2);
				}

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}
		}
		#endregion

		///////////////////////////////////////////

		#region FinishPreviousCycleTimeResults

		private void FinishPreviousCycleTimeResults(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime EndTime)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 前回開始したレコードを終了
					sb.Append("UPDATE CycleTimeResults_Tbl SET ");
					sb.Append("  EndTime = @EndTime ");
					sb.Append("  ,CycleTime = datediff(millisecond, StartTime, @EndTime) / 1000.0 ");
					sb.Append(" WHERE CompositionId = @CompositionId ");
					sb.Append("   AND ProcessIdx = @ProcessIdx ");
					sb.Append("   AND StartTime < @EndTime ");
					sb.Append("   AND EndTime IS NULL ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = EndTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException UpdatePreviousCycleTimeResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception UpdatePreviousCycleTimeResults", ex);
			}
		}
		#endregion

		#region InsertCycleTimeResults
		private void InsertCycleTimeResults(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime)
		{

			#region SQL_MERGE_INSERT
			try
			{
				String CommandText = "";

				Guid ProductTypeId = GetProductTypeId(dbConnector, CompositionId, ProcessIdx);

				if (ProductTypeId != Guid.Empty)
				{
					using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
					{
						StringBuilder sb = new StringBuilder();

						// 新たな開始レコード追加
						sb.Clear();
						sb.Append("MERGE INTO CycleTimeResults_Tbl AS T1  ");
						sb.Append("  USING ");
						sb.Append("    (SELECT ");
						sb.Append("      @CompositionId AS CompositionId ");
						sb.Append("      ,@ProcessIdx AS ProcessIdx");
						sb.Append("      ,@StartTime AS StartTime");
						sb.Append("    ) AS T2");
						sb.Append("  ON (");
						sb.Append("   T1.CompositionId = T2.CompositionId ");
						sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
						sb.Append("   AND T1.StartTime = T2.StartTime ");
						sb.Append("  )");
						sb.Append(" WHEN NOT MATCHED THEN ");
						sb.Append("  INSERT (");
						sb.Append("   CompositionId");
						sb.Append("   ,ProcessIdx");
						sb.Append("   ,StartTime");
						sb.Append("   ,ProductTypeId");
						sb.Append("   ) VALUES (");
						sb.Append("   @CompositionId ");
						sb.Append("   ,@ProcessIdx ");
						sb.Append("   ,@StartTime ");
						sb.Append("   ,@ProductTypeId ");
						sb.Append("   )");
						sb.Append(";");

						cmd.CommandText = sb.ToString();
						CommandText = cmd.CommandText;

						cmd.Parameters.Clear();
						cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
						cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
						cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;
						cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertCycleTimeResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertCycleTimeResults", ex);
			}
			#endregion
		}
		#endregion

		#region InsertCycleVideoTbl
		private void InsertCycleVideoTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime,
				int CameraIdx,
				DateTime CreateableTime)
		{
			#region SQL_MERGE_INSERT
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 新たな開始レコード追加
					sb.Clear();
					sb.Append("MERGE INTO CycleVideo_Tbl AS T1  ");
					sb.Append("  USING ");
					sb.Append("    (SELECT ");
					sb.Append("      @CompositionId AS CompositionId ");
					sb.Append("      ,@ProcessIdx AS ProcessIdx");
					sb.Append("      ,@StartTime AS StartTime");
					sb.Append("      ,@CameraIdx AS CameraIdx");
					sb.Append("    ) AS T2");
					sb.Append("  ON (");
					sb.Append("   T1.CompositionId = T2.CompositionId ");
					sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
					sb.Append("   AND T1.StartTime = T2.StartTime ");
					sb.Append("   AND T1.CameraIdx = T2.CameraIdx ");
					sb.Append("  )");
					sb.Append(" WHEN NOT MATCHED THEN ");
					sb.Append("  INSERT (");
					sb.Append("   CompositionId");
					sb.Append("   ,ProcessIdx");
					sb.Append("   ,StartTime");
					sb.Append("   ,CameraIdx");
					sb.Append("   ,VideoFileName");
					sb.Append("   ,CreationStatus");
					sb.Append("   ,CreateableTime");
					sb.Append("   ) VALUES (");
					sb.Append("   @CompositionId ");
					sb.Append("   ,@ProcessIdx ");
					sb.Append("   ,@StartTime ");
					sb.Append("   ,@CameraIdx ");
					sb.Append("   ,'' ");
					sb.Append("   ,0 ");
					sb.Append("   ,@CreateableTime ");
					sb.Append("   )");
					sb.Append(";");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;
					cmd.Parameters.Add("@CameraIdx", SqlDbType.Int).Value = CameraIdx;
					cmd.Parameters.Add("@CreateableTime", SqlDbType.DateTime).Value = CreateableTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertCycleVideoTbl", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertCycleVideoTbl", ex);
			}
			#endregion
		}
		#endregion

		#region InsertCycleVideoCreatTimeTbl
		private void InsertCycleVideoCreatTimeTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime,
				int CameraIdx,
				int SeparationIdx,
				DateTime SeparationStartTime,
				DateTime SeparationEndTime
				)
		{
			#region SQL_MERGE_INSERT
			try
			{
				String CommandText = "";

				Guid ProductTypeId = GetProductTypeId(dbConnector, CompositionId, ProcessIdx);

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 新たな開始レコード追加
					sb.Clear();
					sb.Append("MERGE INTO CycleVideoCreat_Time_Tbl AS T1  ");
					sb.Append("  USING ");
					sb.Append("    (SELECT ");
					sb.Append("      @CompositionId AS CompositionId ");
					sb.Append("      ,@ProcessIdx AS ProcessIdx");
					sb.Append("      ,@StartTime AS StartTime");
					sb.Append("      ,@CameraIdx AS CameraIdx");
					sb.Append("      ,@SeparationIdx AS SeparationIdx");
					sb.Append("    ) AS T2");
					sb.Append("  ON (");
					sb.Append("   T1.CompositionId = T2.CompositionId ");
					sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
					sb.Append("   AND T1.StartTime = T2.StartTime ");
					sb.Append("   AND T1.CameraIdx = T2.CameraIdx ");
					sb.Append("   AND T1.SeparationIdx = T2.SeparationIdx ");
					sb.Append("  )");
					sb.Append(" WHEN NOT MATCHED THEN ");
					sb.Append("  INSERT (");
					sb.Append("   CompositionId");
					sb.Append("   ,ProcessIdx");
					sb.Append("   ,StartTime");
					sb.Append("   ,CameraIdx");
					sb.Append("   ,SeparationIdx");
					sb.Append("   ,SeparationStartTime");
					sb.Append("   ,SeparationEndTime");
					sb.Append("   ) VALUES (");
					sb.Append("   @CompositionId ");
					sb.Append("   ,@ProcessIdx ");
					sb.Append("   ,@StartTime ");
					sb.Append("   ,@CameraIdx ");
					sb.Append("   ,@SeparationIdx ");
					sb.Append("   ,@SeparationStartTime ");
					sb.Append("   ,@SeparationEndTime ");
					sb.Append("   )");
					sb.Append(";");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;
					cmd.Parameters.Add("@CameraIdx", SqlDbType.Int).Value = CameraIdx;
					cmd.Parameters.Add("@SeparationIdx", SqlDbType.Int).Value = SeparationIdx;
					cmd.Parameters.Add("@SeparationStartTime", SqlDbType.DateTime).Value = SeparationStartTime;
					cmd.Parameters.Add("@SeparationEndTime", SqlDbType.DateTime).Value = SeparationEndTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertCycleVideoCreatTimeTbl", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertCycleVideoCreatTimeTbl", ex);
			}
			#endregion
		}
		#endregion

		#region InsertCycleVideoCreatOriginalTbl
		private void InsertCycleVideoCreatOriginalTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime,
				DateTime EndTime,
				int CameraIdx,
				Guid DeviceId
				)
		{
			#region SQL_MERGE_INSERT
			try
			{
				String CommandText = "";

				DataTable dt = new DataTable("Event_Tbl");

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("SELECT T1.* ");
					sb.Append(" FROM Event_Tbl T1 ");
					sb.Append(" WHERE T1.EventType = @EventType ");
					sb.Append("  AND T1.DeviceId = @DeviceId ");
					sb.Append("  AND T1.EventTime >= @StartTime ");
					sb.Append("  AND T1.EventTime <= @EndTime ");
					sb.Append("  Order by EventTime ");

					cmd.CommandText = sb.ToString();

					cmd.Parameters.Add(new SqlParameter("@EventType", SqlDbType.Int)).Value = 2;
					cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;
					cmd.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime)).Value = StartTime.AddSeconds(-RawVideoSecond);
					cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime)).Value = EndTime;

					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dt);
					}
				}

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					int OriginalVideoIdx = 0;
					foreach (var targetEventItem in dt.AsEnumerable())
					{
						StringBuilder sb = new StringBuilder();

						// 新たな開始レコード追加
						sb.Clear();
						sb.Append("MERGE INTO CycleVideoCreat_Original_Tbl AS T1  ");
						sb.Append("  USING ");
						sb.Append("    (SELECT ");
						sb.Append("      @CompositionId AS CompositionId ");
						sb.Append("      ,@ProcessIdx AS ProcessIdx");
						sb.Append("      ,@StartTime AS StartTime");
						sb.Append("      ,@CameraIdx AS CameraIdx");
						sb.Append("      ,@OriginalVideoIdx AS OriginalVideoIdx");
						sb.Append("    ) AS T2");
						sb.Append("  ON (");
						sb.Append("   T1.CompositionId = T2.CompositionId ");
						sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
						sb.Append("   AND T1.StartTime = T2.StartTime ");
						sb.Append("   AND T1.CameraIdx = T2.CameraIdx ");
						sb.Append("   AND T1.OriginalVideoIdx = T2.OriginalVideoIdx ");
						sb.Append("  )");
						sb.Append(" WHEN NOT MATCHED THEN ");
						sb.Append("  INSERT (");
						sb.Append("   CompositionId");
						sb.Append("   ,ProcessIdx");
						sb.Append("   ,StartTime");
						sb.Append("   ,CameraIdx");
						sb.Append("   ,OriginalVideoIdx");
						sb.Append("   ,OriginalVideoFileName");
						sb.Append("   ,OriginalStartTime");
						sb.Append("   ) VALUES (");
						sb.Append("   @CompositionId ");
						sb.Append("   ,@ProcessIdx ");
						sb.Append("   ,@StartTime ");
						sb.Append("   ,@CameraIdx ");
						sb.Append("   ,@OriginalVideoIdx ");
						sb.Append("   ,@OriginalVideoFileName ");
						sb.Append("   ,@OriginalStartTime ");
						sb.Append("   )");
						sb.Append(";");

						cmd.CommandText = sb.ToString();
						CommandText = cmd.CommandText;

						cmd.Parameters.Clear();
						cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
						cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
						cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;
						cmd.Parameters.Add("@CameraIdx", SqlDbType.Int).Value = CameraIdx;
						cmd.Parameters.Add("@OriginalVideoIdx", SqlDbType.Int).Value = OriginalVideoIdx++;
						cmd.Parameters.Add("@OriginalVideoFileName", SqlDbType.NVarChar).Value = targetEventItem.Field<String>("EventData");
						cmd.Parameters.Add("@OriginalStartTime", SqlDbType.DateTime).Value = targetEventItem.Field<DateTime>("EventTime");

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertCycleVideoCreatOriginalTbl", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertCycleVideoCreatOriginalTbl", ex);
			}
			#endregion
		}
		#endregion

		#region UpdateEventTblStatus
		private void UpdateEventTblStatus(
				SqlDBConnector dbConnector,
				DateTime EventTime,
				Guid DeviceId,
				int EventTimeSub,
				int ProcessingState
				)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("UPDATE Event_Tbl SET ");
					sb.Append("  ProcessingState = @ProcessingState ");
					sb.Append(" WHERE EventTime = @EventTime ");
					sb.Append("  AND DeviceId = @DeviceId ");
					sb.Append("  AND EventTimeSub = @EventTimeSub ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SqlParameter("@EventTime", SqlDbType.DateTime)).Value = EventTime;
					cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;
					cmd.Parameters.Add(new SqlParameter("@EventTimeSub", SqlDbType.Int)).Value = EventTimeSub;
					cmd.Parameters.Add(new SqlParameter("@ProcessingState", SqlDbType.Int)).Value = ProcessingState;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException UpdateEventTblStatus", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception UpdateEventTblStatus", ex);
			}
		}
		#endregion

		#region UpdateCameraActiveTbl
		private void UpdateCameraActiveTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId
				)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("DELETE FROM Camera_Active_Tbl ");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.ExecuteNonQuery();

					// CompositionIdがEmptyならばカメラは使用しない
					if (CompositionId == Guid.Empty) return;

					sb.Clear();
					sb.Append("INSERT INTO Camera_Active_Tbl( ");
					sb.Append("  DeviceId ");
					sb.Append("  ,CameraFPS ");
					sb.Append("  ,CameraCondition ");
					sb.Append(")");
					sb.Append(" SELECT  ");
					sb.Append("  T3.DeviceId ");
					sb.Append(" ,MIN(T3.CameraFPS) CameraFPS ");
					sb.Append(" ,MIN(T3.CameraCondition) CameraCondition ");
					sb.Append(" FROM ProductTypeResults_Tbl T1 ");
					sb.Append("  INNER JOIN DataCollection_Enable_Mst T2 ");
					sb.Append("   ON  T2.CompositionId = T1.CompositionId");
					sb.Append("   AND T2.ProcessIdx = T1.ProcessIdx");
					sb.Append("   AND T2.ProductTypeId = T1.ProductTypeId");
					sb.Append("   AND T2.[Enable] = 1");
					sb.Append("  INNER JOIN DataCollection_Camera_Mst T3 ");
					sb.Append("   ON  T3.CompositionId = T2.CompositionId");
					sb.Append("   AND T3.ProcessIdx = T2.ProcessIdx");
					sb.Append("   AND T3.ProductTypeId = T2.ProductTypeId");
					sb.Append("   AND T3.CameraCondition <> -1");
					sb.Append("  WHERE T1.EndTime IS NULL  ");
					sb.Append("       AND T1.CompositionId = @CompositionId ");
					sb.Append("   Group by T3.DeviceId ");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException UpdateCameraActiveTbl", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception UpdateCameraActiveTbl", ex);
			}
		}
		#endregion

		#region IncrementOperatingShiftProductionQuantityTbl
		private void IncrementOperatingShiftProductionQuantityTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime
				)
		{
			#region SQL_MERGE_INSERT
			try
			{
				String CommandText = "";

				Guid ProductTypeId = GetProductTypeId(dbConnector, CompositionId, ProcessIdx);

				if (IsProductionQuantityCounter(dbConnector, CompositionId, ProcessIdx))
				{
					using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
					{
						DateTime OperationDate = DateTime.MinValue;
						int OperationShift = 0;
						StringBuilder sb = new StringBuilder();

						sb.Append("SELECT Top(1) T1.* ");
						sb.Append(" FROM Operating_Shift_Tbl T1 ");
						sb.Append(" WHERE T1.StartTime <= @StartTime ");
						sb.Append("  AND T1.EndTime >= @StartTime  ");
						sb.Append("  Order by T1.ActualType ");

						cmd.CommandText = sb.ToString();

						cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								OperationDate = reader.GetDateTime(reader.GetOrdinal("OperationDate"));
								OperationShift = reader.GetInt32(reader.GetOrdinal("OperationShift"));
							}
						}

						// 実績も計画の稼働時間が取れなければ、良品数はカウントできない（しない)
						if (OperationDate == DateTime.MinValue || OperationShift == 0) return;

						sb.Clear();
						sb.Append("MERGE INTO Operating_Shift_ProductionQuantity_Tbl AS T1  ");
						sb.Append("  USING ");
						sb.Append("    (SELECT ");
						sb.Append("      @OperationDate AS OperationDate ");
						sb.Append("      ,@OperationShift AS OperationShift");
						sb.Append("      ,@ActualType AS ActualType");
						sb.Append("      ,@ProductTypeId AS ProductTypeId");
						sb.Append("    ) AS T2");
						sb.Append("  ON (");
						sb.Append("   T1.OperationDate = T2.OperationDate ");
						sb.Append("   AND T1.OperationShift = T2.OperationShift ");
						sb.Append("   AND T1.ActualType = T2.ActualType ");
						sb.Append("   AND T1.ProductTypeId = T2.ProductTypeId ");
						sb.Append("  )");
						sb.Append(" WHEN MATCHED THEN ");
						sb.Append("  UPDATE ");
						sb.Append("   SET ProductionQuantity = ProductionQuantity + 1");
						sb.Append(" WHEN NOT MATCHED THEN ");
						sb.Append("  INSERT (");
						sb.Append("   OperationDate");
						sb.Append("   ,OperationShift");
						sb.Append("   ,ActualType");
						sb.Append("   ,ProductTypeId");
						sb.Append("   ,ProductionQuantity");
						sb.Append("   ) VALUES (");
						sb.Append("   @OperationDate ");
						sb.Append("   ,@OperationShift ");
						sb.Append("   ,@ActualType ");
						sb.Append("   ,@ProductTypeId ");
						sb.Append("   ,1 ");
						sb.Append("   )");
						sb.Append(";");

						cmd.CommandText = sb.ToString();
						CommandText = cmd.CommandText;

						cmd.Parameters.Clear();
						cmd.Parameters.Add("@OperationDate", SqlDbType.Date).Value = OperationDate.Date;
						cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;
						cmd.Parameters.Add("@ActualType", SqlDbType.Int).Value = 0;
						cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException UpdateOperatingShiftProductionQuantityTbl", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception UpdateOperatingShiftProductionQuantityTbl", ex);
			}
			#endregion

		}
		#endregion

		#region IsProductionQuantityCounter

		private Boolean IsProductionQuantityCounter(
			SqlDBConnector dbConnector
			, Guid CompositionId
			, int ProcessIdx
			)
		{
			DataTable dt = new DataTable("ProductTypeResults_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append(" T1.* ");
				sb.Append(" FROM ProductTypeResults_Tbl T1 ");
				sb.Append(" INNER JOIN DataCollection_Trigger_Mst T2 ");
				sb.Append("  ON  T2.CompositionId = T1.CompositionId");
				sb.Append("  AND T2.ProcessIdx = T1.ProcessIdx");
				sb.Append("  AND T2.ProductTypeId = T1.ProductTypeId");
				sb.Append(" WHERE ");
				sb.Append("   T1.EndTime IS NULL  ");
				sb.Append("   AND T1.CompositionId = @CompositionId ");
				sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("   AND T2.IsProductionQuantityCounter = 1 ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.Rows.Count > 0;
		}
		#endregion

		#region GetCompositionId
		private Guid GetCompositionId(SqlDBConnector dbConnector)
		{
			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM CompositionResults_Tbl T1 ");
				sb.Append(" WHERE T1.EndTime IS NULL ");
				sb.Append("  order by StartTime DESC ");

				cmd.CommandText = sb.ToString();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetGuid(reader.GetOrdinal("CompositionId"));
					}
				}
			}

			// 稼働中編成がない = 直間
			return Guid.Empty;
		}

		#endregion

		#region GetProductTypeId
		private Guid GetProductTypeId(
						SqlDBConnector dbConnector,
						Guid CompositionId,
						int ProcessIdx
						)
		{
			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM ProductTypeResults_Tbl T1 ");
				sb.Append(" WHERE CompositionId = @CompositionId ");
				sb.Append("   AND ((@ProcessIdx < 100 AND ProcessIdx = @ProcessIdx) ");
				sb.Append("       OR (@ProcessIdx >= 100 AND ProcessIdx = (SELECT MAX(ProcessIdx) FROM Composition_Process_Mst WHERE CompositionId = @CompositionId) ))");
				sb.Append("   AND EndTime IS NULL ");
				sb.Append("  order by StartTime DESC ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
				cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetGuid(reader.GetOrdinal("ProductTypeId"));
					}
				}
			}
			return Guid.Empty;
		}

		#endregion

		#region QueryUnProcessingEventTbl
		private DataTable QueryUnProcessingEventTbl(
				SqlDBConnector dbConnector, int EventType)
		{
			DataTable dt = new DataTable("Event_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" FROM Event_Tbl T1 ");
				sb.Append(" WHERE T1.EventType = @EventType ");
				sb.Append("  AND T1.ProcessingState = @ProcessingState ");
				sb.Append("  Order by EventTime ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@EventType", SqlDbType.Int)).Value = EventType;
				cmd.Parameters.Add(new SqlParameter("@ProcessingState", SqlDbType.Int)).Value = 0;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryPreviousEventTbl
		private DataRow QueryPreviousEventTbl(
				SqlDBConnector dbConnector
				, int EventType
				, DateTime EventTime
				, int EventTimeSub
				, int EventDetail
				, Guid DeviceId)
		{
			DataTable dt = new DataTable("Event_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM Event_Tbl T1 ");
				sb.Append(" WHERE T1.EventType = @EventType ");
				sb.Append("  AND T1.DeviceId = @DeviceId ");
				sb.Append("  AND (T1.EventTime < @EventTime ");
				sb.Append("   OR (T1.EventTime = @EventTime AND T1.EventTimeSub < @EventTimeSub) ) ");
				sb.Append("  AND T1.EventDetail = @EventDetail ");
				sb.Append("  Order by EventTime DESC ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@EventType", SqlDbType.Int)).Value = EventType;
				cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;
				cmd.Parameters.Add(new SqlParameter("@EventTime", SqlDbType.DateTime)).Value = EventTime;
				cmd.Parameters.Add(new SqlParameter("@EventTimeSub", SqlDbType.Int)).Value = EventTimeSub;
				cmd.Parameters.Add(new SqlParameter("@EventDetail", SqlDbType.Int)).Value = EventDetail;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.AsEnumerable().FirstOrDefault();
		}

		private DataRow QueryPreviousEventTbl(
				SqlDBConnector dbConnector
				, int EventType
				, DateTime EventTime
				, int EventTimeSub
				, Guid DeviceId)
		{
			DataTable dt = new DataTable("Event_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM Event_Tbl T1 ");
				sb.Append(" WHERE T1.EventType = @EventType ");
				sb.Append("  AND T1.DeviceId = @DeviceId ");
				sb.Append("  AND (T1.EventTime < @EventTime ");
				sb.Append("   OR (T1.EventTime = @EventTime AND T1.EventTimeSub < @EventTimeSub) ) ");
				sb.Append("  Order by EventTime DESC ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@EventType", SqlDbType.Int)).Value = EventType;
				cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;
				cmd.Parameters.Add(new SqlParameter("@EventTime", SqlDbType.DateTime)).Value = EventTime;
				cmd.Parameters.Add(new SqlParameter("@EventTimeSub", SqlDbType.Int)).Value = EventTimeSub;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.AsEnumerable().FirstOrDefault();
		}
		#endregion

		#region QueryUnProcessingSpecifiedTimeProcessingTbl
		private DataTable QueryUnProcessingSpecifiedTimeProcessingTbl(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("SpecifiedTimeProcessing_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" FROM SpecifiedTimeProcessing_Tbl T1 ");
				sb.Append(" WHERE T1.ProcessingState = 0 ");
				sb.Append("  AND T1.SpecifiedTime < SYSDATETIME() ");
				sb.Append("  Order by SpecifiedTime, OperationDate, OperationShift ");

				cmd.CommandText = sb.ToString();

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region UpdateSpecifiedTimeProcessingTblStatus
		private void UpdateSpecifiedTimeProcessingTblStatus(
				SqlDBConnector dbConnector,
				Guid ProcessingId,
				int ProcessingState
				)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("UPDATE SpecifiedTimeProcessing_Tbl SET ");
					sb.Append("  ProcessingState = @ProcessingState ");
					sb.Append(" WHERE ProcessingId = @ProcessingId ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SqlParameter("@ProcessingId", SqlDbType.UniqueIdentifier)).Value = ProcessingId;
					cmd.Parameters.Add(new SqlParameter("@ProcessingState", SqlDbType.Int)).Value = ProcessingState;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException UpdateSpecifiedTimeProcessingTblStatus", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception UpdateSpecifiedTimeProcessingTblStatus", ex);
			}
		}
		#endregion

		#region IsChatteringData

		private Boolean IsChatteringData(
			SqlDBConnector dbConnector
			, Guid DeviceId
			, decimal SensorChatteringFilter
			, DateTime EventTime
			, int EventTimeSub
			, int EventDetail)
		{
			// DIのみを対象 EventType = 0
			var PreviousSensor = 
					QueryPreviousEventTbl(dbConnector, 0, EventTime, EventTimeSub, EventDetail ^ 1, DeviceId);
			return PreviousSensor != null && (DateTime)PreviousSensor.Field<DateTime>("EventTime") >= EventTime.AddSeconds(-(Double)SensorChatteringFilter);
		}
		#endregion

		#region IsAliveProductType

		private Boolean IsAliveProductType(
			SqlDBConnector dbConnector, int ProcessIdx)
		{
			DataTable dt = new DataTable("ProductTypeResults_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append("  T1.* ");
				sb.Append(" FROM ProductTypeResults_Tbl T1 ");
				sb.Append("  INNER JOIN DataCollection_Enable_Mst T2 ON ");
				sb.Append("    T2.CompositionId = T1.CompositionId ");
				sb.Append("    AND T2.ProcessIdx = T1.ProcessIdx ");
				sb.Append("    AND T2.ProductTypeId = T1.ProductTypeId ");
				sb.Append(" WHERE T1.EndTime IS NULL ");
				sb.Append("  AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("  AND T2.Enable = 1 ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;


				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.Rows.Count > 0;
		}
		#endregion

		#region QueryDataCollectionTriggerMst
		private DataTable QueryDataCollectionTriggerMst(
				SqlDBConnector dbConnector, Guid CompositionId)
		{
			DataTable dt = new DataTable("DataCollection_Trigger_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" FROM DataCollection_Trigger_Mst T1 ");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				sb.Append("   AND ProductTypeId = @ProductTypeId ");
				sb.Append("  Order by ProcessIdx ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				//cmd.Parameters.Add(new SqlParameter("@ProductTypeId", SqlDbType.UniqueIdentifier)).Value = ProductTypeId;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryCurrentTriggerSensor
		private DataTable QueryCurrentTriggerSensor(
				SqlDBConnector dbConnector, Guid CompositionId)
		{
			DataTable dt = new DataTable("DataCollection_Trigger_Sensor_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append(" T3.DeviceId");
				sb.Append(" ,T3.SensorIdx");
				sb.Append(" ,T3.SensorChatteringFilter");
				sb.Append(" ,T1.ProductTypeId");
				sb.Append(" ,T1.ProcessIdx");
				sb.Append(" ,T1.CompositionId");
				sb.Append(" ,T4.TriggerType");
				sb.Append(" ,T4.IsProductionQuantityCounter");
				sb.Append(" ,T4.IntervalFilter");
				sb.Append(" FROM ProductTypeResults_Tbl T1 ");
				sb.Append(" INNER JOIN DataCollection_Enable_Mst T2 ");
				sb.Append("  ON  T2.CompositionId = T1.CompositionId");
				sb.Append("  AND T2.ProcessIdx = T1.ProcessIdx");
				sb.Append("  AND T2.ProductTypeId = T1.ProductTypeId");
				sb.Append("  AND T2.[Enable] = 1");
				sb.Append(" INNER JOIN DataCollection_Trigger_Sensor_Mst T3 ");
				sb.Append("  ON  T3.CompositionId = T2.CompositionId");
				sb.Append("  AND T3.ProcessIdx = T2.ProcessIdx");
				sb.Append("  AND T3.ProductTypeId = T2.ProductTypeId");
				sb.Append(" INNER JOIN DataCollection_Trigger_Mst T4 ");
				sb.Append("  ON  T4.CompositionId = T2.CompositionId");
				sb.Append("  AND T4.ProcessIdx = T2.ProcessIdx");
				sb.Append("  AND T4.ProductTypeId = T2.ProductTypeId");
				sb.Append(" WHERE ");
				sb.Append("   T1.EndTime IS NULL  "); 
				sb.Append("   AND T1.CompositionId = @CompositionId ");
				sb.Append("  Order by T3.ProcessIdx, T3.SensorIdx ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryCurrentCamera
		private DataTable QueryCurrentCamera(
				SqlDBConnector dbConnector, Guid CompositionId)
		{
			DataTable dt = new DataTable("DataCollection_Camera_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append(" T3.* ");
				sb.Append(" FROM ProductTypeResults_Tbl T1 ");
				sb.Append(" INNER JOIN DataCollection_Enable_Mst T2 ");
				sb.Append("  ON  T2.CompositionId = T1.CompositionId");
				sb.Append("  AND T2.ProcessIdx = T1.ProcessIdx");
				sb.Append("  AND T2.ProductTypeId = T1.ProductTypeId");
				sb.Append("  AND T2.[Enable] = 1");
				sb.Append(" INNER JOIN DataCollection_Camera_Mst T3 ");
				sb.Append("  ON  T3.CompositionId = T2.CompositionId");
				sb.Append("  AND T3.ProcessIdx = T2.ProcessIdx");
				sb.Append("  AND T3.ProductTypeId = T2.ProductTypeId");
				sb.Append("  AND T3.CameraCondition <> -1");
				sb.Append(" WHERE ");
				sb.Append("   T1.EndTime IS NULL  ");
				sb.Append("   AND T1.CompositionId = @CompositionId ");
				sb.Append("  Order by ProcessIdx, T3.CameraIdx  ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryPreviousCycleTimeResultsTbl
		private DataRow QueryPreviousCycleTimeResultsTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime EndTime)
		{
			DataTable dt = new DataTable("CycleTimeResults_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM CycleTimeResults_Tbl T1 ");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				sb.Append("  AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("  AND T1.EndTime = @EndTime ");
				sb.Append("  Order by StartTime DESC ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
				cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime)).Value = EndTime;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.AsEnumerable().FirstOrDefault();
		}

		#endregion

		#region QueryRangeCycleTimeResultsTbl
		private DataTable QueryRangeCycleTimeResultsTbl(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime_Range_Begin, DateTime StartTime_Range_End)
		{
			DataTable dt = new DataTable("CycleTimeResults_Tbl");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT Top(1) T1.* ");
				sb.Append(" FROM CycleTimeResults_Tbl T1 ");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				sb.Append("  AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("  AND T1.StartTime > @StartTime_Range_Begin ");
				sb.Append("  AND T1.StartTime > @StartTime_Range_End ");
				sb.Append("  Order by StartTime DESC ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
				cmd.Parameters.Add(new SqlParameter("@StartTime_Range_Begin", SqlDbType.DateTime)).Value = StartTime_Range_Begin;
				cmd.Parameters.Add(new SqlParameter("@StartTime_Range_End", SqlDbType.DateTime)).Value = StartTime_Range_End;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region ValidateComposition
		private Boolean ValidateComposition(SqlDBConnector dbConnector, Guid CompositionId)
		{
			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT COUNT(*) ");
				sb.Append(" FROM Composition_Mst ");
				sb.Append(" WHERE CompositionId = @CompositionId ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetInt32(0) > 0;
					}
				}
			}

			return false;
		}
		#endregion

		#region ValidateProductType
		private Boolean ValidateProductType(
			SqlDBConnector dbConnector
			, Guid ProductTypeId)
		{
			DataTable dt = new DataTable("DataCollection_Enable_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append("  T1.* ");
				sb.Append(" FROM ProductType_Mst T1 ");
				sb.Append(" WHERE  T1.ProductTypeId = @ProductTypeId ");
				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.Rows.Count > 0;
		}

		#endregion

		#region ValidateCompositionProcess
		private Boolean ValidateCompositionProcess(
			SqlDBConnector dbConnector
			, Guid CompositionId
			, int ProcessIdx)
		{
			DataTable dt = new DataTable("DataCollection_Enable_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append("  T1.* ");
				sb.Append(" FROM Composition_Process_Mst T1 ");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				sb.Append("  AND T1.ProcessIdx = @ProcessIdx ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
				cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt.Rows.Count > 0;
		}

		#endregion

		#region ValidateWorker
		private Boolean ValidateWorker(SqlDBConnector dbConnector, Guid WorkerId)
		{
			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT COUNT(*) ");
				sb.Append(" FROM Worker_Mst ");
				sb.Append(" WHERE WorkerId = @WorkerId ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@WorkerId", SqlDbType.UniqueIdentifier).Value = WorkerId;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetInt32(0) > 0;
					}
				}
			}

			return false;
		}
		#endregion

		///////////////////////////////////////////
		/// Composition

		#region InsertCompositionResults

		private void InsertCompositionResults(
				SqlDBConnector dbConnector,
				Guid CompositionId,
				DateTime StartTime)
		{
			try
			{
				String CommandText = "";

				// 1:編成変更
				//		コード1 = 編成ID
				//   編成実績テーブル(CompositionResults_Tbl)へ追加
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 前回開始したレコードを終了
					sb.Append("UPDATE CompositionResults_Tbl SET ");
					sb.Append("  EndTime = @StartTime ");
					sb.Append(" WHERE EndTime IS NULL ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();

					// 新たな開始レコード追加
					sb.Clear();
					sb.Append("MERGE INTO CompositionResults_Tbl AS T1  ");
					sb.Append("  USING ");
					sb.Append("    (SELECT ");
					sb.Append("      @CompositionId AS CompositionId ");
					sb.Append("      ,@StartTime AS StartTime");
					sb.Append("    ) AS T2");
					sb.Append("  ON (");
					sb.Append("   T1.CompositionId = T2.CompositionId ");
					sb.Append("   AND T1.StartTime = T2.StartTime ");
					sb.Append("  )");
					sb.Append(" WHEN NOT MATCHED THEN ");
					sb.Append("  INSERT (");
					sb.Append("   CompositionId");
					sb.Append("   ,StartTime");
					sb.Append("   ) VALUES (");
					sb.Append("   @CompositionId ");
					sb.Append("   ,@StartTime ");
					sb.Append("   )");
					sb.Append(";");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertCompositionResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertCompositionResults", ex);
			}
		}
		#endregion

		#region FinishCompositionResults

		private void FinishCompositionResults(
				SqlDBConnector dbConnector, DateTime EndTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "FinishCompositionResults";
					cmd.Parameters.AddWithValue("@EndTime", EndTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException FinishCompositionResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception FinishCompositionResults", ex);
			}
		}
		#endregion

		#region RestoreCompositionResults

		/// <summary>
		/// 編成引継(終了時刻が直近の編成を引継ぐ)
		/// </summary>
		/// <param name="dbConnector"></param>
		private void RestoreCompositionResults(
				SqlDBConnector dbConnector, DateTime StartTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "RestoreCompositionResults";
					cmd.Parameters.AddWithValue("@StartTime", StartTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException RestoreCompositionResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception RestoreCompositionResults", ex);
			}
		}
		#endregion

		#region QueryCompositionProcessMst
		private DataTable QueryCompositionProcessMst(
				SqlDBConnector dbConnector
				, Guid CompositionId
				, Boolean IsOnlyEnable = false)
		{
			DataTable dt = new DataTable("Composition_Process_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append(" T1.* ");
				sb.Append(" ,CASE WHEN T3.[Enable] IS NULL THEN 0 ELSE T3.[Enable] END [Enable] ");
				sb.Append(" FROM Composition_Process_Mst T1 ");
				sb.Append(" LEFT JOIN ProductTypeResults_Tbl T2");
				sb.Append(" ON T2.CompositionId = T1.CompositionId");
				sb.Append("   AND T2.ProcessIdx = T1.ProcessIdx");
				sb.Append("   AND T2.EndTime IS NULL");
				sb.Append(" LEFT JOIN DataCollection_Enable_Mst T3");
				sb.Append(" ON T3.CompositionId = T2.CompositionId");
				sb.Append("   AND T3.ProcessIdx = T2.ProcessIdx");
				sb.Append("   AND T3.ProductTypeId = T2.ProductTypeId");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				if(IsOnlyEnable)
					sb.Append(" AND (T3.[Enable] IS NULL OR T3.[Enable] = 1)  ");
				sb.Append("  Order by ProcessIdx ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		///////////////////////////////////////////
		/// ProductTypeResults

		#region InsertProductTypeResults

		public void InsertProductTypeResults(
				SqlDBConnector dbConnector,
				Guid ProductTypeId,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime)
		{
			try
			{
				String CommandText = "";

				// 2:品番変更
				//		コード1 = 品番ID
				//		コード2 = 工程インデックス
				//		コード2が99の場合は全工程への展開とする
				//		品番実績テーブル(ProductNumberResults_Tbl)へ追加
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 前回開始したレコードを終了
					sb.Append("UPDATE ProductTypeResults_Tbl SET ");
					sb.Append("  EndTime = @StartTime ");
					sb.Append(" WHERE ProcessIdx = @ProcessIdx ");
					sb.Append("   AND EndTime IS NULL ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();

					// 新たな開始レコード追加
					sb.Clear();
					sb.Append("MERGE INTO ProductTypeResults_Tbl AS T1  ");
					sb.Append("  USING ");
					sb.Append("    (SELECT ");
					sb.Append("      @ProductTypeId AS ProductTypeId ");
					sb.Append("      ,@CompositionId AS CompositionId ");
					sb.Append("      ,@ProcessIdx AS ProcessIdx ");
					sb.Append("      ,@StartTime AS StartTime");
					sb.Append("    ) AS T2");
					sb.Append("  ON (");
					sb.Append("   T1.ProductTypeId = T2.ProductTypeId ");
					sb.Append("   AND T1.CompositionId = T2.CompositionId ");
					sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
					sb.Append("   AND T1.StartTime = T2.StartTime ");
					sb.Append("  )");
					sb.Append(" WHEN NOT MATCHED THEN ");
					sb.Append("  INSERT (");
					sb.Append("   ProductTypeId");
					sb.Append("   ,CompositionId");
					sb.Append("   ,ProcessIdx");
					sb.Append("   ,StartTime");
					sb.Append("   ) VALUES (");
					sb.Append("   @ProductTypeId ");
					sb.Append("   ,@CompositionId ");
					sb.Append("   ,@ProcessIdx ");
					sb.Append("   ,@StartTime ");
					sb.Append("   )");
					sb.Append(";");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertProductTypeResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertProductTypeResults", ex);
			}
		}
		#endregion

		#region FinishProductTypeResults
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbConnector"></param>
		public void FinishProductTypeResults(
				SqlDBConnector dbConnector, DateTime EndTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "FinishProductTypeResults";
					cmd.Parameters.AddWithValue("@EndTime", EndTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException FinishProductTypeResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception FinishProductTypeResults", ex);
			}
		}
		#endregion

		#region RestoreProductTypeResults

		/// <summary>
		/// 品番引継(終了時刻が直近の品番を引継ぐ)
		/// </summary>
		/// <param name="dbConnector"></param>
		private void RestoreProductTypeResults(
				SqlDBConnector dbConnector, DateTime StartTime, DateTime EndTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "RestoreProductTypeResults";
					cmd.Parameters.AddWithValue("@StartTime", StartTime);
					cmd.Parameters.AddWithValue("@EndTime", EndTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException RestoreProductTypeResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception RestoreProductTypeResults", ex);
			}
		}
		#endregion

		///////////////////////////////////////////
		/// ProductTypeReserve
		#region　ProductTypeReserve

		//#region InsertProductTypeReserve

		//public void InsertProductTypeReserve(
		//		SqlDBConnector dbConnector,
		//		Guid ProductTypeId,
		//		Guid CompositionId,
		//		int ProcessIdx)
		//{
		//	try
		//	{
		//		String CommandText = "";

		//		// 2:品番変更
		//		//		コード1 = 品番ID
		//		//		コード2 = 工程インデックス
		//		//		コード2が99の場合は全工程への展開とする
		//		//		品番実績テーブル(ProductNumberResults_Tbl)へ追加
		//		using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//		{
		//			StringBuilder sb = new StringBuilder();

		//			// 新たな開始レコード追加
		//			sb.Clear();
		//			sb.Append("MERGE INTO ProductTypeReserve_Tbl AS T1  ");
		//			sb.Append("  USING ");
		//			sb.Append("    (SELECT ");
		//			sb.Append("      @ProcessIdx AS ProcessIdx ");
		//			sb.Append("    ) AS T2");
		//			sb.Append("  ON (");
		//			sb.Append("   T1.ProcessIdx = T2.ProcessIdx ");
		//			sb.Append("  )");
		//			sb.Append(" WHEN NOT MATCHED THEN ");
		//			sb.Append("  INSERT (");
		//			sb.Append("   ProductTypeId");
		//			sb.Append("   ,CompositionId");
		//			sb.Append("   ,ProcessIdx");
		//			sb.Append("   ) VALUES (");
		//			sb.Append("   @ProductTypeId ");
		//			sb.Append("   ,@CompositionId ");
		//			sb.Append("   ,@ProcessIdx ");
		//			sb.Append("   )");
		//			sb.Append(";");

		//			cmd.CommandText = sb.ToString();
		//			CommandText = cmd.CommandText;

		//			cmd.Parameters.Clear();
		//			cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;
		//			cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
		//			cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

		//			cmd.ExecuteNonQuery();
		//		}
		//	}
		//	catch (System.Data.SqlClient.SqlException ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("SqlException InsertProductTypeReserve", ex);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("Exception InsertProductTypeReserve", ex);
		//	}
		//}
		//#endregion

		//#region QueryProductTypeReserveTbl
		//private DataRow QueryProductTypeReserveTbl(
		//		SqlDBConnector dbConnector, int ProcessIdx)
		//{
		//	DataTable dt = new DataTable("ProductTypeReserve_Tbl");

		//	using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//	{
		//		StringBuilder sb = new StringBuilder();
		//		sb.Append("SELECT Top(1) T1.* ");
		//		sb.Append(" FROM ProductTypeReserve_Tbl T1 ");
		//		sb.Append(" WHERE T1.ProcessIdx = @ProcessIdx ");

		//		cmd.CommandText = sb.ToString();

		//		cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

		//		using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
		//		{
		//			adapter.Fill(dt);
		//		}
		//	}

		//	return dt.AsEnumerable().FirstOrDefault();
		//}
		//#endregion

		//#region ClearProductTypeReserve

		///// <summary>
		///// 
		///// </summary>
		///// <param name="dbConnector"></param>
		//public void ClearProductTypeReserve(
		//	SqlDBConnector dbConnector, int ProcessIdx = -1)
		//{
		//	try
		//	{
		//		using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//		{
		//			StringBuilder sb = new StringBuilder();

		//			sb.Append("DELETE FROM ProductTypeReserve_Tbl ");
		//			if(ProcessIdx >= 0)
		//				sb.Append(" WHERE ProcessIdx = @ProcessIdx  ");

		//			cmd.CommandText = sb.ToString();

		//			cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

		//			cmd.ExecuteNonQuery();
		//		}
		//	}
		//	catch (System.Data.SqlClient.SqlException ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("SqlException ClearProductTypeReserve", ex);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("Exception ClearProductTypeReserve", ex);
		//	}
		//}
		//#endregion

		#region XXXX TakeOverProductNumber

		///// <summary>
		///// 品番引継
		/////  直変更時
		/////  編成変更時
		///// </summary>
		///// <param name="dbConnector"></param>
		///// <param name="CompositionId"></param>
		//public void TakeOverProductNumber(
		//		SqlDBConnector dbConnector,
		//		Guid CompositionId)
		//{
		//	try
		//	{
		//		using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//		{
		//			StringBuilder sb = new StringBuilder();

		//			cmd.CommandType = CommandType.StoredProcedure;
		//			cmd.CommandText = "TakeOverProductNumber";
		//			cmd.Parameters.AddWithValue("@CompositionId", CompositionId);

		//			DataSet dataSet = new DataSet();
		//			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
		//			{
		//				adapter.Fill(dataSet);
		//			}
		//		}
		//	}
		//	catch (System.Data.SqlClient.SqlException ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("SqlException TakeOverWorkerWhenCompositionChanged", ex);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("Exception TakeOverWorkerWhenCompositionChanged", ex);
		//	}
		//}
		#endregion

		#endregion
		///////////////////////////////////////////
		/// Worker

		#region InsertWorkerResults

		public void InsertWorkerResults(
				SqlDBConnector dbConnector,
				Guid WorkerId,
				Guid CompositionId,
				int ProcessIdx,
				DateTime StartTime)
		{
			try
			{
				String CommandText = "";

				// 3:作業者(工程毎)変更
				//		コード1 = 作業者ID
				//		コード2 = 工程インデックス
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					// 前回開始したレコードを終了
					sb.Append("UPDATE WorkerResults_Tbl SET ");
					sb.Append("  EndTime = @StartTime ");
					sb.Append(" WHERE ProcessIdx = @ProcessIdx ");
					sb.Append("   AND EndTime IS NULL ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();

					// 新たな開始レコード追加
					sb.Clear();
					sb.Append("MERGE INTO WorkerResults_Tbl AS T1  ");
					sb.Append("  USING ");
					sb.Append("    (SELECT ");
					sb.Append("      @WorkerId AS WorkerId ");
					sb.Append("      ,@CompositionId AS CompositionId ");
					sb.Append("      ,@ProcessIdx AS ProcessIdx ");
					sb.Append("      ,@StartTime AS StartTime");
					sb.Append("    ) AS T2");
					sb.Append("  ON (");
					sb.Append("   T1.WorkerId = T2.WorkerId ");
					sb.Append("   AND T1.CompositionId = T2.CompositionId ");
					sb.Append("   AND T1.ProcessIdx = T2.ProcessIdx ");
					sb.Append("   AND T1.StartTime = T2.StartTime ");
					sb.Append("  )");
					sb.Append(" WHEN NOT MATCHED THEN ");
					sb.Append("  INSERT (");
					sb.Append("   WorkerId");
					sb.Append("   ,CompositionId");
					sb.Append("   ,ProcessIdx");
					sb.Append("   ,StartTime");
					sb.Append("   ) VALUES (");
					sb.Append("   @WorkerId ");
					sb.Append("   ,@CompositionId ");
					sb.Append("   ,@ProcessIdx ");
					sb.Append("   ,@StartTime ");
					sb.Append("   )");
					sb.Append(";");

					cmd.CommandText = sb.ToString();
					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add("@WorkerId", SqlDbType.UniqueIdentifier).Value = WorkerId;
					cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
					cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;
					cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = StartTime;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException InsertWorkerResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception InsertWorkerResults", ex);
			}
		}
		#endregion

		#region FinishWorkerResults
		public void FinishWorkerResults(
				SqlDBConnector dbConnector, DateTime EndTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "FinishWorkerResults";
					cmd.Parameters.AddWithValue("@EndTime", EndTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException FinishWorkerResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception FinishWorkerResults", ex);
			}
		}
		#endregion

		#region XXXXXX TakeOverWorker

		///// <summary>
		///// 作業者引継
		/////  編成変更時
		///// </summary>
		///// <param name="dbConnector"></param>
		///// <param name="CompositionId"></param>
		//public void TakeOverWorker(
		//		SqlDBConnector dbConnector,
		//		Guid CompositionId)
		//{
		//	try
		//	{
		//		using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//		{
		//			StringBuilder sb = new StringBuilder();

		//			cmd.CommandType = CommandType.StoredProcedure;
		//			cmd.CommandText = "TakeOverWorker";
		//			cmd.Parameters.AddWithValue("@CompositionId", CompositionId);

		//			DataSet dataSet = new DataSet();
		//			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
		//			{
		//				adapter.Fill(dataSet);
		//			}
		//		}
		//	}
		//	catch (System.Data.SqlClient.SqlException ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("SqlException TakeOverWorkerWhenCompositionChanged", ex);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("Exception TakeOverWorkerWhenCompositionChanged", ex);
		//	}
		//}
		#endregion

		#region ShiftChangedWorkerResults
		public void ShiftChangedWorkerResults(
				SqlDBConnector dbConnector, DateTime StartTime)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "ShiftChangedWorkerResults";
					cmd.Parameters.AddWithValue("@StartTime", StartTime);

					DataSet dataSet = new DataSet();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dataSet);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException ShiftChangedWorkerResults", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception ShiftChangedWorkerResults", ex);
			}
		}
		#endregion

		///////////////////////////////////////////
		/// CycleTime
		#region CreateCycleVideoSub

		private void CreateCycleVideoSub(
					SqlDBConnector dbConnector
					, Guid CompositionId
					, int ProcessIdx
					, DateTime StartTime
					, DateTime EndTime
					)
		{
			VideoRetrievalContWCFSender videoSender = new VideoRetrievalContWCFSender(VideoRetrievalWCFAddress);

			foreach (var drCameraItem
				in QueryCurrentCamera(dbConnector, CompositionId).AsEnumerable().Where(x =>
					(int)x.Field<Int32>("ProcessIdx") == ProcessIdx))
			{
				InsertCycleVideoCreatTimeTbl(
					dbConnector,
					CompositionId,
					ProcessIdx,
					StartTime,
					drCameraItem.Field<Int32>("CameraIdx"),
					0,
					StartTime,
					EndTime
					);

				InsertCycleVideoCreatOriginalTbl(
						dbConnector,
						CompositionId,
						ProcessIdx,
						StartTime,
						EndTime,
						drCameraItem.Field<Int32>("CameraIdx"),
						drCameraItem.Field<Guid>("DeviceId")
						);

				InsertCycleVideoTbl(
						dbConnector,
						CompositionId,
						ProcessIdx,
						StartTime,
						drCameraItem.Field<Int32>("CameraIdx"),
						EndTime.AddMinutes(CycleVideoMarginMinutes)
						);

				// 動画切出しサービス呼び出し
				videoSender.CreateCycleVideo(
					CompositionId,
					ProcessIdx,
					drCameraItem.Field<Int32>("CameraIdx"),
					StartTime);
			}
		}
		#endregion

		#region CreateCycleVideo

		private void CreateCycleVideo(
					SqlDBConnector dbConnector
					, Guid CompositionId
					, int ProcessIdx
					, DateTime EventTime)
		{
			var drPreviousCycleTime = QueryPreviousCycleTimeResultsTbl(
					dbConnector,
					CompositionId,
					ProcessIdx,
					EventTime);
			if (drPreviousCycleTime != null)
			{
				CreateCycleVideoSub(
					dbConnector
					, CompositionId
					, ProcessIdx
					, drPreviousCycleTime.Field<DateTime>("StartTime")
					, drPreviousCycleTime.Field<DateTime>("EndTime")
					);
			}
		}
		#endregion

		#region TruncationCycleTime

		/// <summary>
		/// サイクルタイム切捨て
		/// </summary>
		/// <param name="dbConnector"></param>
		public void TruncationCycleTime(SqlDBConnector dbConnector)
		{
			try
			{
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("DELETE FROM CycleTimeResults_Tbl ");
					sb.Append(" WHERE EndTime IS NULL  ");

					cmd.CommandText = sb.ToString();

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException TruncationCycleTime", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception TruncationCycleTime", ex);
			}
		}
		#endregion

		#region StartNewCycle
		public void StartNewCycle(
				SqlDBConnector dbConnector
				, Guid CompositionId
				, int ProcessIdx
				, DateTime StartTime)
		{
			try
			{
				// 生産数カウンター加算
				IncrementOperatingShiftProductionQuantityTbl(
					dbConnector
					, CompositionId
					, ProcessIdx
					, StartTime);

				InsertCycleTimeResults(
					dbConnector
					, CompositionId
					, ProcessIdx
					, StartTime
					);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException StartNewCycle", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception StartNewCycle", ex);
			}
		}
		#endregion

		#region FinishPreviousCycle
		public void FinishPreviousCycle(
				SqlDBConnector dbConnector
				, Guid CompositionId
				, int ProcessIdx
				, DateTime EndTime)
		{
			try
			{
				FinishPreviousCycleTimeResults(
					dbConnector
					, CompositionId
					, ProcessIdx
					, EndTime
					);

				CreateCycleVideo(
					dbConnector
					, CompositionId
					, ProcessIdx
					, EndTime
					);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException FinishPreviousCycle", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception FinishPreviousCycle", ex);
			}
		}
		#endregion

		#region XXXXX SetInOperationCycleTimeEnd
		///// <summary>
		///// 
		///// </summary>
		///// <param name="dbConnector"></param>
		///// <param name="EndTime"></param>
		//public void SetInOperationCycleTimeEnd(
		//		SqlDBConnector dbConnector,
		//		DateTime EndTime)
		//{
		//	try
		//	{
		//		using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//		{
		//			StringBuilder sb = new StringBuilder();

		//			cmd.CommandType = CommandType.StoredProcedure;
		//			cmd.CommandText = "SetInOperationCycleTimeEnd";
		//			cmd.Parameters.AddWithValue("@EndTime", EndTime);

		//			DataSet dataSet = new DataSet();
		//			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
		//			{
		//				adapter.Fill(dataSet);
		//			}
		//		}
		//	}
		//	catch (System.Data.SqlClient.SqlException ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("SqlException SetInOperationCycleTimeEnd", ex);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.Logger.Instance.WriteTraceLog("Exception SetInOperationCycleTimeEnd", ex);
		//	}
		//}
		#endregion

		#region RemoveEventData

		/// <summary>
		/// 期限切れサイクルビデオレコード & Video　データ削除
		/// </summary>
		/// <param name="dbConnector"></param>
		private void RemoveEventData(
				SqlDBConnector dbConnector, int ExpiredDay)
		{
			try
			{
				Common.Logger.Instance.WriteTraceLog($"[RemoveEventData] ");
				DataTable dt = new DataTable("Event_Tbl");
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("SELECT T1.* ");
					sb.Append(" FROM Event_Tbl T1 ");
					sb.Append(" WHERE ");
					sb.Append("   T1.EventType  = 2 ");
					sb.Append("   And dateadd(day, @ExpiredDay, T1.EventTime) < SYSDATETIME() ");

					cmd.CommandText = sb.ToString();
					cmd.Parameters.Add(new SqlParameter("@ExpiredDay", SqlDbType.Int)).Value = ExpiredDay;

					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dt);
					}

					foreach (DataRow dr in dt.AsEnumerable())
					{
						String VideoFileName = dr.Field<String>("EventData");
						if (!String.IsNullOrWhiteSpace(VideoFileName))
						{
							String VideoFileFullPath = System.IO.Path.Combine(RowVideoPath, VideoFileName);
							if (System.IO.File.Exists(VideoFileFullPath))
							{
								try
								{
									System.IO.File.Delete(VideoFileFullPath);
									Common.Logger.Instance.WriteTraceLog($"[RemoveEventData] {VideoFileFullPath}");
								}
								catch { }
							}
						}
					}

					sb.Clear();
					sb.Append("DELETE T1 ");
					sb.Append(" FROM Event_Tbl T1 ");
					sb.Append(" WHERE ");
					sb.Append("  dateadd(day, @ExpiredDay, T1.EventTime) < SYSDATETIME() ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException RemoveEventData", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception RemoveEventData", ex);
			}
		}
		#endregion

		#region RemoveExpiredVideo

		/// <summary>
		/// 保存期間の過ぎた動画ファイル等を削除
		/// </summary>
		/// <param name="dbConnector"></param>
		private void RemoveExpiredVideo(
				SqlDBConnector dbConnector)
		{
			try
			{
				Common.Logger.Instance.WriteTraceLog($"[RemoveExpiredVideo] ");
				DataTable dt = new DataTable("CycleVideo_Tbl");
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					int MoveRetentionDays = 0;
					DataTable dtKeyValueMst_RetentionPeriod = QueryKeyValueMst(dbConnector, "データ保存期間");
					if (int.TryParse(
								dtKeyValueMst_RetentionPeriod.AsEnumerable().FirstOrDefault(
									x => x.Field<String>("Name") == "動画")?.Field<String>("Value").ToString() ?? "",
								out MoveRetentionDays) && MoveRetentionDays > 0)
					{

						StringBuilder sb = new StringBuilder();
						sb.Append("SELECT T1.* ");
						sb.Append(" FROM CycleVideo_Tbl T1 ");
						sb.Append(" WHERE ");
						sb.Append("   T1.CreationStatus  = 1 ");
						sb.Append("   And dateadd(day, @MoveRetentionDays, T1.StartTime) < SYSDATETIME() ");

						cmd.CommandText = sb.ToString();
						cmd.Parameters.Add("@MoveRetentionDays", SqlDbType.Int).Value = MoveRetentionDays;

						using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}

						foreach (DataRow dr in dt.AsEnumerable())
						{
							String VideoFileName = dr.Field<String>("VideoFileName");
							if (!String.IsNullOrWhiteSpace(VideoFileName))
							{
								String VideoFileFullPath = System.IO.Path.Combine(CycleVideoPath, VideoFileName);
								if (System.IO.File.Exists(VideoFileFullPath))
								{
									try
									{
										System.IO.File.Delete(VideoFileFullPath);
										Common.Logger.Instance.WriteTraceLog($"[RemoveExpiredVideo] {VideoFileFullPath}");
									}
									catch { }
								}
							}
						}

						sb.Clear();
						sb.Append("DELETE T1 ");
						sb.Append("  FROM CycleVideo_Tbl T1 ");
						sb.Append(" WHERE ");
						sb.Append("   dateadd(day, @MoveRetentionDays, T1.StartTime) < SYSDATETIME() ");

						cmd.CommandText = sb.ToString();

						CommandText = cmd.CommandText;
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@MoveRetentionDays", SqlDbType.Int).Value = MoveRetentionDays;

						cmd.ExecuteNonQuery();

						sb.Clear();
						sb.Append("DELETE T1 ");
						sb.Append("  FROM CycleVideoCreat_Original_Tbl T1 ");
						sb.Append(" WHERE ");
						sb.Append("   dateadd(day, @MoveRetentionDays, T1.StartTime) < SYSDATETIME() ");

						cmd.CommandText = sb.ToString();

						CommandText = cmd.CommandText;
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@MoveRetentionDays", SqlDbType.Int).Value = MoveRetentionDays;

						cmd.ExecuteNonQuery();

						sb.Clear();
						sb.Append("DELETE T1 ");
						sb.Append("  FROM CycleVideoCreat_Time_Tbl T1 ");
						sb.Append(" WHERE ");
						sb.Append("   dateadd(day, @MoveRetentionDays, T1.StartTime) < SYSDATETIME() ");

						cmd.CommandText = sb.ToString();

						CommandText = cmd.CommandText;
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@MoveRetentionDays", SqlDbType.Int).Value = MoveRetentionDays;

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException RemoveExpiredVideo", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception RemoveExpiredVideo", ex);
			}
		}
		#endregion

		#region RemoveExpiredCycle

		/// <summary>
		/// 期限切れサイクルデータ削除
		/// </summary>
		/// <param name="dbConnector"></param>
		private void RemoveExpiredCycle(
				SqlDBConnector dbConnector)
		{
			try
			{
				String CommandText = "";

				Common.Logger.Instance.WriteTraceLog($"[RemoveExpiredCycle] ");
				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					int CTResultRetentionDays = 0;
					DataTable dtKeyValueMst_RetentionPeriod = QueryKeyValueMst(dbConnector, "データ保存期間");
					if (int.TryParse(
								dtKeyValueMst_RetentionPeriod.AsEnumerable().FirstOrDefault(
									x => x.Field<String>("Name") == "実績CT")?.Field<String>("Value").ToString() ?? "",
								out CTResultRetentionDays) && CTResultRetentionDays > 0)
					{

						StringBuilder sb = new StringBuilder();

						sb.Append("DELETE T1 ");
						sb.Append("  FROM CycleTimeResults_Tbl T1 ");
						sb.Append(" WHERE ");
						sb.Append("   dateadd(day, @CTResultRetentionDays, T1.StartTime) < SYSDATETIME() ");

						cmd.CommandText = sb.ToString();
						cmd.Parameters.Add("@CTResultRetentionDays", SqlDbType.Int).Value = CTResultRetentionDays;

						CommandText = cmd.CommandText;

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				Common.Logger.Instance.WriteTraceLog("SqlException RemoveExpiredCycle", ex);
			}
			catch (Exception ex)
			{
				Common.Logger.Instance.WriteTraceLog("Exception RemoveExpiredCycle", ex);
			}
		}
		#endregion

		#region QueryKeyValueMst
		private DataTable QueryKeyValueMst(
				SqlDBConnector dbConnector, String KeyName)
		{
			DataTable dt = new DataTable("KeyValue_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append(" KeyName");
				sb.Append(" ,[Value]");
				sb.Append(" ,[Name]");
				sb.Append(" ,OrderNo");
				sb.Append(" FROM KeyValue_Mst ");
				sb.Append(" WHERE KeyName = @KeyName ");
				sb.Append("  Order By OrderNo ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add("@KeyName", SqlDbType.NVarChar).Value = KeyName;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}

		#endregion

		#region ClearCameraActive

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbConnector"></param>
		public void ClearCameraActive(SqlDBConnector dbConnector)
		{
			ChangeCameraActive(dbConnector, Guid.Empty);
		}
		#endregion

		#region ChangeCameraActive

		public void ChangeCameraActive(SqlDBConnector dbConnector, Guid CompositionId)
		{
			WebcamServiceContWCFSender webcamSender = new WebcamServiceContWCFSender(WebcamServiceWCFAddress);

			Common.Logger.Instance.WriteTraceLog($"[ChangeCameraActive] CompositionId = {CompositionId}");
			UpdateCameraActiveTbl(dbConnector, CompositionId);
			// 構成変更時に呼び出し
			webcamSender.NotifyCameraChange();
		}
		#endregion

		#region MonitoringTrigger

		private void MonitoringElapsed()
		{
			SqlDBConnector dbConnector = new SqlDBConnector(ConnectionString);

			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				DataTable dtSpecifiedTimeProcessingTbl = QueryUnProcessingSpecifiedTimeProcessingTbl(dbConnector);

				foreach (var drProcessing in dtSpecifiedTimeProcessingTbl.AsEnumerable())
				{
					int ProcessingType = drProcessing.Field<Int32>("ProcessingType");
					Common.Logger.Instance.WriteTraceLog($"[MonitoringElapsed] 処理データタイプ = {ProcessingType}");
					switch (ProcessingType)
					{
						case 1: //直開始
							{
								// 編成引継(終了時刻が直近の編成を引継ぐ)
								RestoreCompositionResults(dbConnector
									, drProcessing.Field<DateTime>("SpecifiedTime"));
								// Step2 品番タイプ変更 品番引継
								RestoreProductTypeResults(dbConnector
									, drProcessing.Field<DateTime>("SpecifiedTime")
									, drProcessing.Field<DateTime>("SpecifiedTime"));
								// Step2 対応済 作業者"未設定"(という名前の作業者)を必要工程に割当てる
								ShiftChangedWorkerResults(dbConnector, drProcessing.Field<DateTime>("SpecifiedTime"));
								// Step2 アクティブカメラ変更
								ChangeCameraActive(dbConnector, GetCompositionId(dbConnector));

								Guid CompositionId = GetCompositionId(dbConnector);
								DataTable dtProcess = QueryCompositionProcessMst(dbConnector, CompositionId, true);

								foreach (var processItem in dtProcess.AsEnumerable())
								{
									StartNewCycle(
										dbConnector,
										CompositionId,
										processItem.Field<Int32>("ProcessIdx"),
										drProcessing.Field<DateTime>("SpecifiedTime"));
								}
							}
							break;
						case 2: //直終了
							{
								Guid CompositionId = GetCompositionId(dbConnector);
								DataTable dtProcess = QueryCompositionProcessMst(dbConnector, CompositionId);
								foreach (var processItem in dtProcess.AsEnumerable())
								{
									// サイクルを完了
									FinishPreviousCycle(
										dbConnector,
										CompositionId,
										processItem.Field<Int32>("ProcessIdx"),
										drProcessing.Field<DateTime>("SpecifiedTime"));
								}
								// 作業者終了
								FinishWorkerResults(dbConnector, drProcessing.Field<DateTime>("SpecifiedTime"));
								// 品番終了
								FinishProductTypeResults(dbConnector, drProcessing.Field<DateTime>("SpecifiedTime"));
								// 編成終了
								FinishCompositionResults(dbConnector, drProcessing.Field<DateTime>("SpecifiedTime"));
								// 編成録画停止
								// 動画ファイル等を削除
								ClearCameraActive(dbConnector);

								// EventData保存期間
								RemoveEventData(dbConnector, 31);
								// 保存期間の過ぎた動画ファイル等を削除
								RemoveExpiredVideo(dbConnector);
								// 保存期間の過ぎたサイクルビデオファイル等を削除
								RemoveExpiredCycle(dbConnector);
							}
							break;
						default:
							break;
					}

					UpdateSpecifiedTimeProcessingTblStatus(
						dbConnector,
						drProcessing.Field<Guid>("ProcessingId"),
						2);
				}

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}
		}

		/// <summary>
		/// Start Monitoring Trigger
		/// </summary>
		public void StartMonitoringTrigger()
		{
			Common.Logger.Instance.WriteTraceLog($"[StartMonitoringTrigger] 開始");
			_BreakMonitoring = false;
			_MonitoringTimer.Start();
		}

		/// <summary>
		/// Stop Monitoring Trigger
		/// </summary>
		public void StopMonitoringTrigger()
		{
			Common.Logger.Instance.WriteTraceLog($"[StopMonitoringTrigger] 停止");
			_BreakMonitoring = true;
			_MonitoringTimer.Stop();
		}
		#endregion

		#region GetDOPort
		private Int32 GetDOPort(
						SqlDBConnector dbConnector,
						Guid ProductTypeId,
						Guid CompositionId,
						int ProcessIdx
						)
		{
			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT ");
				sb.Append(" T1.* ");
				sb.Append(" ,T2.DevicePhysicalIdentifier ");
				sb.Append(" FROM DataCollection_DO_Mst T1 ");
				sb.Append(" INNER JOIN ConnectionDevice_Mst T2 ON ");
				sb.Append("  T2.DeviceId = T1.DeviceId ");
				sb.Append(" WHERE ProductTypeId = @ProductTypeId ");
				sb.Append("   AND CompositionId = @CompositionId ");
				sb.Append("   AND ProcessIdx = @ProcessIdx ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = ProductTypeId;
				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
				cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						int iPort;
						if(int.TryParse(
								reader.GetString(reader.GetOrdinal("DevicePhysicalIdentifier")), 
								out iPort)) 
							return iPort;
					}
				}
			}
			return QROKPortNo;
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Dispose (IDisposable Member)
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing">true:ﾃﾞｽﾄﾗｸﾀ以外 false:ﾃﾞｽﾄﾗｸﾀ</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				StopMonitoringTrigger();
			}
		}
		#endregion
	}
}
