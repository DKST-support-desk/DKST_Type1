using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
    /// <summary>
    /// メイン画面
    /// </summary>
	public partial class FrmMain : Form
    {
        //********************************************
        //* グローバル変数
        //********************************************
        #region "グローバル変数"

        /// <summary>
        /// 画面編集フラグ
        /// </summary>
        static public bool gIsDataChange = false;

        #endregion "グローバル変数"

        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"

        /// <summary>
        /// 現在の表示タブインデックス
        /// </summary>
        private int mNowTabIndex = 0;

        /// <summary>
        /// 各タブのインデックス番号
        /// </summary>
        private enum mTabIndex : int
        {
            operatingShift = 0,
            lineInfo,
            productType,
            deviceCamera,
            deviceDI,
            deviceDO,
            deviceQR,
            composition,
        }

        private SqlProductTypeMst mProductTypeMst;
        private SqlCompositionMst mCompositionMst;


        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion "メンバー変数"


        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            this.Size = new Size(Properties.Settings.Default.ScreenWidth, Properties.Settings.Default.ScreenHeight);
            // 品番タイプマスタクラス取得
            mProductTypeMst = new SqlProductTypeMst(Properties.Settings.Default.ConnectionString_New);
            // 編成マスタクラス取得
            mCompositionMst = new SqlCompositionMst(Properties.Settings.Default.ConnectionString_New);
            ucComposition.EventUpdateStandardVal += UcComposition_EventUpdateStandardVal;
        }


        #endregion "コンストラクタ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"

        /// <summary>
        /// タブセレクトイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //画面のデータを編集していたら確認メッセージを表示する
            if (gIsDataChange)
            {
                var result = MessageBox.Show(this, "表示内容が編集されています。\r\n編集を破棄し、画面遷移してもよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    //画面遷移する場合は元画面の表示を元に戻す
                    switch (mNowTabIndex)
                    {
                        //稼働登録画面
                        case (int)mTabIndex.operatingShift:
                            ucOperating_Shift_Regist.InitializeComponentData();
                            break;

                        //ライン情報登録画面
                        case (int)mTabIndex.lineInfo:
                            ucLineInfo.DispData();
                            break;

                        //品番登録画面
                        case (int)mTabIndex.productType:
                            ucProductType.InitializeDataGridView();
                            break;

                        //カメラ登録画面
                        case (int)mTabIndex.deviceCamera:
                            ucCamera.InitializeDataGridView();
                            break;

                        //DI機器登録画面
                        case (int)mTabIndex.deviceDI:
                            uC_Device_DI.InitializeDataGridView();
                            break;

                        //ブザー機器登録画面
                        case (int)mTabIndex.deviceDO:
                            uC_Device_DO.InitializeDataGridView();
                            break;

                        //QRリーダ登録画面
                        case (int)mTabIndex.deviceQR:
                            uC_Device_QR.InitializeDataGridView();
                            break;

                        //編成情報登録画面
                        case (int)mTabIndex.composition:
                            ucComposition.InitializeComponentData();
                            break;
                    }

                    //編集フラグを落とす
                    gIsDataChange = false;
                }
                else
                {
                    //Noなら画面遷移をキャンセルする
                    e.Cancel = true;
                    return;
                }
            }
            else if (tabMain.SelectedIndex.Equals((int)mTabIndex.composition))
            {
                if (CheckProductType())
                {
                    // 画面遷移をキャンセルする
                    e.Cancel = true;
                    return;
                }
            }

            //遷移後の画面インデックスを保持しておく
            mNowTabIndex = tabMain.SelectedIndex;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ScreenWidth = this.Size.Width;
            Properties.Settings.Default.ScreenHeight = this.Size.Height;
            Properties.Settings.Default.Save();
        }

        #endregion "イベント"

        #region "プライベートメソッド"
        /// <summary>
        /// 品番タイプ登録の有無によって画面遷移を判断
        /// </summary>
        private bool CheckProductType()
        {
            bool flg = false;

            // 品番タイプ登録が無ければ画面遷移キャンセル
            if (mProductTypeMst.Select().Rows.Count == 0)
            {
                MessageBox.Show("品番が設定されていません。\n品番タイプ登録画面から品番を登録してください。",
                                "編成情報登録", MessageBoxButtons.OK, MessageBoxIcon.Information);

                flg = true;
            }

            return flg;
        }

        private void UcComposition_EventUpdateStandardVal()
        {
            try
            {
                logger.Info(String.Format("標準値マスタ更新"));
                ucProductType.InitializeDataGridView();
                ucCamera.InitializeDataGridView();
                uC_Device_DI.InitializeDataGridView();
                uC_Device_DO.InitializeDataGridView();
                uC_Device_QR.InitializeDataGridView();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion "プライベートメソッド"

    }
}
