using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
    /// <summary>
    /// http://jeanne.wankuma.com/tips/csharp/textbox/permitchars.html
    /// からの流用
    /// </summary>
    public partial class PermitedTextBoxBase : System.Windows.Forms.TextBox
    {
        #region Protected Fields
		// relevant Windows messages...
		protected const int WM_CHAR = 0x0102;
		protected const int WM_CUT = 0x0300;
		protected const int WM_COPY = 0x0301;
		protected const int WM_PASTE = 0x0302;

		/// <summary>
		/// Permit characters
        /// </summary>
		protected IList<char> _PermitChars = new List<char>();

        /// <summary>
		/// Permit At Onece characters
        /// </summary>
		protected IList<char> _PermitCharsAtOnece = new List<char>();

		/// <summary>
		/// Prohibited characters
		/// </summary>
		protected IList<char> _ProhibitedChars = new List<char>();

		/// <summary>
		/// 
		/// </summary>
		protected String _PermitRegularExpressionPattern = "";

		private System.Windows.Forms.ToolTip _ToolTip;

        #endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
        /// </summary>
        public PermitedTextBoxBase()
        {
			InitializeComponent();
        }

        #endregion

        #region Event

        #region WndProc メソッド (override)

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_CHAR:
					if (_PermitChars.Count > 0 ||
						_PermitCharsAtOnece.Count > 0 ||
						_ProhibitedChars.Count > 0 ||
						!String.IsNullOrWhiteSpace(_PermitRegularExpressionPattern))
                    {
                        KeyPressEventArgs eKeyPress = new KeyPressEventArgs((char)(m.WParam.ToInt32()));
                        this.OnChar(eKeyPress);

                        if (eKeyPress.Handled)
                        {
                            return;
                        }
                    }
                    break;
                case WM_PASTE:
					if (_PermitChars.Count > 0 ||
						_PermitCharsAtOnece.Count > 0 ||
						_ProhibitedChars.Count > 0 ||
						!String.IsNullOrWhiteSpace(_PermitRegularExpressionPattern))
                    {
                        this.OnPaste(new System.EventArgs());
                        return;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region OnChar メソッド (virtual)

		/// <summary>
		/// OnChar
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnChar(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            if (!IsPermitChar(e.KeyChar))
            {
                e.Handled = true;
				return;
            }

			// 整合性チェック
			if (!IsValid(GetForecastText(e.KeyChar.ToString())))
			{
				e.Handled = true;
			}
        }

        #endregion

        #region OnPaste メソッド (virtual)

		/// <summary>
		/// OnPaste
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnPaste(System.EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
			object clipStringObject = null;

            if (data == null) return;

			if ((clipStringObject = data.GetData(System.Windows.Forms.DataFormats.Text)) == null) return;

			// Get String @ Clipboard
			string clipString = clipStringObject.ToString();

			// クリップボード文字列が null または 文字列長が 0以上ならば判断
			if (clipString != null && clipString.Length > 0)
            {
				this.SelectedText = GetPermitedString(clipString);
            }
        }

		#endregion

		#endregion

		#region Property


		/// <summary>
		/// Value
		/// </summary>
		[Category("Numeric Text")]
		virtual public decimal Value
		{
			get
			{
				decimal dOut;
				if (decimal.TryParse(base.Text, out dOut)) return dOut;

				return 0;
			}
			set
			{
				base.Text = value.ToString();
			}
		}

		/// <summary>
		/// ToolTip
		/// </summary>
		[Category("Numeric Text")]
		public ToolTip ToolTip
		{
			get
			{
				return _ToolTip;
			}
			set
			{
				_ToolTip = value;
			}
		}

        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods

        #region IsPermitChar メソッド

		/// <summary>
		/// IsPermitChar
		/// </summary>
		/// <param name="chTarget"></param>
		/// <returns></returns>
		protected bool IsPermitChar(char chTarget)
        {
            foreach (char ch in _PermitChars)
            {
                if (chTarget == ch)
                {
					return true;
                }
            }

			if (IsPermitCharAtOnce(base.Text, chTarget))
			{
				return true;
			}

			// Check Prohibited characters
            foreach (char ch in _ProhibitedChars)
            {
                if (chTarget == ch)
                {
					return false;
                }
            }

			if (_PermitChars.Count <= 0 && _PermitCharsAtOnece.Count <= 0) return true;

            return false;
        }

        #endregion

        #region GetPermitedString メソッド

		/// <summary>
		/// GetPermitedString
		/// </summary>
		/// <param name="strTarget"></param>
		/// <returns></returns>
		protected string GetPermitedString(string strTarget)
        {
            string strRet = string.Empty;

            foreach (char chTarget in strTarget)
            {
                if (IsPermitChar(chTarget))
                {
 					// 整合性チェック
					if (IsValid(GetForecastText(strTarget)))
					{
						strRet += chTarget;
					}
				}
            }

            return strRet;
        }

        #endregion

		#region IsValid メソッド

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <param name="stTarget"></param>
		/// <param name="chTarget"></param>
		/// <returns></returns>
		protected virtual bool IsValid(string stTarget)
		{
			if (!String.IsNullOrWhiteSpace(_PermitRegularExpressionPattern))
			{
				return System.Text.RegularExpressions.Regex.IsMatch(stTarget, _PermitRegularExpressionPattern);
			}

			return true;
		}

		#endregion

        #endregion

		#region Private Methods

		#region IsPermitCharAtOnce メソッド

		/// <summary>
		/// IsPermitCharAtOnce
		/// </summary>
		/// <param name="stTarget"></param>
		/// <param name="chTarget"></param>
		/// <returns></returns>
		private bool IsPermitCharAtOnce(string stTarget, char chTarget)
		{
			foreach (char ch in _PermitCharsAtOnece)
			{
				// 今回入力された"文字"がPermitCharsAtOneceに存在
				if (chTarget == ch)
				{
					// 追加する文字列にすでに存在した場合 許可されない
					if (stTarget.IndexOf(ch) != -1)
					{
						return false;
					}

					return true;
				}
			}

			return false;
		}

		#region GetForecastText メソッド

		/// <summary>
		/// 選択と入力は考慮して整合性判断すべき対象文字列を想定
		/// </summary>
		/// <param name="strInput"></param>
		/// <returns></returns>
		private String GetForecastText(String strInput)
		{
			String PrefixSelection = "";
			String SuffixSelection = "";

			if (this.SelectionStart > 0)
			{
				PrefixSelection = this.Text.Substring(0, this.SelectionStart);
			}

			if (this.Text.Length  - (this.SelectionStart + this.SelectionLength) > 0)
			{
				SuffixSelection = this.Text.Substring(this.SelectionStart + this.SelectionLength);
			}

			return PrefixSelection + strInput + SuffixSelection;
		}

		#endregion

		#endregion

		#endregion
	}
}
