using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerX
{
	public partial class WindowForm : Form
	{
		private List<Project> projects = new List<Project>();

		private int SelectedIndex {
			get {
				return this.tabControl.SelectedIndex;
			}
		}
		private Project SelectedProject {
			get {
				return projects[SelectedIndex];
			}
		}

		public WindowForm()
		{
			InitializeComponent();
			this.MinimumSize = new Size(650, 300);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(windowForm_Closing);

			InitProjects();
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			NewProject();
		}

		private void saveAsButton_Click(object sender, EventArgs e)
		{
			SaveAsProject();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			if (SelectedProject.NotNamed)
			{
				SaveAsProject();
				return;
			}
			SaveProject();
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if(openDialog.ShowDialog() == DialogResult.OK)
			{
				LoadProject(openDialog.FileName);
			}
		}

		private void windowForm_Closing(object sender, FormClosingEventArgs e)
		{
			string openFilenames = String.Join(";", projects.Where(x => !x.NotNamed).Select(x => x.FullPath).ToArray());
			Properties.Settings.Default.OpenFilenames = openFilenames;
			Properties.Settings.Default.Save();

			foreach(Project project in projects)
			{
				project.Dispose();
			}
		}

		private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.Graphics.DrawString("x", e.Font, Brushes.DarkRed, e.Bounds.Right - 15, e.Bounds.Top + 4);
			e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
			e.DrawFocusRectangle();
		}

		private void tabControl_MouseUp(object sender, MouseEventArgs e)
		{
			for(int i = 0; i < this.tabControl.TabPages.Count; i++)
			{
				Rectangle r = tabControl.GetTabRect(i);
				//Getting the position of the "x" mark.
				Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
				if(closeButton.Contains(e.Location))
				{
					this.tabControl.SelectedIndex = i;
					CloseProject();
					return;
				}
			}
		}

		private void InitProjects()
		{
			projects.Clear(); //do i need to close files here?
			this.tabControl.Controls.Clear();

			//reopen the files that were open the last time the app closed
			if(OpenPreviousFiles())
				return;

			//otherwise, open a new project
			NewProject();
		}

		private bool OpenPreviousFiles()
		{
			string setting = Properties.Settings.Default.OpenFilenames;
			if(String.IsNullOrEmpty(setting))
				return false;

			string[] filenames = setting.Split(';');
			foreach(string filename in filenames)
			{
				LoadProject(filename);
			}
			return true;
		}

		private void NewProject()
		{
			projects.Add(new Project());
			NewTab();
		}

		private void NewTab(string tabName = "New")
		{
			int tabCount = this.tabControl.Controls.Count + 1;
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
			tabPage.Location = new System.Drawing.Point(4, 22);
			tabPage.Padding = new System.Windows.Forms.Padding(3);
			tabPage.Size = new System.Drawing.Size(276, 195);
			tabPage.TabIndex = tabCount;
			tabPage.Text = FormatTabName(tabName);
			tabPage.UseVisualStyleBackColor = true;
			tabPage.AutoScroll = true;
			this.tabControl.Controls.Add(tabPage);
			this.tabControl.SelectedIndex = this.tabControl.Controls.Count - 1;
			this.tabControl.SelectedTab.Controls.Add(new TaskListControl());
		}

		private void SaveAsProject()
		{
			SaveFileDialog saveAsDialog = new SaveFileDialog();
			saveAsDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if (saveAsDialog.ShowDialog() == DialogResult.OK)
			{
				SelectedProject.FullPath = saveAsDialog.FileName;
				this.tabControl.SelectedTab.Text = FormatTabName(SelectedProject.Name);
				SaveProject();
			}
		}

		private string FormatTabName(string name)
		{
			return name + "        ";
		}

		private void SaveProject()
		{
			SelectedProject.Save();
		}

		private void LoadProject(string fullPath)
		{
			if(!File.Exists(fullPath))
			{
				MessageBox.Show(String.Format("File does not exist: {0}", fullPath), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			projects.Add(new Project(fullPath));
			NewTab(projects.Last().Name);
		}

		private void CloseProject()
		{
			if(SelectedProject.NotNamed) //todo: update to not saved since last edit
			{
				DialogResult result = MessageBox.Show("Save before closing?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Cancel)
					return;
				if(result == DialogResult.Yes)
					SaveAsProject();
			}
			int selectedIndex = SelectedIndex;
			this.tabControl.TabPages.RemoveAt(selectedIndex);
			projects.RemoveAt(selectedIndex);
		}
	}
}
