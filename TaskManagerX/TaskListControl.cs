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

//			this.Controls.Add(new TaskControl());
			this.Controls.Add(new TaskControl());
		}

		private void TaskListControl_Load(object sender, EventArgs e)
		{
			this.Dock = DockStyle.Fill;
		}
	}
}
