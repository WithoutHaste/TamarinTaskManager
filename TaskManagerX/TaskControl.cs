using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerX
{
	public partial class TaskControl : UserControl
	{
		public TaskControl()
		{
			InitializeComponent();
		}

		private void TaskControl_Load(object sender, EventArgs e)
		{
			this.Anchor = AnchorStyles.Left;
			this.Dock = DockStyle.Top;
		}
	}
}
