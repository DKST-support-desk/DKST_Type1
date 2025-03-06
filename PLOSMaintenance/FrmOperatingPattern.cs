using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace PLOSMaintenance
{
	public partial class FrmOperatingPattern : Form
	{
		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public String PatternName 
		{ 
			get
			{ 
				return txtPatternName.Text; 
			}
			set
			{
				txtPatternName.Text = value;
			}
		}

		public FrmOperatingPattern()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 登録ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOKClicked(object sender, EventArgs e)
		{
			logger.Info("パターン登録　登録ボタン押下");

			// 入力を確認
            if (String.IsNullOrWhiteSpace(txtPatternName.Text))
            {
				logger.Warn("パターン登録名が入力されていません。");
				MessageBox.Show(this, "パターン登録名が入力されていません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
            {
				string patternName = txtPatternName.Text.Trim();

				// 文字数確認
                if (patternName.Length < 1 || 21 < patternName.Length)
                {
					logger.Warn("パターン登録名は1文字以上21文字以内で入力してください。");
					MessageBox.Show(this, "パターン登録名は1文字以上21文字以内で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
                else if(DialogResult.Yes == MessageBox.Show(this, "この登録名で登録してよろしいですか？", "登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
					logger.Warn("Yes押下");

					DialogResult = DialogResult.OK;

					Close();
				}
                else
                {
					logger.Warn("No押下");
				}
            }
		}

		/// <summary>
		/// キャンセルボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClicked(object sender, EventArgs e)
		{
			logger.Info("パターン登録　キャンセルボタン押下");

			DialogResult = DialogResult.Cancel;

			Close();
		}
	}
}
