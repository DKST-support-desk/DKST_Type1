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
	/// <summary>
	/// UCWorker
	/// </summary>
	public partial class UCWorker : UserControl
	{
		private BindingSource _BindingSource;
		private DataTable _QueryData;
		private List<Guid> _RemoveList = new List<Guid>();

		#region EventHandler(Delegate)

		/// <summary>
		/// For Notify Data Change
		/// </summary>
		public event EventHandler WorkerChanged;

		#endregion

		public UCWorker()
		{
			InitializeComponent();

			InitializeDataGridView();
		}

		private void InitializeDataGridView()
		{
			_RemoveList.Clear();
			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
			if (dbConnector != null)
			{
				dbConnector.Create();
				dbConnector.OpenDatabase();
				DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
				textEditCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

				DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
				imgColumn.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
				imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
				imgColumn.Name = "DeleteActionImage";
				imgColumn.HeaderText = "";

				_QueryData = QueryWorkerMst(dbConnector);
				_BindingSource = new BindingSource(_QueryData, String.Empty);
				dataGridView.Columns.Clear();
				dataGridView.DataSource = _BindingSource;
				dataGridView.Columns["WorkerId"].Visible = false;
				dataGridView.Columns["Deleted"].Visible = false;
				dataGridView.Columns["WorkerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dataGridView.Columns["WorkerName"].HeaderText = "作業者名";
				dataGridView.Columns["WorkerName"].FillWeight = 90;
				((System.Windows.Forms.DataGridViewTextBoxColumn)dataGridView.Columns["WorkerName"]).MaxInputLength = 127;
				dataGridView.Columns["WorkerName"].DefaultCellStyle = textEditCellStyle;
				dataGridView.Columns["Remarks"].Width = 150;
				dataGridView.Columns.Insert(0, imgColumn);

				dbConnector.CloseDatabase();
				dbConnector.Dispose();
			}
		}

		private Boolean InputDataValidate()
		{
			foreach (DataRow rowItem in _QueryData.Rows)
			{
				if (rowItem.RowState == DataRowState.Modified
				  || rowItem.RowState == DataRowState.Added)
				{
					if (String.IsNullOrWhiteSpace(rowItem.ItemArray[1].ToString()))
					{
						MessageBox.Show(this, "作業者名に空白以外の文字列を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
			}

			return true;
		}

		#region QueryWorkerMst
		private DataTable QueryWorkerMst(
				SqlDBConnector dbConnector)
		{
			DataTable dt = new DataTable("Worker_Mst");

			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT T1.* ");
				sb.Append(" FROM Worker_Mst T1 ");
				sb.Append(" WHERE T1.Deleted = 0 ");
				sb.Append("  Order by WorkerId ");

				cmd.CommandText = sb.ToString();

				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}

			return dt;
		}
		#endregion

		#region UpdateWorkerMst
		private void UpdateWorkerMst(
				DataTable dt)
		{
			try
			{
				SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
				if (dbConnector != null)
				{
					dbConnector.Create();
					dbConnector.OpenDatabase();
					String CommandText = "";
					StringBuilder sb = new StringBuilder();

					using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
					{
						foreach (DataRow rowItem in dt.Rows)
						{
							if (rowItem.RowState == DataRowState.Modified
							  || rowItem.RowState == DataRowState.Added)
							{
								sb.Clear();
								sb.Append("MERGE INTO Worker_Mst AS T1  ");
								sb.Append("  USING ");
								sb.Append("    (SELECT ");
								sb.Append("      @WorkerId AS WorkerId ");
								sb.Append("    ) AS T2");
								sb.Append("  ON (");
								sb.Append("   T1.WorkerId = T2.WorkerId ");
								sb.Append("  )");
								sb.Append(" WHEN MATCHED THEN ");
								sb.Append("  UPDATE SET ");
								sb.Append("   WorkerName = @WorkerName");
								sb.Append("   ,Remarks = @Remarks");
								sb.Append("   ,Deleted = @Deleted");
								sb.Append(" WHEN NOT MATCHED THEN ");
								sb.Append("  INSERT (");
								sb.Append("   WorkerId");
								sb.Append("   ,WorkerName");
								sb.Append("   ,Remarks");
								sb.Append("   ,Deleted");
								sb.Append("   ) VALUES (");
								sb.Append("   @WorkerId ");
								sb.Append("   ,@WorkerName ");
								sb.Append("   ,@Remarks ");
								sb.Append("   ,@Deleted ");
								sb.Append("   )");
								sb.Append(";");

								cmd.CommandText = sb.ToString();

								CommandText = cmd.CommandText;

								cmd.Parameters.Clear();
								cmd.Parameters.Add(new SqlParameter("@WorkerId", SqlDbType.UniqueIdentifier)).Value = new Guid(rowItem.ItemArray[0].ToString());
								cmd.Parameters.Add(new SqlParameter("@WorkerName", SqlDbType.NVarChar)).Value = rowItem.ItemArray[1];
								cmd.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar)).Value = rowItem.ItemArray[2];
								cmd.Parameters.Add(new SqlParameter("@Deleted", SqlDbType.Bit)).Value = rowItem.ItemArray[3];

								cmd.ExecuteNonQuery();
							}
						}

						foreach (Guid target in _RemoveList)
						{
							sb.Clear();
							sb.Append("DELETE FROM Worker_Mst");
							sb.Append(" WHERE WorkerId = @WorkerId ");

							cmd.CommandText = sb.ToString();

							CommandText = cmd.CommandText;

							cmd.Parameters.Clear();
							cmd.Parameters.Add(new SqlParameter("@WorkerId", SqlDbType.UniqueIdentifier)).Value = target;

							cmd.ExecuteNonQuery();
						}
					}
					dbConnector.CloseDatabase();
					dbConnector.Dispose();
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
			}
		}

		#endregion

		private void OnAddButtonClicked(object sender, EventArgs e)
		{
			DataRow dr = _QueryData.NewRow();
			dr[0] = Guid.NewGuid();
			dr[1] = "";
			dr[2] = "";
			dr[3] = 0;
			_QueryData.Rows.Add(dr);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridCellContentClicked(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView dgv = (DataGridView)sender;
			//"Button"列ならば、ボタンがクリックされた
			if (dgv.Columns[e.ColumnIndex].Name == "DeleteActionImage")
			{
				if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show(this, $"{dgv.Rows[e.RowIndex].Cells[2].Value}を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					_RemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[1].Value);
					dgv.Rows.RemoveAt(e.RowIndex);
				}
			}
		}

		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			dataGridView.EndEdit();

			if (InputDataValidate())
			{
				UpdateWorkerMst(_QueryData);
				NotifyStatusChange(new EventArgs());
				_QueryData.AcceptChanges();
				InitializeDataGridView();
			}
		}

		#region Public Methods

		/// <summary>
		/// Notify Status Changed
		/// </summary>
		public void NotifyStatusChange(EventArgs args)
		{
			EventHandler handler = WorkerChanged;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, args);
				}
			}
		}

		#endregion
	}
}
