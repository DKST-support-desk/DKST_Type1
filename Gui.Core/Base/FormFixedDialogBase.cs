using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormFixedDialogBase : FormBase
    {
		public enum EditMode
		{
			Create,
			Edit,
			View,
		}

        public FormFixedDialogBase()
        {
            InitializeComponent();
        }
    }
}
