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
    /// からのパクリ＋α
    /// </summary>
    public partial class PermitedTextBox : PermitedTextBoxBase
    {
        #region Private Fields
        #endregion

        #region Constructors and Destructor
        /// <summary>
		/// Constructors
        /// </summary>
        public PermitedTextBox()
        {
            InitializeComponent();
        }
        #endregion

        #region Event

        #endregion

        #region Property

        #region PermitChars プロパティ

		/// <summary>
		/// PermitChars
		/// </summary>
		[Category("CustomControl")]
		[Browsable(false)]
		public IList<char> PermitChars
        {
            get
            {
                return this._PermitChars;
            }

            set
            {
				if (value != null) this._PermitChars = value;
            }
        }

		/// <summary>
		/// PermitCharsAtOnece
		/// </summary>
		[Category("CustomControl")]
		[Browsable(false)]
		public IList<char> PermitCharsAtOnece
		{
			get
			{
				return this._PermitCharsAtOnece;
			}

			set
			{
				if (value != null) this._PermitCharsAtOnece = value;
			}
		}

		/// <summary>
		/// Prohibited characters
		/// </summary>
		[Category("CustomControl")]
		[Browsable(false)]
		public IList<char> ProhibitedChars
		{
			get
			{
				return this._ProhibitedChars;
			}

			set
			{
				if (value != null) this._ProhibitedChars = value;
			}
		}

		/// <summary>
		/// PermitRegularExpression
		/// </summary>
		[Category("CustomControl")]
		public String PermitRegularExpressionPattern
		{
			get
			{
				return _PermitRegularExpressionPattern;
			}

			set
			{
				_PermitRegularExpressionPattern = value.Trim();
			}
		}

		#endregion

        #endregion
    }
}
