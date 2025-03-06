using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace PLOS.Gui.Core.DataGridViews
{
    /// <summary>
    /// Displays editable numeric text information in a 
    /// <see cref="DataGridView"/> control.
    /// </summary>
    public class DataGridViewNumericCell : DataGridViewTextBoxCell
	{
		#region Private Fields

		#endregion
		
		#region Constructors and Destructor

		/// <summary>
		/// Constructors
        /// </summary>
        public DataGridViewNumericCell()
        {
        }

        #endregion

        #region Public Properties

 		/// <summary>
		/// EditType
		/// </summary>
        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewNumericEditingControl);
            }
        }
        #endregion

        #region Public Methods

		/// <summary>
		/// 編集コントロールを初期化
		/// InitializeEditingControl
		/// </summary>
		/// <param name="rowIndex"></param>
		/// <param name="initialFormattedValue"></param>
		/// <param name="dataGridViewCellStyle"></param>
        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);

			// 編集コントロールの取得
            DataGridViewNumericEditingControl ctl =
                DataGridView.EditingControl as DataGridViewNumericEditingControl;

            if (ctl != null)
            {
				DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;

				// カスタム列のプロパティを反映
				if (column != null)
				{
				//    ctl.NumericType       = column.NumericType;
				//    ctl.CustomNumericType = column.CustomNumericType;
					ctl.MinValue = column.MinValue;
					ctl.MaxValue = column.MaxValue;
				    ctl.CultureInfo = column.CultureInfo;
				//    ctl.Options           = column.Options;
					ctl.AllowLeadingSign = column.AllowLeadingSign;
					ctl.AllowDecimalPoint = column.AllowDecimalPoint;
					ctl.MaxFractionalDigits = column.MaxFractionalDigits;
					ctl.MaxIntegerDigits = column.MaxIntegerDigits;

					if (column.MaxLength >= 0)
				    {
				        ctl.MaxLength = column.MaxLength;
				    }

					if (column.ToolTip != null)
					{
						ctl.ToolTip = column.ToolTip;
					}
				}

                try
                {
                    if (initialFormattedValue != null)
                    {
                        ctl.Text = Convert.ToString(initialFormattedValue);
                    }  
                    else
                    {
                        int cellRow = rowIndex;
                        if (cellRow < 0)
                        {
                            cellRow = this.RowIndex;
                        }

						if (cellRow >= 0)
						{
							ctl.Text = this.GetValue(rowIndex) as string;
						}
                    }
                }
                catch
                {
                    //ctl.Text = Convert.ToString(initialFormattedValue);
                }
            }
        }

        #endregion
    }
}
