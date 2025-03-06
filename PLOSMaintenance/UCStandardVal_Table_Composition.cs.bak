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
using DBConnect.SQL;
using System.Windows.Forms.DataVisualization.Charting;
using log4net;

namespace PLOSMaintenance
{
	public partial class UCStandardVal_Table_Composition : UserControl
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバ変数"
		private class ChartSeriesStdValType
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="chartSeries"></param>
			/// <param name="stdValType"></param>
			/// <param name="stdValType_Subtraction"></param>
			public ChartSeriesStdValType(String chartSeries, int stdValType, int stdValType_Subtraction = 0)
			{
				ChartSeries = chartSeries;
				StdValType = stdValType;
				StdValType_Subtraction = stdValType_Subtraction;
			}

			/// <summary>
			/// 
			/// </summary>
			public String ChartSeries { get; set; }

			/// <summary>
			/// タイプ
			/// </summary>
			public int StdValType { get; set; }

			/// <summary>
			/// 
			/// </summary>
			public int StdValType_Subtraction { get; set; }
		}

		/// <summary>
		/// 編成プロセスマスタテーブルアクセス
		/// </summary>
		private SqlCompositionProcessMst mSqlCompositionProcessMst;

		/// <summary>
		/// 編成プロセスマスタテーブルデータ
		/// </summary>
		private DataTable mDtCompositionProcessMst;

		/// <summary>
		/// 標準値品番別マスタテーブルアクセス
		/// </summary>
		private SqlStandardValProductTypeMst mSqlStandardValProductTypeMst;

		/// <summary>
		/// 標準値品番別マスタテーブルデータ
		/// </summary>
		private DataTable mDtStandardValProductTypeMst;

		/// <summary>
		/// 標準値品番工程別マスタテーブルアクセス
		/// </summary>
		private SqlStandardValProductTypeProcessMst mSqlStandardValProductTypeProcessMst;

		/// <summary>
		/// 
		/// </summary>
		private DataTable mDtStandardVal;

		/// <summary>
		/// パネル
		/// </summary>
		private TableLayoutPanel tLPnlStdVal;

		/// <summary>
		/// CT標準値/機器の設定コントロールリスト
		/// </summary>
		private List<UCStandardVal_Table_Item> mStdValTblList;

		/// <summary>
		/// 
		/// </summary>
		private List<ChartSeriesStdValType> ChartSeriesStdValTypeList;

		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// 登録クリックイベント デリゲート
		/// </summary>
		[Browsable(false)]
		public event EventHandler RegistClicked;

		/// <summary>
		/// グラフの表示変更イベント デリゲート
		/// </summary>
		public event EventHandler HideGraphCheckedChanged;

		/// <summary>
		/// 戻るボタン押下イベント　デリゲート
		/// </summary>
		public event EventHandler BackClicked;

		/// <summary>
		/// 入力値変更フラグ
		/// </summary>
		private bool changeValueFlg;
		#endregion "メンバ変数"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region "コンストラクタ"
		/// <summary>
		/// 標準値マスタ
		/// </summary>
		public UCStandardVal_Table_Composition()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 標準値マスタ
		/// </summary>
		/// <param name="compositionId"></param>
		/// <param name="productTypeId"></param>
		/// <param name="numberPeople"></param>
		/// <param name="uniqueName"></param>
		public UCStandardVal_Table_Composition(Guid compositionId, Guid productTypeId, string numberPeople, string uniqueName)
		{
			InitializeComponent();

			CompositionId = compositionId;
			ProductTypeId = productTypeId;

			tLPnlStdVal = new TableLayoutPanel();
			mStdValTblList = new List<UCStandardVal_Table_Item>();

			ChartSeriesStdValTypeList = new List<ChartSeriesStdValType>()
			{
				new ChartSeriesStdValType("Series_CTLow_L", 3), new ChartSeriesStdValType("Series_CTLow_R", 3),
				new ChartSeriesStdValType("Series_CTHi", 1, 3),new ChartSeriesStdValType("Series_CTAve", 2),
				new ChartSeriesStdValType("Series_Ancillary", 11), new ChartSeriesStdValType("Series_Setup", 12),
			};

			mSqlCompositionProcessMst = new SqlCompositionProcessMst(Properties.Settings.Default.ConnectionString_New);
			mSqlStandardValProductTypeProcessMst = new SqlStandardValProductTypeProcessMst(Properties.Settings.Default.ConnectionString_New);
			mSqlStandardValProductTypeMst = new SqlStandardValProductTypeMst(Properties.Settings.Default.ConnectionString_New);

			IsEnableShowStandardVal = InitializeComponentData(numberPeople, uniqueName);
		}
		#endregion "コンストラクタ"

		//********************************************
		//* プロパティ
		//********************************************
		#region "プロパティ"
		/// <summary>
		/// 編成ID
		/// </summary>
		[Category("Custom")]
		[Description("CompositionId")]
		[DefaultValue("")]
		public Guid CompositionId { get; set; }

		/// <summary>
		/// 品番タイプID
		/// </summary>
		[Category("Custom")]
		[Description("ProductTypeId")]
		[DefaultValue("")]
		public Guid ProductTypeId { get; set; }


		/// <summary>
		/// グラフ表示/非表示
		/// </summary>
		public Boolean GraphVisible
		{
			set
			{
				pnlGraph.Visible = value;
				foreach (var item in mStdValTblList)
					item.GraphVisibleCheckBoxStatus = value ? CheckState.Checked : CheckState.Unchecked;
				pnlMain.AutoScrollPosition = new Point(0, 0);
			}
		}

		public Boolean IsEnableShowStandardVal
        {
			get; set;
        }

		/// <summary>
		/// 入力値変更フラグ
		/// </summary>
		public bool ChangeValueFlg
        {
            get { return changeValueFlg; }
            set { changeValueFlg = value; }
        }
		#endregion "プロパティ"

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// CT標準値の設定変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UcStdValTbl_StandardValChanged(object sender, EventArgs e)
		{
			if (sender is UCStandardVal_Table_Item item)
			{
				ShowChart(mDtCompositionProcessMst, item.ProductTypeId, item.GetAllStdVal());
			}
		}

		/// <summary>
		/// 登録ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			EventHandler handler = RegistClicked;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, e);
				}
			}
		}

		/// <summary>
		/// 戻るボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClose(object sender, EventArgs e)
		{
			EventHandler handler = BackClicked;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, e);
				}
			}
		}

        /// <summary>
        /// テキスト変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, EventArgs e)
		{
			try
			{
				int ProcessIdx;
				int StdValType;
				decimal StdVal;
				decimal nmStdVal99_101, nmStdVal99_102;

				if (!changeValueFlg)
				{
					changeValueFlg = true;
				}

				// 入力値の確認
				if (nmtxtStdVal99_101.Text == "" || nmtxtStdVal99_102.Text == "")
				{
					nmtxtStdVal99_103.Text = "";
				}

				if (sender is PLOS.Gui.Core.CustumContol.NumericTextBox numText)
				{
					if (numText.Tag is String tagStr)
					{
						String[] tagStrDim = tagStr.Split(',');
						if (tagStrDim.Length == 2)
						{
							if (int.TryParse(tagStrDim[0], out ProcessIdx)
								&& int.TryParse(tagStrDim[1], out StdValType)
								&& decimal.TryParse(numText.Text, out StdVal))
							{
								switch (StdValType)
								{
									case 101:
									case 102:
										if (decimal.TryParse(nmtxtStdVal99_101.Text, out nmStdVal99_101)
											&& decimal.TryParse(nmtxtStdVal99_102.Text, out nmStdVal99_102))
											nmtxtStdVal99_103.Text = (nmStdVal99_101 + nmStdVal99_102).ToString();
										break;
									default:
										break;
								}
							}
						}
					}
				}
			}
            catch (Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// グラフ表示変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHideGraphCheckedChanged(object sender, EventArgs e)
		{
            try
            {
				EventHandler handler = HideGraphCheckedChanged;
				if (handler != null)
				{
					foreach (EventHandler evhd in handler.GetInvocationList())
					{
						evhd(sender, e);
					}
				}
			}
            catch (Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

        // 2022/06/14 西部追加
        /// <summary>
        /// サイズ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCStandardVal_Table_Composition_SizeChanged(object sender, EventArgs e)
        {
            //テーブルレイアウトパネル部分の最大横幅を変更する
            ((UCStandardVal_Table_Item)mStdValTblList[0]).ChangeTableAreaWidth(this.Width);
        }
		#endregion "イベント"

		//********************************************
		//* パブリックメソッド
		//********************************************
		#region "パブリックメソッド"
		/// <summary>
		/// 戻るボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public bool OnClose()
		{
			logger.Info("戻るボタン押下");

			logger.Info("入力値が変更されています。値をリセットして編成情報登録画面に戻りますか？");

			if (DialogResult.Yes != MessageBox.Show("入力値が変更されています。\n値をリセットして編成情報登録画面に戻りますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				logger.Info("No押下");
				return false;
			}
			logger.Info("Yes押下");
			ParentForm.Close();
			logger.Info("標準値マスタ画面を閉じました。");

			return true;
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public bool CheckInput(out string errMsg)
		{
			const int Min = 0;
			const int Max = 9999;

			errMsg = string.Empty;

			// HT
			Decimal ht = 0;
			if (Decimal.TryParse(nmtxtStdVal99_101.Text, out ht))
			{
				if (ht < Min || Max < ht)
				{
					errMsg = "HTの値が範囲外です。\nHTは0以上9999以下の数値のみ入力できます。";
					return false;
				}
			}
			else
			{
				errMsg = "HTの値が入力されていません。";
				return false;
			}

			// MT
			Decimal mt = 0;
			if (Decimal.TryParse(nmtxtStdVal99_102.Text, out mt))
			{
				if (mt < Min || Max < mt)
				{
					errMsg = "MTの値が範囲外です。\nMTは0以上9999以下の数値のみ入力できます。";
					return false;
				}
			}
			else
			{
				errMsg = "MTの値が入力されていません。";
				return false;
			}

			bool checkRet = true;
            bool goodProductFlag = false;
			foreach (var item in mStdValTblList)
			{
				// CT標準値/機器の設定入力チェック
				checkRet = item.CheckInput(out errMsg, out bool isGoodProduct);

                // 2022/06/11 西部修正
                //良品工程フラグが立っていたら良品工程存在フラグを立てる
                if(isGoodProduct)
                {
                    goodProductFlag = true;
				}

                if (!checkRet)
                {
					break;
                }
			}

			if(!checkRet)
            {
				return false;
            }

            //良品工程存在フラグが立っていないなら良品工程が選択されていない
            if (goodProductFlag == false)
            {
                errMsg = "良品工程が選択されていません。";
                return false;
            }

            return true;
		}

		/// <summary>
		/// 登録処理
		/// </summary>
		public bool Regist()
		{
			try
            {
				int idx = mDtStandardValProductTypeMst.Rows.IndexOf(mDtStandardValProductTypeMst.AsEnumerable()
										.FirstOrDefault(x => x.Field<Guid>(ColumnStandardValProductTypeMst.COMPOSITION_ID) == CompositionId
														  && x.Field<Guid>(ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID) == ProductTypeId));

				if (idx == -1)
				{
					mDtStandardValProductTypeMst.Rows.Add(mDtStandardValProductTypeMst.NewRow());
					idx = mDtStandardValProductTypeMst.Rows.Count - 1;
				}

				mDtStandardValProductTypeMst.Rows[idx][ColumnStandardValProductTypeMst.COMPOSITION_ID] = CompositionId;
				mDtStandardValProductTypeMst.Rows[idx][ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID] = ProductTypeId;

				Decimal ht = 0;
				Decimal.TryParse(nmtxtStdVal99_101.Text, out ht);
				mDtStandardValProductTypeMst.Rows[idx][ColumnStandardValProductTypeMst.HUMAN_TIME] = ht;

				Decimal mt = 0;
				Decimal.TryParse(nmtxtStdVal99_102.Text, out mt);
				mDtStandardValProductTypeMst.Rows[idx][ColumnStandardValProductTypeMst.MACHINE_TIME] = mt;

				Decimal mct = 0;
				Decimal.TryParse(nmtxtStdVal99_103.Text, out mct);
				mDtStandardValProductTypeMst.Rows[idx][ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME] = mct;


				using (SqlDBConnector db = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New))
                {
					db.Create();
					db.OpenDatabase();

					SqlConnection conn = db.DbConnection;
					using (SqlTransaction transaction = conn.BeginTransaction())
					{
						bool registRet1 = true;
						registRet1 = mSqlStandardValProductTypeMst.Upsert(mDtStandardValProductTypeMst, transaction);

						if(!registRet1)
						{
							transaction.Rollback();
							db.CloseDatabase();
							return false;
						}

						bool registRet2 = true;
						foreach (var item in mStdValTblList)
						{
							// CT標準値/機器の設定登録
							registRet2 = item.Regist(transaction);

							if(!registRet2)
							{
								break;
							}
						}

						if(!registRet2)
						{
							transaction.Rollback();
							db.CloseDatabase();
							return false;
						}
						transaction.Commit();
					}
					db.CloseDatabase();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				return false;
			}

			return true;
		}

		#endregion "パブリックメソッド"

		//********************************************
		//* プライベートメソッド
		//********************************************
		#region "プライベートメソッド"
		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="numberPeople"></param>
		/// <param name="uniqueName"></param>
		private Boolean InitializeComponentData(string numberPeople, string uniqueName)
		{
			this.chartStdVal = new Chart();
			((ISupportInitialize)(this.chartStdVal)).BeginInit();
			this.pnlGraph = new Panel();
			this.label1 = new Label();

			ChartArea chartArea1 = new ChartArea();
			Series series1 = new Series();
			Series series2 = new Series();
			Series series3 = new Series();
			Series series4 = new Series();
			Series series5 = new Series();
			Series series6 = new Series();

			// chartStdVal
			chartArea1.AxisX.Crossing = 0D;
			chartArea1.AxisX.MajorGrid.Enabled = false;
			chartArea1.AxisX.Minimum = 0D;
			chartArea1.AxisY.MajorGrid.Enabled = false;
			chartArea1.Name = "ChartArea1";

			this.chartStdVal.ChartAreas.Add(chartArea1);
			this.chartStdVal.Dock = DockStyle.Fill;
			this.chartStdVal.Location = new Point(200, 4);
			this.chartStdVal.Name = "chartStdVal";

			series1.BorderColor = Color.Gray;
			series1.ChartArea = "ChartArea1";
			series1.ChartType = SeriesChartType.StackedColumn;
			series1.Color = Color.Gray;
			series1.CustomProperties = "StackedGroupName=LeftSide";
			series1.IsVisibleInLegend = false;
			series1.Name = "Series_CTLow_L";

			series2.BorderColor = Color.Gray;
			series2.ChartArea = "ChartArea1";
			series2.ChartType = SeriesChartType.StackedColumn;
			series2.Color = Color.Gray;
			series2.CustomProperties = "StackedGroupName=RightSide";
			series2.IsVisibleInLegend = false;
			series2.Name = "Series_CTLow_R";

			series3.BorderColor = Color.Black;
			series3.BorderDashStyle = ChartDashStyle.Dash;
			series3.ChartArea = "ChartArea1";
			series3.ChartType = SeriesChartType.StackedColumn;
			series3.Color = Color.Transparent;
			series3.CustomProperties = "StackedGroupName=RightSide";
			series3.IsVisibleInLegend = false;
			series3.Name = "Series_CTHi";

			series4.BackHatchStyle = ChartHatchStyle.DarkUpwardDiagonal;
			series4.BorderColor = Color.Black;
			series4.ChartArea = "ChartArea1";
			series4.ChartType = SeriesChartType.StackedColumn;
			series4.Color = Color.Gray;
			series4.CustomProperties = "StackedGroupName=LeftSide";
			series4.IsVisibleInLegend = false;
			series4.Name = "Series_Ancillary";

			series5.BackHatchStyle = ChartHatchStyle.SmallGrid;
			series5.BorderColor = Color.Black;
			series5.ChartArea = "ChartArea1";
			series5.ChartType = SeriesChartType.StackedColumn;
			series5.Color = Color.Gray;
			series5.CustomProperties = "StackedGroupName=LeftSide";
			series5.IsVisibleInLegend = false;
			series5.Name = "Series_Setup";

			series6.ChartArea = "ChartArea1";
			series6.ChartType = SeriesChartType.Point;
			series6.IsVisibleInLegend = false;
			series6.MarkerColor = Color.Black;
			series6.MarkerSize = 4;
			series6.MarkerStyle = MarkerStyle.Circle;
			series6.Name = "Series_CTAve";

			this.chartStdVal.Series.Add(series1);
			this.chartStdVal.Series.Add(series2);
			this.chartStdVal.Series.Add(series3);
			this.chartStdVal.Series.Add(series4);
			this.chartStdVal.Series.Add(series5);
			this.chartStdVal.Series.Add(series6);
			this.chartStdVal.Size = new Size(672, 255);
			this.chartStdVal.TabIndex = 0;

			// pnlGraph
			this.pnlGraph.BackColor = SystemColors.Control;
			this.pnlGraph.Controls.Add(this.label1);
			this.pnlGraph.Location = new Point(0, 0);
			this.pnlGraph.Name = "pnlGraph";
			this.pnlGraph.Padding = new Padding(200, 4, 4, 4);
			this.pnlGraph.Size = new Size(876, 250);
			this.pnlGraph.TabIndex = 0;
			pnlGraph.Dock = DockStyle.Top;

			// label1
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.White;
			this.label1.Location = new Point(199, 4);
			this.label1.Name = "label1";
			this.label1.Size = new Size(57, 15);
			this.label1.TabIndex = 7;
			this.label1.Text = "CT【秒】";

			// 編成人数
			this.lblNumberOfPeople.Text = numberPeople;

			// 編成名
			this.lblUniqueName.Text = uniqueName;

			if (CompositionId == Guid.Empty) return false;

			mDtCompositionProcessMst = mSqlCompositionProcessMst.SelectProCompositionRow(CompositionId);
			mDtStandardVal = mSqlStandardValProductTypeProcessMst.Select(CompositionId, ProductTypeId);
			mDtStandardValProductTypeMst = mSqlStandardValProductTypeMst.Select(CompositionId, ProductTypeId);

			if(mDtCompositionProcessMst == null || mDtCompositionProcessMst.Rows.Count < 1)
            {
				logger.Error(String.Format("登録前の編成情報で標準値マスタ画面起動"));
				MessageBox.Show("登録前の編成情報です。\n標準値マスタ登録画面は表示できません。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
            }

			mStdValTblList.Clear();
			int iCnt = 0;

			pnlGraph.SuspendLayout();
			tLPanelHead.SuspendLayout();
			tLPnlStdVal.SuspendLayout();
			SuspendLayout();

			// tLPnlStdVal追加
			pnlMain.Controls.Add(tLPnlStdVal);
			// pnlGraph追加
			pnlMain.Controls.Add(pnlGraph);

			tLPnlStdVal.AutoSize = true;
			tLPnlStdVal.Location = new Point(0, 260);
			tLPnlStdVal.Name = "tLPnlStdVal";
			tLPnlStdVal.ColumnCount = 1;
			tLPnlStdVal.ColumnStyles.Add(new ColumnStyle());
			tLPnlStdVal.RowCount = 1; 
			tLPnlStdVal.Dock = DockStyle.Top;
			tLPnlStdVal.RowStyles.Add(new RowStyle());
			tLPnlStdVal.TabIndex = 1;

            nmtxtStdVal99_101.Text = mDtStandardValProductTypeMst.AsEnumerable().FirstOrDefault(
                x => x.Table.Columns[2].ColumnName == ColumnStandardValProductTypeMst.HUMAN_TIME)?.Field<Decimal>(ColumnStandardValProductTypeMst.HUMAN_TIME).ToString() ?? String.Empty;
			nmtxtStdVal99_101.TextChanged += new EventHandler(OnTextChanged);

			nmtxtStdVal99_102.Text = mDtStandardValProductTypeMst.AsEnumerable().FirstOrDefault(
				x => x.Table.Columns[3].ColumnName == ColumnStandardValProductTypeMst.MACHINE_TIME)?.Field<Decimal>(ColumnStandardValProductTypeMst.MACHINE_TIME).ToString() ?? String.Empty;
			nmtxtStdVal99_102.TextChanged += new EventHandler(OnTextChanged);

			nmtxtStdVal99_103.Text = mDtStandardValProductTypeMst.AsEnumerable().FirstOrDefault(
				x => x.Table.Columns[4].ColumnName == ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME)?.Field<Decimal>(ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME).ToString() ?? String.Empty;

			iCnt = 0;

			UCStandardVal_Table_Item ucStdValTbl = new PLOSMaintenance.UCStandardVal_Table_Item(CompositionId, ProductTypeId, mDtCompositionProcessMst, this);
			if(!ucStdValTbl.EnableStandardValView)
            {
				return false;
            }

			ucStdValTbl.AutoSize = true;
			ucStdValTbl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ucStdValTbl.BackColor = SystemColors.Control;
			ucStdValTbl.Location = new Point(3, 3);
			ucStdValTbl.Name = $"ucStdValTbl_{iCnt:00}";
			ucStdValTbl.TabIndex = 0;

			ucStdValTbl.CompositionId = CompositionId;
			ucStdValTbl.ProductTypeId = ProductTypeId;

			ucStdValTbl.DtCompositionProcessMst = mDtCompositionProcessMst;
			ucStdValTbl.StandardValChanged += UcStdValTbl_StandardValChanged;
			ucStdValTbl.HideGraphCheckedChanged += OnHideGraphCheckedChanged;

			mStdValTblList.Add(ucStdValTbl);
			tLPnlStdVal.Controls.Add(ucStdValTbl, 0, iCnt);

			if (iCnt == 0)
			{
				ShowChart(mDtCompositionProcessMst, ucStdValTbl.ProductTypeId, ucStdValTbl.GetAllStdVal());
			}
			iCnt++;

			pnlGraph.Controls.Add(this.chartStdVal);

			((ISupportInitialize)(this.chartStdVal)).EndInit();

			pnlGraph.ResumeLayout(false);
			pnlGraph.PerformLayout();
			pnlTop.ResumeLayout(false);
			pnlTop.PerformLayout();
			tLPnlStdVal.ResumeLayout(false);
			tLPnlStdVal.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

			return true;
		}

		/// <summary>
		/// グラフ表示
		/// </summary>
		/// <param name="DtCompositionProcessMst"></param>
		/// <param name="ProductTypeId"></param>
		/// <param name="stdValList"></param>
		private void ShowChart(DataTable DtCompositionProcessMst, Guid ProductTypeId, List<UCStandardVal_Table_Item.StdValItem> stdValList)
		{
			var XAxisList = DtCompositionProcessMst.AsEnumerable().Select(x =>
				new { ProcessName = x.Field<String>("ProcessName"), ProcessIdx = x.Field<Int32>("ProcessIdx") }).ToList();

			foreach (var item in chartStdVal.Series)
			{
				item.Points.Clear();
			}

			foreach (var xaxis in XAxisList)
			{
				foreach (var stdValType in ChartSeriesStdValTypeList)
				{
					DataPoint dp = new DataPoint();
					decimal ypoint = stdValList.FirstOrDefault(
						x => x.ProcessIdx == xaxis.ProcessIdx
						&& x.StdValType == stdValType.StdValType)?.StdVal ?? 0.0M;

					decimal ypoint_Subtraction = stdValList.FirstOrDefault(
						x => x.ProcessIdx == xaxis.ProcessIdx
						&& x.StdValType == stdValType.StdValType_Subtraction)?.StdVal ?? 0.0M;

					if ("Series_CTAve" == stdValType.ChartSeries)
					{
						if (xaxis.ProcessIdx >= 0)
						{
							dp.SetValueXY(xaxis.ProcessIdx + 1.2, ypoint - ypoint_Subtraction);
							chartStdVal.Series[stdValType.ChartSeries].Points.Add(dp);
						}
					}
					else
					{
						dp.SetValueXY(xaxis.ProcessIdx + 1, ypoint - ypoint_Subtraction);
						chartStdVal.Series[stdValType.ChartSeries].Points.Add(dp);
					}
				}
			}
		}
        #endregion "プライベートメソッド"
    }
}
