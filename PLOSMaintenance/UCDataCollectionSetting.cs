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
	public partial class UCDataCollectionSetting : UserControl
	{
		private BindingSource _BindingSource;
		private DataTable _dtComposition;
		private List<Guid> _RemoveList = new List<Guid>();
		private List<UCDataCollectionItem> _UCDataCollectionList = new List<UCDataCollectionItem>();

		private Guid CompositionId { get; set; } = Guid.Empty;

		public UCDataCollectionSetting()
		{
			InitializeComponent();

			InitializeComponentData();
		}

		public UCDataCollectionSetting(Guid compositionId)
		{
			this.CompositionId = compositionId;

			InitializeComponent();

			InitializeComponentData();
		}

		private void InitializeComponentData()
		{
			_RemoveList.Clear();
			_UCDataCollectionList.Clear();

			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				DataTable dtDataCollection = QueryDataCollectionTriggerMst(dbConnector);

				tlPnl.RowCount = dtDataCollection.Rows.Count;

				for (int iCnt = 0; iCnt < tlPnl.RowCount; iCnt++)
				{
					tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle());

					UCDataCollectionItem uc = new UCDataCollectionItem(dtDataCollection.Rows[iCnt]);
					_UCDataCollectionList.Add(uc);
					tlPnl.Controls.Add(uc, iCnt, 0);
				}

				DataTable dtQuantityCounter = QueryQuantityCounterTriggerMst(dbConnector);
				if(dtQuantityCounter.Rows.Count > 0)
				{
					UCDataCollectionItem uc = new UCDataCollectionItem(dtQuantityCounter.Rows[0]);
					_UCDataCollectionList.Add(uc);
					tlPnl.Controls.Add(uc, tlPnl.RowCount, 0);
				}
				else
				{
					var dr = dtQuantityCounter.NewRow();
					dr[0] = CompositionId;
					dr[1] = 101;
					dr[2] = "良品カウンタ";
					dr[3] = 1;
					dr[4] = 1;
					UCDataCollectionItem uc = new UCDataCollectionItem(dr);
					_UCDataCollectionList.Add(uc);
					tlPnl.Controls.Add(uc, tlPnl.RowCount, 0);
				}

				_dtComposition = QueryCompositionMst(dbConnector);
				lblUniqueName.Text = String.Format(lblUniqueName.Text, _dtComposition.Rows[0].Field<Int32>("ProcessCount"), _dtComposition.Rows[0].Field<String>("UniqueName"));

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}
		}

		#region QueryCompositionMst
		private DataTable QueryCompositionMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("Composition_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append("  T1.*");
				sb.Append(" ,(SELECT COUNT(CompositionId) FROM [Composition_Process_Mst] WHERE CompositionId = @CompositionId) ProcessCount");
				sb.Append(" FROM Composition_Mst T1 ");
				sb.Append(" Where T1.CompositionId = @CompositionId ");

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

		#region QueryDataCollectionTriggerMst
		private DataTable QueryDataCollectionTriggerMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("DataCollection_Trigger_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append("  T1.CompositionId");
				sb.Append("  ,T1.ProcessIdx");
				sb.Append("  ,T1.ProcessName");
				sb.Append("  ,CASE WHEN T2.[TriggerType] IS NULL THEN 1 ELSE T2.[TriggerType] END [TriggerType] ");
				sb.Append("  ,0 [IsProductionQuantityCounter] ");
				sb.Append(" FROM Composition_Process_Mst T1 ");
				sb.Append("  LEFT JOIN DataCollection_Trigger_Mst T2");
				sb.Append("  ON   T2.CompositionId = T1.CompositionId");
				sb.Append("   AND T2.ProcessIdx = T1.ProcessIdx");
				sb.Append(" Where T1.CompositionId = @CompositionId ");
				sb.Append("  Order by T1.ProcessIdx ");
				//IsProductionQuantityCounter
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

		#region QueryQuantityCounterTriggerMst
		private DataTable QueryQuantityCounterTriggerMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("DataCollection_Trigger_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT  ");
				sb.Append("  T1.CompositionId");
				sb.Append("  ,CASE WHEN T1.[ProcessIdx] IS NULL THEN 101 ELSE T1.[ProcessIdx] END [ProcessIdx] ");
				sb.Append("  ,'良品カウンタ' ProcessName");
				sb.Append("  ,CASE WHEN T1.[TriggerType] IS NULL THEN 1 ELSE T1.[TriggerType] END [TriggerType] ");
				sb.Append("  ,1 [IsProductionQuantityCounter] ");
				sb.Append(" FROM DataCollection_Trigger_Mst T1 ");
				sb.Append(" Where T1.CompositionId = @CompositionId ");
				sb.Append("  AND T1.IsProductionQuantityCounter = 1 ");

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

		#region UpdateCompositionMst
		private void UpdateCompositionMst(SqlDBConnector dbConnector
				, String UniqueName)
		{
			String CommandText = "";
			StringBuilder sb = new StringBuilder();

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				sb.Clear();
				sb.Append("MERGE INTO Composition_Mst AS T1  ");
				sb.Append("  USING ");
				sb.Append("    (SELECT ");
				sb.Append("      @CompositionId AS CompositionId ");
				sb.Append("    ) AS T2");
				sb.Append("  ON (");
				sb.Append("   T1.CompositionId = @CompositionId ");
				sb.Append("  )");
				sb.Append(" WHEN MATCHED THEN ");
				sb.Append("  UPDATE SET ");
				sb.Append("   UniqueName = @UniqueName");
				sb.Append(" WHEN NOT MATCHED THEN ");
				sb.Append("  INSERT (");
				sb.Append("   CompositionId");
				sb.Append("   ,UniqueName");
				sb.Append("   ) VALUES (");
				sb.Append("   @CompositionId ");
				sb.Append("   ,@UniqueName ");
				sb.Append("   )");
				sb.Append(";");

				cmd.CommandText = sb.ToString();

				CommandText = cmd.CommandText;

				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SqlParameter("@CompositionId", SqlDbType.UniqueIdentifier)).Value = CompositionId;
				cmd.Parameters.Add(new SqlParameter("@UniqueName", SqlDbType.NVarChar)).Value = UniqueName;

				cmd.ExecuteNonQuery();
			}
		}

		#endregion

		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();

				UpdateCompositionMst(dbConnector
						, _dtComposition.Rows[0].Field<String>("UniqueName"));

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}

			foreach (var ucCollectionItem in _UCDataCollectionList)
			{
				ucCollectionItem.Regist();
			}

			ParentForm.Close();
		}
	}
}
