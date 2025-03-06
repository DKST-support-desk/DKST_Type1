using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace PLOS.Gui.Core.CustumContol
{
    /// <summary>
	/// NumericTextBox
    /// </summary>
    public partial class NumericTextBox : PermitedTextBoxBase
    {
		#region Private Fields

		private Double _MaxValue;
		private Double _MinValue;

		/// <summary>
		/// 符号付 許可 (True:許可)
		/// </summary>
		private Boolean _AllowLeadingSign;
		/// <summary>
		/// 小数点入力 許可 (True:許可)
		/// </summary>
		private Boolean _AllowDecimalPoint;

		/// <summary>
		/// 最大整数桁数
		/// </summary>
		private uint _MaxIntegerDigits;
		/// <summary>
		/// 最大小数点桁(端数)
		/// </summary>
		private uint _MaxFractionalDigits;

		private CultureInfo _cultureInfo;

		/// <summary>
		/// 値が 0 の時非表示
		/// </summary>
		private Boolean _HideZero;

		///// <summary>
		///// 符号文字
		///// </summary>
		//private char[] _SignChars;

		/// <summary>
		/// 小数点区切り文字
		/// </summary>
		//private char[] _DecimalPointChars;

		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// 
		/// </summary>
		[Category("CustomControl")]
		public NumericTextBox()
        {
			_MaxValue = Double.MaxValue;
			_MinValue = Double.MinValue;
			_MaxIntegerDigits = 8;
			_MaxFractionalDigits = 2;
			_AllowLeadingSign = true;
			_AllowDecimalPoint = true;
			_HideZero = false;

			InitializeComponent();

            base._PermitChars = 
				new char[]{'0','1','2','3','4','5','6','7','8','9'};
				//,'-','+','.'};	/*,'e','E'*/
				//new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '+', '.' };	/*,'e','E'*/

			// Get Current Culture Info
			CultureInfo = CultureInfo.CurrentCulture;
		}
        #endregion

		#region Property

		/// <summary>
		/// MinValue
		/// </summary>
		[Category("Numeric Text")]
		//[TypeConverter(typeof(DecimalConverter))]
		public Double MinValue
		{
			get
			{
				return _MinValue;
			}

			set
			{
				_MinValue = value;
			}
		}

		/// <summary>
		/// MaxValue
		/// </summary>
		[Category("Numeric Text")]
		//[TypeConverter(typeof(DecimalConverter))]
		public Double MaxValue
		{
			get
			{
				return _MaxValue;
			}
			set
			{
				_MaxValue = value;
			}
		}

		/// <summary>
		/// CultureInfo
		/// </summary>
		[Category("Numeric Text")]
		public CultureInfo CultureInfo
		{
			get
			{
				return _cultureInfo;
			}
			set
			{
				if (value != null) // just prevent unnecessary checking...
				{
					if (value is CultureInfo)
					{
						_cultureInfo = value;

						// 1文字のみ許可文字列
						SetPermitAtOnece();
					}
				}
			}
		}

		/// <summary>
		/// PositiveSign
		/// </summary>
		[Category("Numeric Text")]
		public String PositiveSign
		{
			get
			{
				return _cultureInfo.NumberFormat.PositiveSign;
			}
		}

		/// <summary>
		/// NegativeSign
		/// </summary>
		[Category("Numeric Text")]
		public String NegativeSign
		{
			get
			{
				return _cultureInfo.NumberFormat.NegativeSign;
			}
		}

		/// <summary>
		/// NumberDecimalSeparator
		/// </summary>
		[Category("Numeric Text")]
		public String NumberDecimalSeparator
		{
			get
			{
				return _cultureInfo.NumberFormat.NumberDecimalSeparator;
			}
		}

		/// <summary>
		/// NumberGroupSeparator
		/// </summary>
		[Category("Numeric Text")]
		public String NumberGroupSeparator
		{
			get
			{
				return _cultureInfo.NumberFormat.NumberGroupSeparator;
			}
		}

		/// <summary>
		/// 小数点桁数
		/// </summary>
		[Category("Numeric Text")]
		[Description("小数点(端数)桁数")]
		public uint MaxFractionalDigits
		{
			get { return _MaxFractionalDigits; }
			set {
				_MaxFractionalDigits = value;
				if (_MaxFractionalDigits <= 0) _AllowDecimalPoint = false;
				else _AllowDecimalPoint = true;
			}
		}

		/// <summary>
		/// 整数桁数
		/// </summary>
		[Category("Numeric Text")]
		[Description("整数桁数")]
		public uint MaxIntegerDigits
		{
			get { return _MaxIntegerDigits; }
			set	{ _MaxIntegerDigits = value;}
		}

		/// <summary>
		/// 先頭に符号文字可能/不可
		/// </summary>
		[Category("Numeric Text")]
		[Description("先頭に符号文字可能/不可")]
		public Boolean AllowLeadingSign
		{
			get { return _AllowLeadingSign; }
			set {
				_AllowLeadingSign = value;
				// 1文字のみ許可文字列
				SetPermitAtOnece();
			}
		}

		/// <summary>
		/// 小数点文字可能/不可能
		/// </summary>
		[Category("Numeric Text")]
		[Description("小数点文字可能/不可能")]
		public Boolean AllowDecimalPoint
		{
			get { return _AllowDecimalPoint; }
			set { 
				_AllowDecimalPoint = value;
				// 1文字のみ許可文字列
				SetPermitAtOnece();
			}
		}

		/// <summary>
		/// 値が 0 の時非表示
		/// </summary>
		[Category("Numeric Text")]
		[Description("値が 0 の時非表示")]
		public Boolean HideZero
		{
			get { return _HideZero; }
			set
			{
				_HideZero = value;
			}
		}

		#endregion

		#region Protectede Override Methods

		#region IsValid メソッド

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <param name="stTarget"></param>
		/// <param name="chTarget"></param>
		/// <returns></returns>
		protected override bool IsValid(string stTarget)
		{
			Double dOut;
			// 符号文字列チェック
			foreach (char item in NegativeSign + PositiveSign)	//_SignChars)
			{
				if (stTarget.IndexOf(item) > 0)
				{
					// 符号が1文字目以外に出現した場合、
					// 不整合(符号2種類入力への対応にもなる)
					return false;
				}
			}
			// 最大値比較
			if (_MaxValue != Double.MaxValue && Double.TryParse(stTarget, out dOut))
			{
				if (_MaxValue < dOut) return false;
			}

			// 最小値比較
			if (_MinValue != Double.MinValue && Double.TryParse(stTarget, out dOut))
			{
				if (_MinValue > dOut) return false;
			}

			// 小数点文字可能/不可能
			if (!AllowDecimalPoint)
			{
				int iDecimalPointPosition = stTarget.IndexOf(NumberDecimalSeparator);
				if (iDecimalPointPosition != -1)
				{
					// 小数点が存在した場合
					return false;
				}
			}
			
			// 整数桁数、少数桁数チェック
			if (!IsValidDigits(stTarget)) return false;

			return base.IsValid(stTarget);
		}

		/// <summary>
		/// Value
		/// </summary>
		[Category("Numeric Text")]
		override public decimal Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				if(_HideZero && value == 0.0M)
				{ base.Text = "";  }
                else { base.Value = value; }
			}
		}
		#endregion

		#endregion

		#region Private Methods

		/// <summary>
		/// 1文字のみ許可文字列
		/// </summary>
		private void SetPermitAtOnece()
		{
			base._PermitCharsAtOnece.Clear();

			// 符号
			if (AllowLeadingSign)
			{
				foreach (char item in NegativeSign + PositiveSign)	//_SignChars)
				{
					_PermitCharsAtOnece.Add(item);
				}
			}

			// 小数点
			if (AllowDecimalPoint)
			{
				foreach (char item in NumberDecimalSeparator)
				{
					_PermitCharsAtOnece.Add(item);

				}
			}
		}

		/// <summary>
		/// 整数桁数、少数桁数チェック
		/// </summary>
		/// <returns></returns>
		private bool IsValidDigits(string stTarget)
		{
			uint uIntegerDigits = 1;
			uint uFractionalDigits = 0;

			// 文字列先頭に符号があれば削除 桁数判断に符号は無関係
			stTarget = stTarget.TrimStart(new char[] { '+', '-' });

			// 小数点位置を探す
			int iPointPos = stTarget.IndexOf('.');
			if (iPointPos < 0)
			{
				uIntegerDigits = (uint)stTarget.Length;
			}
			else if (iPointPos == 0)
			{
				uFractionalDigits = (uint)stTarget.Length - 1;
			}
			else
			{
				uIntegerDigits = (uint)iPointPos;
				uFractionalDigits = (uint)(stTarget.Length - 1 - iPointPos);
			}

			if (uIntegerDigits > this._MaxIntegerDigits) return false;
			if (uFractionalDigits > this._MaxFractionalDigits) return false;

			return true;
		}

		#endregion
	}
}
