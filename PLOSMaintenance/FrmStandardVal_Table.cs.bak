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
using log4net;

namespace PLOSMaintenance
{
	public partial class FrmStandardVal_Table : Form
	{
        //********************************************
        //* メンバ変数
        //********************************************
        #region "メンバ変数"
		/// <summary>
		/// 品番タイプマスタテーブルアクセス
		/// </summary>
		private SqlProductTypeMst mSqlProductTypeMst;

		/// <summary>
		/// 品番タイプマスタテーブルデータ
		/// </summary>
		private DataTable mDtProductType_Mst;

		/// <summary>
		/// CT・機器設定コントロールリスト
		/// </summary>
		private List<UCStandardVal_Table_Composition> mStdValTblList = new List<UCStandardVal_Table_Composition>();

		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion "メンバ変数"

		//********************************************
		//* プロパティ
		//********************************************
		#region "プロパティ"
		/// <summary>
		/// 編成ID
		/// </summary>
		private Guid CompositionId { get; set; } = Guid.Empty;
		#endregion "プロパティ"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region "コンストラクタ"
		/// <summary>
		/// 標準値マスタ
		/// </summary>
		public FrmStandardVal_Table()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 標準値マスタ
		/// </summary>
		public FrmStandardVal_Table(Guid compositionId, string numberPeople, string uniqueName)
		{
			InitializeComponent();

			this.CompositionId = compositionId;
			mStdValTblList.Clear();

			// DB取得
			mSqlProductTypeMst = new SqlProductTypeMst(Properties.Settings.Default.ConnectionString_New);
			mDtProductType_Mst = mSqlProductTypeMst.Select();

			foreach (DataRow rowItem in mDtProductType_Mst.Rows)
			{
				Guid productTypeId = rowItem.Field<Guid>(ColumnProductTypeMst.PRODUCT_TYPE_ID);

				PLOSMaintenance.UCStandardVal_Table_Composition ucStdVal = new PLOSMaintenance.UCStandardVal_Table_Composition(compositionId, productTypeId, numberPeople, uniqueName);
				if(!ucStdVal.IsEnableShowStandardVal)
                {
					IsEnableShowDialog = false;
					return;
                }

				mStdValTblList.Add(ucStdVal);

				ucStdVal.Dock = System.Windows.Forms.DockStyle.Fill;
				ucStdVal.Name = "ucDataCollectionSetting";
				ucStdVal.TabIndex = 0;
				ucStdVal.RegistClicked += OnRegistButtonClicked;
				ucStdVal.BackClicked += OnClose;
				ucStdVal.HideGraphCheckedChanged += OnHideGraphCheckedChanged;

				System.Windows.Forms.TabPage tabpg = new System.Windows.Forms.TabPage();

                tabStdVal.Controls.Add(tabpg);
                tabpg.Location = new System.Drawing.Point(4, 25);
                tabpg.Name = "tabpg_" + rowItem.Field<String>(ColumnProductTypeMst.PRODUCT_TYPE_NAME);
                tabpg.Padding = new System.Windows.Forms.Padding(3);
                tabpg.Size = new System.Drawing.Size(792, 421);
                tabpg.TabIndex = 0;
                tabpg.Text = rowItem.Field<String>(ColumnProductTypeMst.PRODUCT_TYPE_NAME);
                tabpg.UseVisualStyleBackColor = true;
                tabpg.Controls.Add(ucStdVal);
			}
			IsEnableShowDialog = true;
		}
		#endregion "コンストラクタ"

		//********************************************
		//* プロパティ
		//********************************************
		#region "プロパティ"

		public Boolean IsEnableShowDialog
        {
			get; set;
        }
		#endregion "プロパティ"

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// 登録ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			logger.Info("標準値マスタ 登録ボタン押下");

			try
            {
				// 入力チェック
				string errMsg = string.Empty;
				bool checkRet = true;
				foreach (var item in mStdValTblList)
				{
					checkRet = item.CheckInput(out errMsg);

					if (!checkRet)
					{
						break;
					}
				}

				if (!checkRet)
				{
					logger.Warn("標準値マスタ " + errMsg);
					MessageBox.Show(errMsg, "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// 設定登録
				bool registRet = true;
				foreach (var item in mStdValTblList)
				{
					registRet = item.Regist();

					if (!registRet)
					{
						break;
					}
				}

				if(registRet)
                {
					logger.Info("標準値マスタ DB登録完了");
					MessageBox.Show("登録しました。", "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
					Close();
				}
				else
                {
					logger.Info("標準値マスタ DB登録失敗");
					MessageBox.Show("登録に失敗しました。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				logger.Info("標準値マスタ DB登録失敗");
				MessageBox.Show("登録に失敗しました。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 戻るボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClose(object sender, EventArgs e)
		{
            try
            {
				bool check = false;

				logger.Info("標準値マスタ 戻るボタン押下");

				// タブ毎の編集フラグをチェック
				foreach (var item in mStdValTblList)
				{
					if (item.ChangeValueFlg)
					{
						item.OnClose();
						return;
					}
				}

				Close();
				logger.Info("標準値マスタ画面を閉じました。");
			}
			catch(Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
		}

		/// <summary>
		/// グラフ表示切替
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHideGraphCheckedChanged(object sender, EventArgs e)
		{
			CheckBox ctrl = sender as CheckBox;
			if (ctrl != null)
			{
				foreach (var item in mStdValTblList)
				{
					item.GraphVisible = ctrl.Checked;
				}
			}
		}

        /// <summary>
        /// ロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmStandardVal_Table_Load(object sender, EventArgs e)
        {
            try
            {
				//一度全タブの切替を行うことで別タブのコントロールの読込を行う ※先に読み込まないと登録ボタン押下時に読込前のコントロールで例外が発生する
				for (int i = 0; i < tabStdVal.TabCount; i++)
				{
					tabStdVal.SelectedIndex = i;
				}
				tabStdVal.SelectedIndex = 0;
			}
            catch (Exception ex)
            {
				logger.Error(ex.Message + ",\n" + ex.StackTrace);
            }
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        #endregion "プライベートメソッド"
    }
}
