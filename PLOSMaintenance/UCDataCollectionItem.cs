using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBConnect;

namespace PLOSMaintenance
{
	public partial class UCDataCollectionItem : UserControl
	{
		private Guid CompositionId { get; set; } = Guid.Empty;
		private int ProcessIdx = -1;
		private String ProcessName;
		private int TriggerType;
		private int IsProductionQuantityCounter;
		private DataTable _dtDeviceWithCameraUseList;
		private BindingSource _BindingSourceDeviceWithCameraUse;

		private List<object> _TriggerTypeList = new List<object> {
				new { Value = 1, Name = "ONエッジ" },
				new { Value = 2, Name = "AND" },
				new { Value = 3, Name = "ONメモリ" },
				new { Value = 4, Name = "OFFエッジ" },
				new { Value = 5, Name = "OR" },
				//new { Value = 0, Name = "設定無し" },
				};

		public UCDataCollectionItem()
		{
			InitializeComponent();

			InitializeComponentData();
		}

		public UCDataCollectionItem(DataRow dr)
		{

			this.CompositionId = dr.Field<Guid>("CompositionId");
			ProcessIdx = dr.Field<Int32>("ProcessIdx");
			ProcessName = dr.Field<String>("ProcessName");
			TriggerType = dr.Field<Int32>("TriggerType");
			IsProductionQuantityCounter = dr.Field<Int32>("IsProductionQuantityCounter");

			InitializeComponent();

			InitializeComponentData();
		}

		private void InitializeComponentData()
		{
			if (CompositionId == Guid.Empty || ProcessIdx < 0) return;

			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				DataTable dtConnectionDevice = QueryConnectionDeviceMst(dbConnector);

				var TriggerDeviceList1 =
					(
					from item in dtConnectionDevice.AsEnumerable()
					where item.Field<Int32>("DeviceType") == 0
					select new { Value = item.Field<Guid>("DeviceId"), Name = item.Field<String>("DeviceRemark") }
					).ToList();

				var TriggerDeviceList2 =
					(
					from item in dtConnectionDevice.AsEnumerable()
					where item.Field<Int32>("DeviceType") == 0
					select new { Value = item.Field<Guid>("DeviceId"), Name = item.Field<String>("DeviceRemark") }
					).ToList();

				var DODeviceList =
					(
					from item in dtConnectionDevice.AsEnumerable()
					where item.Field<Int32>("DeviceType") == 3
					select new { Value = item.Field<Guid>("DeviceId"), Name = item.Field<String>("DeviceRemark") }
					).ToList();

				//DataTable dtTriggerSensor = QueryDataCollectionTriggerSensorMst(dbConnector);
				//cmbSensorDevice_1.DataSource = TriggerDeviceList1;
				////実際の値が"Value"列、表示するテキストが"Display"列とする
				//cmbSensorDevice_1.ValueMember = "Value";
				//cmbSensorDevice_1.DisplayMember = "Name";

				//if (dtTriggerSensor.Rows.Count > 0 && !dtTriggerSensor.Rows[0].IsNull("SensorDeviceId"))
				//{
				//	cmbSensorDevice_1.SelectedValue = dtTriggerSensor.Rows[0].Field<Guid>("SensorDeviceId");
				//	NmSensorFilter_1.Value = dtTriggerSensor.Rows[0].Field<Int32>("SensorChatteringFilter");
				//}

				//cmbSensorDevice_2.DataSource = TriggerDeviceList2;
				////実際の値が"Value"列、表示するテキストが"Display"列とする
				//cmbSensorDevice_2.ValueMember = "Value";
				//cmbSensorDevice_2.DisplayMember = "Name";
				//if (dtTriggerSensor.Rows.Count > 1 && !dtTriggerSensor.Rows[1].IsNull("SensorDeviceId"))
				//{
				//	cmbSensorDevice_2.SelectedValue = dtTriggerSensor.Rows[1].Field<Guid>("SensorDeviceId");
				//	NmSensorFilter_2.Value = dtTriggerSensor.Rows[1].Field<Int32>("SensorChatteringFilter");
				//}

				cmbDODevice.DataSource = DODeviceList;
				cmbDODevice.ValueMember = "Value";
				cmbDODevice.DisplayMember = "Name";

				DataTable dtDO = QueryDataCollectionDOMst(dbConnector);
				if (dtDO.Rows.Count > 0 && !dtDO.Rows[0].IsNull("DeviceId"))
				{
					cmbDODevice.SelectedValue = dtDO.Rows[0].Field<Guid>("DeviceId");
				}

				////////////////////////////////////////////////////////////////////////////

				_dtDeviceWithCameraUseList = QueryDeviceWithCameraUseMst(dbConnector);

				_BindingSourceDeviceWithCameraUse = new BindingSource(_dtDeviceWithCameraUseList, String.Empty);

				dgvCamera.Columns.Clear();
				dgvCamera.AutoGenerateColumns = false;
				dgvCamera.DataSource = _BindingSourceDeviceWithCameraUse;

				DataGridViewCheckBoxColumn chkbxUseColumn = new DataGridViewCheckBoxColumn();
				chkbxUseColumn.DataPropertyName = "CameraUse";
				chkbxUseColumn.Name = "CameraUse";
				chkbxUseColumn.HeaderText = "使用";
				chkbxUseColumn.Width = 50;
				dgvCamera.Columns.Add(chkbxUseColumn);

				DataGridViewTextBoxColumn txtDeviceIdColumn = new DataGridViewTextBoxColumn();
				txtDeviceIdColumn.DataPropertyName = "DeviceId";
				txtDeviceIdColumn.Name = "DeviceId";
				txtDeviceIdColumn.Visible = false;
				dgvCamera.Columns.Add(txtDeviceIdColumn);

				DataGridViewTextBoxColumn txtCameraNameColumn = new DataGridViewTextBoxColumn();
				txtCameraNameColumn.DataPropertyName = "DeviceRemark";
				txtCameraNameColumn.Name = "DeviceRemark";
				txtCameraNameColumn.HeaderText = "カメラ名";
				//txtCameraNameColumn.Width = 160;
				txtCameraNameColumn.FillWeight = 80;
				txtCameraNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				txtCameraNameColumn.ReadOnly = true;
				dgvCamera.Columns.Add(txtCameraNameColumn);

				DataGridViewCheckBoxColumn chkbxCameraConditionColumn = new DataGridViewCheckBoxColumn();
				chkbxCameraConditionColumn.DataPropertyName = "CameraCondition";
				chkbxCameraConditionColumn.Name = "CameraCondition";
				chkbxCameraConditionColumn.HeaderText = "異常のみ";
				chkbxCameraConditionColumn.Width = 70;
				dgvCamera.Columns.Add(chkbxCameraConditionColumn);

				///////////////////////////////////////////////////////////////////////

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}

			txtProcessName.Text = ProcessName;

			if (IsProductionQuantityCounter == 0)
			{
				_TriggerTypeList.Add(new { Value = 0, Name = "設定無し" });
			}

			cmbTriggerType.DataSource = _TriggerTypeList;
			//実際の値が"Value"列、表示するテキストが"Display"列とする
			cmbTriggerType.ValueMember = "Value";
			cmbTriggerType.DisplayMember = "Name";
			cmbTriggerType.SelectedValue = TriggerType;
			cmbTriggerType.SelectedIndexChanged += OnTriggerType_SelectedIndexChanged;

			if (IsProductionQuantityCounter > 0)
			{
				lblDO.Visible = false;
				cmbDODevice.Visible = false;
				dgvCamera.Visible = false;
			}

			//NumberOfPeople.Value = _dtProcess.Rows.Count;
			SensorTypeChanged();
		}

		private void OnTriggerType_SelectedIndexChanged(object sender, EventArgs e)
		{
			SensorTypeChanged();
		}

		private void SensorTypeChanged()
		{
			cmbSensorDevice_1.Visible = NmSensorFilter_1.Visible = lblSensorFillterUnit_1.Visible
				= lblSensorTitle.Visible = lblSensorFillterTitle.Visible = (int)cmbTriggerType.SelectedValue != 0;
			dgvCamera.Visible = ((int)cmbTriggerType.SelectedValue != 0) && IsProductionQuantityCounter == 0;
			cmbSensorDevice_2.Visible = NmSensorFilter_2.Visible = lblSensorFillterUnit_2.Visible 
				= (cmbTriggerType.SelectedIndex != 0 && cmbTriggerType.SelectedIndex != 3 && (int)cmbTriggerType.SelectedValue != 0);

			switch (cmbTriggerType.SelectedIndex)
			{
				case 0:
				default:
					picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType1;
					break;
				case 1:
					picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType2;
					break;
				case 2:
					picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType3;
					break;
				case 3:
					picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType4;
					break;
				case 4:
					picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType5;
					break;
				case 5:
					picSensor.Image = null;
					break;
			}
		}

		#region QueryConnectionDeviceMst
		private DataTable QueryConnectionDeviceMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("ConnectionDevice_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" FROM ConnectionDevice_Mst T1 ");
				//sb.Append(" WHERE T1.Deleted = 0 ");
				sb.Append("  Order by DeviceType, DeviceId ");

				cmd.CommandText = sb.ToString();

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryDeviceWithCameraUseMst
		private DataTable QueryDeviceWithCameraUseMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("ConnectionDeviceWithCameraUse_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" ,CASE WHEN T2.[CompositionId] IS NULL THEN 0 ELSE 1 END [CameraUse] ");
				sb.Append(" ,CASE WHEN T2.[CameraCondition] IS NULL THEN 0 ELSE 1 END [CameraCondition] ");
				sb.Append(" FROM ConnectionDevice_Mst T1 ");
				sb.Append(" LEFT JOIN DataCollection_Camera_Mst T2 ");
				sb.Append(" ON   T2.DeviceId = T1.DeviceId ");
				sb.Append("  AND T2.CompositionId = @CompositionId ");
				sb.Append("  AND T2.ProcessIdx = @ProcessIdx ");
				sb.Append(" WHERE T1.DeviceType = 2 ");
				sb.Append("  Order by T1.DeviceId ");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
				cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region QueryDataCollectionDOMst
		private DataTable QueryDataCollectionDOMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("DataCollection_DO_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append("  T1.*");
				sb.Append(" FROM DataCollection_DO_Mst T1 ");
				sb.Append(" WHERE T1.CompositionId = @CompositionId ");
				sb.Append("   AND T1.ProcessIdx = @ProcessIdx");

				cmd.CommandText = sb.ToString();

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@CompositionId", SqlDbType.UniqueIdentifier).Value = CompositionId;
				cmd.Parameters.Add("@ProcessIdx", SqlDbType.Int).Value = ProcessIdx;

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region UpdateDataCollectionTriggerMst
		private void UpdateDataCollectionTriggerMst(SqlDBConnector dbConnector)
		{
			String CommandText = "";
			StringBuilder sb = new StringBuilder();

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				sb.Clear();
				sb.Append("MERGE INTO DataCollection_Trigger_Mst AS T1  ");
				sb.Append("  USING ");
				sb.Append("    (SELECT ");
				sb.Append("      @CompositionId AS CompositionId ");
				sb.Append("      ,@ProcessIdx AS ProcessIdx");
				sb.Append("    ) AS T2");
				sb.Append("  ON (");
				sb.Append("   T1.CompositionId = @CompositionId ");
				sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("  )");
				sb.Append(" WHEN MATCHED THEN ");
				sb.Append("  UPDATE SET ");
				sb.Append("   TriggerType = @TriggerType");
				sb.Append(" WHEN NOT MATCHED THEN ");
				sb.Append("  INSERT (");
				sb.Append("   CompositionId");
				sb.Append("   ,ProcessIdx");
				sb.Append("   ,TriggerType");
				sb.Append("   ,IsProductionQuantityCounter");
				sb.Append("   ) VALUES (");
				sb.Append("   @CompositionId ");
				sb.Append("   ,@ProcessIdx ");
				sb.Append("   ,@TriggerType ");
				sb.Append("   ,@IsProductionQuantityCounter ");
				sb.Append("   )");
				sb.Append(";");

				cmd.CommandText = sb.ToString();

				CommandText = cmd.CommandText;

				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
				cmd.Parameters.Add(new SqlParameter("@TriggerType", SqlDbType.Int)).Value = cmbTriggerType.SelectedValue;
				cmd.Parameters.Add(new SqlParameter("@IsProductionQuantityCounter", SqlDbType.Int)).Value = IsProductionQuantityCounter;

				cmd.ExecuteNonQuery();
			}
		}

		#endregion

		#region RemoveDataCollectionTriggerSensorMst

		private void RemoveDataCollectionTriggerSensorMst(
				SqlDBConnector dbConnector)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("DELETE T1 ");
					sb.Append("  FROM DataCollection_Trigger_Sensor_Mst T1 ");
					sb.Append(" WHERE ");
					sb.Append("       T1.CompositionId = @CompositionId ");
					sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
					cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				//Common.Logger.Instance.WriteTraceLog("SqlException RemoveDataCollectionTriggerSensorMst", ex);
			}
			catch (Exception ex)
			{
				//Common.Logger.Instance.WriteTraceLog("Exception RemoveDataCollectionTriggerSensorMst", ex);
			}
		}
		#endregion

		#region XXXXX UpdateDataCollectionTriggerSensorMst
		//private void UpdateDataCollectionTriggerSensorMst(SqlDBConnector dbConnector
		//				,int SensorIdx, int SensorChatteringFilter, Guid DeviceId)
		//{
		//	String CommandText = "";
		//	StringBuilder sb = new StringBuilder();

		//	using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
		//	{
		//		sb.Clear();
		//		sb.Append("MERGE INTO DataCollection_Trigger_Sensor_Mst AS T1  ");
		//		sb.Append("  USING ");
		//		sb.Append("    (SELECT ");
		//		sb.Append("       @CompositionId AS CompositionId ");
		//		sb.Append("      ,@ProcessIdx AS ProcessIdx");
		//		sb.Append("      ,@SensorIdx AS SensorIdx");
		//		sb.Append("    ) AS T2");
		//		sb.Append("  ON (");
		//		sb.Append("       T1.CompositionId = @CompositionId ");
		//		sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");
		//		sb.Append("   AND T1.SensorIdx = @SensorIdx ");
		//		sb.Append("  )");
		//		sb.Append(" WHEN MATCHED THEN ");
		//		sb.Append("  UPDATE SET ");
		//		sb.Append("   SensorChatteringFilter = @SensorChatteringFilter");
		//		sb.Append("   ,DeviceId = @DeviceId");
		//		sb.Append(" WHEN NOT MATCHED THEN ");
		//		sb.Append("  INSERT (");
		//		sb.Append("   CompositionId");
		//		sb.Append("   ,ProcessIdx");
		//		sb.Append("   ,SensorIdx");
		//		sb.Append("   ,SensorChatteringFilter");
		//		sb.Append("   ,DeviceId");
		//		sb.Append("   ) VALUES (");
		//		sb.Append("   @CompositionId ");
		//		sb.Append("   ,@ProcessIdx ");
		//		sb.Append("   ,@SensorIdx ");
		//		sb.Append("   ,@SensorChatteringFilter ");
		//		sb.Append("   ,@DeviceId ");
		//		sb.Append("   )");
		//		sb.Append(";");

		//		cmd.CommandText = sb.ToString();

		//		CommandText = cmd.CommandText;

		//		cmd.Parameters.Clear();
		//		cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
		//		cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
		//		cmd.Parameters.Add(new SqlParameter("@SensorIdx", SqlDbType.Int)).Value = SensorIdx;
		//		cmd.Parameters.Add(new SqlParameter("@SensorChatteringFilter", SqlDbType.Int)).Value = SensorChatteringFilter;
		//		cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;

		//		cmd.ExecuteNonQuery();
		//	}
		//}

		#endregion

		#region RemoveDataCollectionCameraMst

		private void RemoveDataCollectionCameraMst(
				SqlDBConnector dbConnector)
		{
			try
			{
				String CommandText = "";

				using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
				{
					StringBuilder sb = new StringBuilder();

					sb.Append("DELETE T1 ");
					sb.Append("  FROM DataCollection_Camera_Mst T1 ");
					sb.Append(" WHERE ");
					sb.Append("       T1.CompositionId = @CompositionId ");
					sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");

					cmd.CommandText = sb.ToString();

					CommandText = cmd.CommandText;

					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
					cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;

					cmd.ExecuteNonQuery();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				//Common.Logger.Instance.WriteTraceLog("SqlException RemoveExpiredVideo", ex);
			}
			catch (Exception ex)
			{
				//Common.Logger.Instance.WriteTraceLog("Exception RemoveExpiredVideo", ex);
			}
		}
		#endregion

		#region UpdateDataCollectionCameraMst
		private void UpdateDataCollectionCameraMst(SqlDBConnector dbConnector
						, int CameraIdx, string Camera, int CameraCondition, Guid DeviceId)
		{
			String CommandText = "";
			StringBuilder sb = new StringBuilder();

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				sb.Clear();
				sb.Append("MERGE INTO DataCollection_Camera_Mst AS T1  ");
				sb.Append("  USING ");
				sb.Append("    (SELECT ");
				sb.Append("       @CompositionId AS CompositionId ");
				sb.Append("      ,@ProcessIdx AS ProcessIdx");
				sb.Append("      ,@CameraIdx AS CameraIdx");
				sb.Append("    ) AS T2");
				sb.Append("  ON (");
				sb.Append("       T1.CompositionId = @CompositionId ");
				sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("   AND T1.CameraIdx = @CameraIdx ");
				sb.Append("  )");
				sb.Append(" WHEN MATCHED THEN ");
				sb.Append("  UPDATE SET ");
				sb.Append("   CameraCondition = @CameraCondition");
				sb.Append("   ,Camera = @Camera");
				sb.Append("   ,DeviceId = @DeviceId");
				sb.Append(" WHEN NOT MATCHED THEN ");
				sb.Append("  INSERT (");
				sb.Append("   CompositionId");
				sb.Append("   ,ProcessIdx");
				sb.Append("   ,CameraIdx");
				sb.Append("   ,Camera");
				sb.Append("   ,CameraCondition");
				sb.Append("   ,DeviceId");
				sb.Append("   ) VALUES (");
				sb.Append("   @CompositionId ");
				sb.Append("   ,@ProcessIdx ");
				sb.Append("   ,@CameraIdx ");
				sb.Append("   ,@Camera ");
				sb.Append("   ,@CameraCondition ");
				sb.Append("   ,@DeviceId ");
				sb.Append("   )");
				sb.Append(";");

				cmd.CommandText = sb.ToString();

				CommandText = cmd.CommandText;

				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
				cmd.Parameters.Add(new SqlParameter("@CameraIdx", SqlDbType.Int)).Value = CameraIdx;
				cmd.Parameters.Add(new SqlParameter("@Camera", SqlDbType.NVarChar)).Value = Camera;
				cmd.Parameters.Add(new SqlParameter("@CameraCondition", SqlDbType.Int)).Value = CameraCondition;
				cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;

				cmd.ExecuteNonQuery();
			}
		}

		#endregion

		#region UpdateDataCollectionDOMst
		private void UpdateDataCollectionDOMst(SqlDBConnector dbConnector, Guid DeviceId)
		{
			String CommandText = "";
			StringBuilder sb = new StringBuilder();

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				sb.Clear();
				sb.Append("MERGE INTO DataCollection_DO_Mst AS T1  ");
				sb.Append("  USING ");
				sb.Append("    (SELECT ");
				sb.Append("       @CompositionId AS CompositionId ");
				sb.Append("      ,@ProcessIdx AS ProcessIdx");
				sb.Append("    ) AS T2");
				sb.Append("  ON (");
				sb.Append("       T1.CompositionId = @CompositionId ");
				sb.Append("   AND T1.ProcessIdx = @ProcessIdx ");
				sb.Append("  )");
				sb.Append(" WHEN MATCHED THEN ");
				sb.Append("  UPDATE SET ");
				sb.Append("   DeviceId = @DeviceId");
				sb.Append(" WHEN NOT MATCHED THEN ");
				sb.Append("  INSERT (");
				sb.Append("   CompositionId");
				sb.Append("   ,ProcessIdx");
				sb.Append("   ,DeviceId");
				sb.Append("   ) VALUES (");
				sb.Append("   @CompositionId ");
				sb.Append("   ,@ProcessIdx ");
				sb.Append("   ,@DeviceId ");
				sb.Append("   )");
				sb.Append(";");

				cmd.CommandText = sb.ToString();

				CommandText = cmd.CommandText;

				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@ProcessIdx", SqlDbType.Int)).Value = ProcessIdx;
				cmd.Parameters.Add(new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)).Value = DeviceId;

				cmd.ExecuteNonQuery();
			}
		}

		#endregion

		#region Regist

		public void Regist()
		{
			//try
			//{
			//	SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
			//	if (dbConnector != null)
			//	{
			//		dbConnector.Create();
			//		dbConnector.OpenDatabase();

			//		UpdateDataCollectionTriggerMst(dbConnector);

			//		RemoveDataCollectionTriggerSensorMst(dbConnector);

			//		if ((int)cmbTriggerType.SelectedValue != 0)
			//		{
			//			UpdateDataCollectionTriggerSensorMst(dbConnector, 0, (int)NmSensorFilter_1.Value, (Guid)cmbSensorDevice_1.SelectedValue);

			//			if (cmbTriggerType.SelectedIndex != 0 && cmbTriggerType.SelectedIndex != 3)
			//			{
			//				UpdateDataCollectionTriggerSensorMst(dbConnector, 1, (int)NmSensorFilter_2.Value, (Guid)cmbSensorDevice_2.SelectedValue);
			//			}
			//		}

			//		RemoveDataCollectionCameraMst(dbConnector);

			//		if ((int)cmbTriggerType.SelectedValue != 0)
			//		{
			//			int idx = 0;
			//			foreach (DataRow rowItem in _dtDeviceWithCameraUseList.Rows)
			//			{
			//				if (rowItem.Field<Int32>("CameraUse") == 1)
			//				{
			//					UpdateDataCollectionCameraMst(dbConnector,
			//						idx++, rowItem.Field<String>("DeviceRemark"), rowItem.Field<Int32>("CameraCondition"), rowItem.Field<Guid>("DeviceId"));
			//				}
			//			}
			//		}

			//		UpdateDataCollectionDOMst(dbConnector, (Guid)cmbDODevice.SelectedValue);

			//		dbConnector.CloseDatabase();
			//		dbConnector.Dispose();
			//	}
			//}
			//catch (System.Data.SqlClient.SqlException ex)
			//{
			//	throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
			//}
			//catch (Exception ex)
			//{
			//	throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
			//}
		}
		#endregion
	}
}
