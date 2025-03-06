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
    /// 
    /// </summary>
    public partial class UIntTextBox : NumericTextBox
    {
        #region Constructors and Destructor
        /// <summary>
        /// 
        /// </summary>
        public UIntTextBox()
        {
            MaxValue = UInt64.MaxValue;
            MinValue = 0;
            MaxIntegerDigits =  MaxLength > 0 ? (uint)MaxLength : 1;
            MaxFractionalDigits = 0;
            AllowLeadingSign = false;
            AllowDecimalPoint = false;

            InitializeComponent();

        }
        #endregion
    }
}
