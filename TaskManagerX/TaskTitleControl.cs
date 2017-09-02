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
	public partial class TaskTitleControl : UserControl
	{
		public TaskTitleControl()
		{
			InitializeComponent();
		}

		private void TaskTitleControl_Load(object sender, EventArgs e)
		{
			this.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		}
	}
}
