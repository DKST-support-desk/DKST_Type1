using System;
using System.Data;
using System.Text;
using System.Media;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace PLOS.Gui.Core.DataGridViews
{
    /// <summary>
    /// A <see cref="DataGridViewColumn"/> that allows only numeric text input.
	/// <para>
	/// DataGridViewColumnd Numeric Controler
	/// </para>
	/// </summary>
    public class DataGridViewNumericColumn : DataGridViewColumn
    {
        #region Private Fields

        private int	_maxLength;
		private Double _MaxValue;
		private Double _MinValue;

		//private string _tooltipText;

		private CultureInfo _cultureInfo;

		//private NumericType         _enumType;
		//private NumericType         _enumCustomType;

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

		/// <summary>
		/// Tool Tip String
		/// </summary>
		private System.Windows.Forms.ToolTip _toolTip;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewNumericColumn"/> class.
        /// </summary>
        public DataGridViewNumericColumn()
            : base(new DataGridViewNumericCell())
        {
			_toolTip = new System.Windows.Forms.ToolTip();

			_toolTip.AutoPopDelay = 1000;
			_toolTip.InitialDelay = 0;
			_toolTip.IsBalloon = true;
			_toolTip.ReshowDelay = 100;
			_toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.None;	//System.Windows.Forms.ToolTipIcon.Error;
			_toolTip.ToolTipTitle = "";

            _MaxValue       = Double.MaxValue;
            _MinValue       = Double.MinValue;
			_AllowLeadingSign = true;
			_AllowDecimalPoint = true;
			_MaxFractionalDigits = 2;
			_MaxIntegerDigits = 8;


			//_enumType       = NumericType.Double;
			//_enumCustomType = NumericType.Double;

			//_options        = NumericOptions.All;

			_cultureInfo = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _maxLength      = 32767;
        }

        #endregion

        #region Public Properties

		#region CellTemplate Set Get

		/// <summary>
		/// CellTemplate Set Get
		/// </summary>
		public override DataGridViewCell CellTemplate
		{
			get
			{
				return base.CellTemplate;
			}

			set
			{
				DataGridViewNumericCell cell = value as DataGridViewNumericCell;
				if ((value != null) && (cell == null))
				{
					throw new InvalidCastException(
						"The cell type must be DataGridViewNumericCell for this column.");
				}

				base.CellTemplate = value;
			}
		}

		#endregion

		/// <summary>
		/// Minimum Value
		/// </summary>
        [TypeConverter(typeof(DecimalConverter))]
        public Double MinValue
        {
            get
            {
                return _MinValue;
            }
            set
            {
				//if (value != null)
				//{
                    _MinValue = value;
				//}
            }
        }

		/// <summary>
		/// Max Value
		/// </summary>
        [TypeConverter(typeof(DecimalConverter))]
		public Double MaxValue
        {
            get
            {
                return _MaxValue;
            }
            set
            {
				//if (value != null)
				//{
                    _MaxValue = value;
				//}
            }
        }

		/// <summary>
		/// CultureInfo
		/// </summary>
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
					_cultureInfo = value;
				}
			}
		}

		/// <summary>
		/// ToolTip
		/// </summary>
		public ToolTip ToolTip
		{
			get
			{
				return _toolTip;
			}
		}

		/// <summary>
		/// Decimal Places
		/// </summary>
		[Category("Numeric Text")]
		[Description("Decimal Places")]
		public uint MaxFractionalDigits
		{
			get { return _MaxFractionalDigits; }
			set
			{
				_MaxFractionalDigits = value;
				if (_MaxFractionalDigits == 0) _AllowDecimalPoint = false;
			}
		}
		/// <summary>
		/// 侵umber of integer digits
		/// </summary>
		[Category("Numeric Text")]
		[Description("Number of integer digits")]
		public uint MaxIntegerDigits
		{
			get { return _MaxIntegerDigits; }
			set { _MaxIntegerDigits = value; }
		}
		/// <summary>
		/// Allow Leading Sign
		/// </summary>
		[Category("Numeric Text")]
		[Description("Allow Leading Sign")]
		public Boolean AllowLeadingSign
		{
			get { return _AllowLeadingSign; }
			set { _AllowLeadingSign = value; }
		}
		/// <summary>
		/// Allow Decimal Point
		/// </summary>
		[Category("Numeric Text")]
		[Description("Allow Decimal Point")]
		public Boolean AllowDecimalPoint
		{
			get { return _AllowDecimalPoint; }
			set { _AllowDecimalPoint = value; }
		}

		///// <summary>
		///// Gets or sets the number display options of this numeric text box.
		///// <para>
		///// Allow '+', '-', '.', 'e' Character
		///// </para>
		///// </summary>
		///// <value>
		///// An enumeration of the type <see cref="NumericOptions"/> specifying
		///// the number display options, such as positive and negative sign.
		///// The default is <see cref="NumericOptions.All"/>.
		///// </value>
		//public NumericOptions Options
		//{
		//    get
		//    {
		//        return _options;
		//    }

		//    set
		//    {
		//        _options = value;
		//    }
		//}

		/// <summary>
		/// Gets or sets the maximum number of characters allowed in the text 
		/// box. 
		/// </summary>
		/// <value>
		/// The maximum number of characters allowed in the text box.
		/// The default value is <c>0</c>, meaning that there is no limit 
		/// on the length. 
		/// </value>
		public int MaxLength
		{
			get
			{
				return _maxLength;
			}
			set
			{
				if (value >= 0)
				{
					_maxLength = value;
				}
			}
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            DataGridViewNumericColumn column = (DataGridViewNumericColumn)base.Clone();

            column._MaxValue       = this._MaxValue;
            column._MinValue       = this._MinValue;

            column._maxLength      = this._maxLength;

            return column;
        }

        #endregion
    }
}
