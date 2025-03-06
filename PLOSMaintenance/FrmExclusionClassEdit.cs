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
	public partial class FrmExclusionClassEdit : Form
	{
		public FrmExclusionClassEdit()
		{
			InitializeComponent();
		}

		private void OnOKClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			Close();
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;

			Close();
		}

		public String ExclusionClass
		{
			get
			{
				return txtExclusionClass.Text;
			}
			set
			{
				txtExclusionClass.Text = value;
			}
		}
	}
}
