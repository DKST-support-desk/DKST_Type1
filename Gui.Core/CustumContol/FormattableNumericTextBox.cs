using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PLOS.Gui.Core.CustumContol
{
	public partial class FormattableNumericTextBox : NumericTextBox
	{
		#region Private Fields

		/// <summary>
		/// _Value
		/// </summary>
		private decimal _Value = 0.0M;

		/// <summary>
		/// ToString Format
		/// </summary>
		private String _ToStringFormat;

		#endregion

		#region EventHandler(Delegate)
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public FormattableNumericTextBox()
		{
			InitializeComponent();

			_ToStringFormat = "N";

			this.Enter += new EventHandler(OnTextBoxEnter);
			this.Leave += new EventHandler(OnTextBoxLeave);
		}

		#endregion

		#region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTextBoxLeave(object sender, EventArgs e)
		{
			if (!String.IsNullOrWhiteSpace(_ToStringFormat))
			{
				if (!String.IsNullOrWhiteSpace(Text))
				{
					Value = ParseText2Value(Text);

					base.Text = FormatToString();
				}
				else
				{
					Value = 0.0M;
					base.Text = FormatToString();
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTextBoxEnter(object sender, EventArgs e)
		{
			// 桁区切り変換しない
			base.Text = Value.ToString();
		}

		#endregion

		#region Property

		/// <summary>
		/// Value
		/// </summary>
		[Category("Numeric Text")]
		[TypeConverter(typeof(DecimalConverter))]
		public decimal Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
				base.Text = FormatToString();
			}
		}

		/// <summary>
		/// ToString Format
		/// </summary>
		[Category("Numeric Text")]
		public String ToStringFormat
		{
			get
			{
				return _ToStringFormat;
			}
			set
			{
				_ToStringFormat = value;
			}
		}


		/// <summary>
		/// Value
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				Value = ParseText2Value(value);
			}
		}

		#endregion

		#region Private Methods

		private decimal ParseText2Value(String srcText)
		{
			// 数値変換 NumericTextBox なので通所変換エラーは発生しない (ので無視)
			try { return decimal.Parse(srcText); }
			catch { return 0M; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private String FormatToString()
		{
			try
			{
				return Value.ToString(_ToStringFormat, CultureInfo);
			}
			catch
			{
				return "[Critical ERROR] Format ERROR";
			}
		}

		#endregion
	}
}