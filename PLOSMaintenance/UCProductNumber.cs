//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Data.SqlClient;
//using DBConnect;

//namespace PLOSMaintenance
//{
//	public partial class UCProductNumber : UserControl
//	{
//		private BindingSource _BindingSource;
//		private DataTable _QueryData;
//		private List<Guid> _RemoveList = new List<Guid>();

//		public UCProductNumber()
//		{
//			InitializeComponent();

//			InitializeDataGridView();
//		}

//		private void InitializeDataGridView()
//		{
//			_RemoveList.Clear();
//			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
//			if (dbConnector != null)
//			{
//				dbConnector.Create();
//				dbConnector.OpenDatabase();
//				DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
//				textEditCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

//				_QueryData = QueryProductNumberMst(dbConnector);
//				_BindingSource = new BindingSource(_QueryData, String.Empty);
//				dataGridView.Columns.Clear();

//				dataGridView.AutoGenerateColumns = false;
//				dataGridView.DataSource = _BindingSource;

//				DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
//				imgColumn.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
//				imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
//				imgColumn.Name = "DeleteActionImage";
//				imgColumn.HeaderText = "";
//				dataGridView.Columns.Add(imgColumn);

//				DataGridViewTextBoxColumn txtProductTypeIdColumn = new DataGridViewTextBoxColumn();
//				txtProductTypeIdColumn.DataPropertyName = "ProductTypeId";
//				txtProductTypeIdColumn.Name = "ProductTypeId";
//				txtProductTypeIdColumn.Visible = false;
//				dataGridView.Columns.Add(txtProductTypeIdColumn);

//				DataGridViewTextBoxColumn txtProductTypeNameColumn = new DataGridViewTextBoxColumn();
//				txtProductTypeNameColumn.DataPropertyName = "ProductTypeName";
//				txtProductTypeNameColumn.Name = "ProductTypeName";
//				txtProductTypeNameColumn.HeaderText = "品番タイプ";
//				txtProductTypeNameColumn.FillWeight = 80;
//				txtProductTypeNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
//				txtProductTypeNameColumn.MaxInputLength = 49;
//				txtProductTypeNameColumn.DefaultCellStyle = textEditCellStyle;
//				dataGridView.Columns.Add(txtProductTypeNameColumn);

//				DataGridViewComboBoxColumn cmbbxProductTypeIdColumn = new DataGridViewComboBoxColumn();

//				//表示する列の名前を設定する
//				cmbbxProductTypeIdColumn.DataPropertyName = "ProductTypeId";
//				cmbbxProductTypeIdColumn.Name = "ProductTypeId";
//				cmbbxProductTypeIdColumn.HeaderText = "タイプ名";
//				cmbbxProductTypeIdColumn.Width = 350;
//				cmbbxProductTypeIdColumn.SortMode = DataGridViewColumnSortMode.Automatic;

//				//DataGridViewComboBoxColumnのDataSourceを設定
//				cmbbxProductTypeIdColumn.DataSource = QueryProductTypeMst(dbConnector);
//				//実際の値が"Value"列、表示するテキストが"Display"列とする
//				cmbbxProductTypeIdColumn.ValueMember = "ProductTypeId";
//				cmbbxProductTypeIdColumn.DisplayMember = "ProductTypeName";

//				dataGridView.Columns.Add(cmbbxProductTypeIdColumn);

//				dbConnector.CloseDatabase();
//				dbConnector.Dispose();
//			}
//		}

//		private Boolean InputDataValidate()
//		{
//			foreach (DataRow rowItem in _QueryData.Rows)
//			{
//				if (rowItem.RowState == DataRowState.Modified
//				  || rowItem.RowState == DataRowState.Added)
//				{
//					if (String.IsNullOrWhiteSpace(rowItem.ItemArray[1].ToString()))
//					{
//						MessageBox.Show(this, "品番に空白以外の文字列を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
//						return false;
//					}
//				}
//			}

//			return true;
//		}

//		#region QueryProductNumberMst
//		private DataTable QueryProductNumberMst(
//				SqlDBConnector dbConnector)
//		{
//			DataTable dt = new DataTable("ProductNumber_Mst");

//			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
//			{
//				StringBuilder sb = new StringBuilder();
//				sb.Append("SELECT T1.* ");
//				sb.Append(" FROM ProductNumber_Mst T1 ");
//				sb.Append(" WHERE T1.Deleted = 0 ");
//				sb.Append("  Order by ProductNumber ");

//				cmd.CommandText = sb.ToString();

//				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
//				{
//					adapter.Fill(dt);
//				}
//			}

//			return dt;
//		}
//		#endregion

//		#region QueryProductTypeMst
//		private DataTable QueryProductTypeMst(
//				SqlDBConnector dbConnector)
//		{
//			DataTable dt = new DataTable("ProductType_Mst");

//			using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
//			{
//				StringBuilder sb = new StringBuilder();
//				sb.Append("SELECT T1.* ");
//				sb.Append(" FROM ProductType_Mst T1 ");
//				sb.Append(" WHERE T1.Deleted = 0 ");
//				sb.Append("  Order by OrderIdx ");

//				cmd.CommandText = sb.ToString();

//				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
//				{
//					adapter.Fill(dt);
//				}
//			}

//			return dt;
//		}
//		#endregion

//		#region UpdateProductTypeMst
//		private void UpdateProductTypeMst(
//				DataTable dt)
//		{
//			try
//			{
//				SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
//				if (dbConnector != null)
//				{
//					dbConnector.Create();
//					dbConnector.OpenDatabase();
//					String CommandText = "";
//					StringBuilder sb = new StringBuilder();

//					using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
//					{
//						foreach (DataRow rowItem in dt.Rows)
//						{
//							if (rowItem.RowState == DataRowState.Modified
//							  || rowItem.RowState == DataRowState.Added)
//							{
//								sb.Clear();
//								sb.Append("MERGE INTO ProductType_Mst AS T1  ");
//								sb.Append("  USING ");
//								sb.Append("    (SELECT ");
//								sb.Append("      @ProductTypeId AS ProductTypeId ");
//								sb.Append("    ) AS T2");
//								sb.Append("  ON (");
//								sb.Append("   T1.ProductTypeId = T2.ProductTypeId ");
//								sb.Append("  )");
//								sb.Append(" WHEN MATCHED THEN ");
//								sb.Append("  UPDATE SET ");
//								sb.Append("   ProductTypeId = @ProductTypeId");
//								//sb.Append("   ,ProductTypeId = @ProductTypeId");
//								sb.Append("   ,Deleted = @Deleted");
//								sb.Append(" WHEN NOT MATCHED THEN ");
//								sb.Append("  INSERT (");
//								sb.Append("   ProductTypeId");
//								//sb.Append("   ,ProductTypeName");
//								//sb.Append("   ,ProductTypeId");
//								sb.Append("   ,Deleted");
//								sb.Append("   ) VALUES (");
//								sb.Append("   @ProductTypeId ");
//								//sb.Append("   ,@ProductNumber ");
//								//sb.Append("   ,@ProductTypeId ");
//								sb.Append("   ,@Deleted ");
//								sb.Append("   )");
//								sb.Append(";");

//								cmd.CommandText = sb.ToString();

//								CommandText = cmd.CommandText;

//								cmd.Parameters.Clear();
//								//cmd.Parameters.Add(new SqlParameter("@ProductNumberId", SqlDbType.UniqueIdentifier)).Value = new Guid(rowItem.ItemArray[0].ToString());
//								//cmd.Parameters.Add(new SqlParameter("@ProductNumber", SqlDbType.NVarChar)).Value = rowItem.ItemArray[1];
//								cmd.Parameters.Add(new SqlParameter("@ProductTypeId", SqlDbType.UniqueIdentifier)).Value = rowItem.ItemArray[2];
//								cmd.Parameters.Add(new SqlParameter("@Deleted", SqlDbType.Bit)).Value = rowItem.ItemArray[3];

//								cmd.ExecuteNonQuery();
//							}
//						}

//						foreach (Guid target in _RemoveList)
//						{
//							sb.Clear();
//							sb.Append("DELETE FROM ProductType_Mst");
//							sb.Append(" WHERE ProductTypeId = @ProductTypeId ");

//							cmd.CommandText = sb.ToString();

//							CommandText = cmd.CommandText;

//							cmd.Parameters.Clear();
//							cmd.Parameters.Add(new SqlParameter("@ProductTypeId", SqlDbType.UniqueIdentifier)).Value = target;

//							cmd.ExecuteNonQuery();
//						}
//					}
//					dbConnector.CloseDatabase();
//					dbConnector.Dispose();
//				}
//			}
//			catch (System.Data.SqlClient.SqlException ex)
//			{
//				throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
//			}
//			catch (Exception ex)
//			{
//				throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
//			}
//		}

//		#endregion

//		private void OnAddButtonClicked(object sender, EventArgs e)
//		{
//			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
//			Guid ProductTypeIdTop1 = Guid.Empty;
//			if (dbConnector != null)
//			{
//				dbConnector.Create();
//				dbConnector.OpenDatabase();
//				DataTable dt = QueryProductTypeMst(dbConnector);
//				ProductTypeIdTop1 = (Guid)((DataRow)dt.Rows[0]).ItemArray[0];
//				dbConnector.CloseDatabase();
//				dbConnector.Dispose();
//			}

//			DataRow dr = _QueryData.NewRow();
//			dr[0] = Guid.NewGuid();
//			dr[1] = "";
//			dr[2] = ProductTypeIdTop1;
//			dr[3] = 0;
//			_QueryData.Rows.Add(dr);
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void OnGridCellContentClicked(object sender, DataGridViewCellEventArgs e)
//		{
//			DataGridView dgv = (DataGridView)sender;
//			//"Button"列ならば、ボタンがクリックされた
//			if (dgv.Columns[e.ColumnIndex].Name == "DeleteActionImage")
//			{
//				if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show(this, $"{dgv.Rows[e.RowIndex].Cells[2].Value}を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
//				{
//					_RemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[1].Value);
//					dgv.Rows.RemoveAt(e.RowIndex);
//				}
//			}
//		}

//		private void OnRegistButtonClicked(object sender, EventArgs e)
//		{
//			dataGridView.EndEdit();

//			if (InputDataValidate())
//			{
//				UpdateProductTypeMst(_QueryData);
//				_QueryData.AcceptChanges();
//				InitializeDataGridView();
//			}
//		}


//		public void OnProductTypeChanged(object sender, EventArgs e)
//		{
//			SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString);
//			if (dbConnector != null)
//			{
//				dbConnector.Create();
//				dbConnector.OpenDatabase();
//				((DataGridViewComboBoxColumn)dataGridView.Columns["ProductTypeId"]).DataSource = QueryProductTypeMst(dbConnector);
//				dbConnector.CloseDatabase();
//				dbConnector.Dispose();
//			}
//		}
//	}
//}
