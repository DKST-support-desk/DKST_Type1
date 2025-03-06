using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLOSMaintenance
{
    /// <summary>
    /// パスワード入力画面
    /// </summary>
    public partial class FrmIdentifierPassword : Form
    {
        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmIdentifierPassword()
        {
            InitializeComponent();
        }

        #endregion "コンストラクタ"


        //********************************************
        //* イベント
        //********************************************
        #region "イベント"

        /// <summary>
        /// 登録ボタン押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOKClicked(object sender, EventArgs e)
        {
            //Common.Logger.Instance.WriteTraceLog("登録ボタン押下");
            //Common.Logger.Instance.WriteTraceLog("入力パスワード:" + txtIdentifierPassword.Text);

            if (Properties.Settings.Default.IdentifierPassword == txtIdentifierPassword.Text)
            {
                //Common.Logger.Instance.WriteTraceLog("パスワード正常");
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                //Common.Logger.Instance.WriteTraceLog("パスワードが違います。");
                MessageBox.Show(this, "パスワードが違います。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャンセルボタン押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelClicked(object sender, EventArgs e)
        {
            //Common.Logger.Instance.WriteTraceLog("キャンセルボタン押下");

            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion "イベント"
    }
}
