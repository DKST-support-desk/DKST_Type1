using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBConnect;

namespace PLOSMaintenance
{
	public partial class FrmDataCollectionSetting : Form
	{
		private UCDataCollectionSetting ucDataCollectionSetting;
		private Guid CompositionId { get; set; } = Guid.Empty;

		public FrmDataCollectionSetting()
		{
			InitializeComponent();
		}

		public FrmDataCollectionSetting(Guid compositionId)
		{
			this.CompositionId = compositionId;

			this.ucDataCollectionSetting = new PLOSMaintenance.UCDataCollectionSetting(compositionId);
			this.SuspendLayout();
			// 
			// ucDataCollectionSetting
			// 
			this.ucDataCollectionSetting.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ucDataCollectionSetting.Name = "ucDataCollectionSetting";
			//this.ucDataCollectionSetting.Size = new System.Drawing.Size(1266, 737);
			this.ucDataCollectionSetting.TabIndex = 0;

			this.Controls.Add(this.ucDataCollectionSetting);

			this.ResumeLayout(false);
		}
	}
}
