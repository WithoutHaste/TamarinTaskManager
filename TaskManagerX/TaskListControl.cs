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
	public partial class TaskListControl : UserControl
	{
		public TaskListControl()
		{
			InitializeComponent();

			this.titlePanel.Controls.Add(new TaskTitleControl());

			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 0) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 35) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 70) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 105) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 140) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 175) });
			this.scrollPanel.Controls.Add(new TaskControl() { Location = new Point(0, 210) });
		}

		private void TaskListControl_Load(object sender, EventArgs e)
		{
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Top;
		}
	}
}
