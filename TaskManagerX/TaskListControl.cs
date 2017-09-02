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
		public TaskListControl(Project project)
		{
			InitializeComponent();

			this.titlePanel.Controls.Add(new TaskTitleControl());
			AddEmptyTaskControl(y: 0);
			/*
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 0) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 35) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 70) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 105) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 140) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 175) });
			this.scrollPanel.Controls.Add(new TaskControl(project.Statuses, project.Categories) { Location = new Point(0, 210) });
			*/
		}

		private void TaskListControl_Load(object sender, EventArgs e)
		{
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Top;
		}

		private void AddEmptyTaskControl(int y)
		{
			TaskControl control = NewTaskControl(y);
			control.ShowOnlyAddButton();
			control.RowNumber = 0;
			this.scrollPanel.Controls.Add(control);
		}

		private TaskControl NewTaskControl(int y)
		{
			TaskControl control = new TaskControl() { Location = new Point(0, y) };
			control.SetAddEventHandler(new System.EventHandler(addButton_Click));
			return control;
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			Control parentControl = (sender as Control).Parent;
			while(!(parentControl is TaskControl))
			{
				parentControl = parentControl.Parent;
			}
			TaskControl control = NewTaskControl(parentControl.Location.Y + parentControl.Size.Height);
			control.RowNumber = (parentControl as TaskControl).RowNumber + 1;
			foreach(TaskControl lowerControl in this.scrollPanel.Controls)
			{
				if(lowerControl.RowNumber < control.RowNumber)
					continue;
				lowerControl.RowNumber++;
				lowerControl.Location = new Point(lowerControl.Location.X, lowerControl.Location.Y + lowerControl.Size.Height);
			}
			this.scrollPanel.Controls.Add(control);
		}
	}
}
