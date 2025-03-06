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
    public partial class FrmPasswordChange : Form
    {
        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmPasswordChange()
        {
            InitializeComponent();
        }

        #endregion "コンストラクタ"


        //********************************************
        //* イベント
        //********************************************
        #region "イベント"

        /// <summary>
        /// パスワード変更ボタン押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOKClicked(object sender, EventArgs e)
        {
            //Common.Logger.Instance.WriteTraceLog("パスワード変更ボタン押下");
            //Common.Logger.Instance.WriteTraceLog("入力パスワード:" + txtNowPass.Text + " 新しいパスワード:" + txtNewPass + " 新しいパスワード(再入力):" + txtNewPassAgain);

            //未入力チェック
            if (string.IsNullOrWhiteSpace(txtNowPass.Text))
            {
                //Common.Logger.Instance.WriteTraceLog("パスワードが入力されていません。");
                MessageBox.Show(this, "パスワードが入力されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPass.Text))
            {
                //Common.Logger.Instance.WriteTraceLog("新しいパスワードが入力されていません。");
                MessageBox.Show(this, "新しいパスワードが入力されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassAgain.Text))
            {
                //Common.Logger.Instance.WriteTraceLog("新しいパスワード(再入力)が入力されていません。");
                MessageBox.Show(this, "新しいパスワード(再入力)が入力されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //パスワードチェック
            if (Properties.Settings.Default.IdentifierPassword != txtNowPass.Text)
            {
                //Common.Logger.Instance.WriteTraceLog("パスワードが違います。");
                MessageBox.Show(this, "パスワードが違います。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //新しいパスワード合致チェック
            if (txtNewPass.Text != txtNewPassAgain.Text)
            {
                //Common.Logger.Instance.WriteTraceLog("新しいパスワードと新しいパスワード(再入力)が一致しません。");
                MessageBox.Show(this, "新しいパスワードと新しいパスワード(再入力)が一致しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //設定ファイルの書き換え
            //※設定はユーザ毎に以下に保存される模様
            //  C:\Users\[ユーザー名]\AppData\Local\PLOSMaintenance\PLOSMaintenance.exe_Url_xxfxmf3corgmj1w2dze0tty3og4tdwvo\1.0.0.0\user.config
            Properties.Settings.Default.IdentifierPassword = txtNewPass.Text;
            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
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
