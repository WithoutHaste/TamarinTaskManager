using System;
using System.Collections.Generic;
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
			LoadProject();
		}

		private void InitProjects()
		{
			//if projects were open last time, reopen them
			//if not, open a single new project

			projects.Clear(); //do i need to close files here?
			this.tabControl.Controls.Clear();
			NewProject();
		}

		private void NewProject()
		{
			projects.Add(new Project());
			NewTab();
		}

		private void NewTab()
		{
			int tabCount = this.tabControl.Controls.Count + 1;
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
			tabPage.Location = new System.Drawing.Point(4, 22);
			tabPage.Padding = new System.Windows.Forms.Padding(3);
			tabPage.Size = new System.Drawing.Size(276, 195);
			tabPage.TabIndex = tabCount;
			tabPage.Text = "New "+tabCount;
			tabPage.UseVisualStyleBackColor = true;
			this.tabControl.Controls.Add(tabPage);
			this.tabControl.SelectedIndex = this.tabControl.Controls.Count - 1;
		}

		private void SaveAsProject()
		{
			SaveFileDialog saveAsDialog = new SaveFileDialog();
			saveAsDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if (saveAsDialog.ShowDialog() == DialogResult.OK)
			{
				SelectedProject.PathAndFilename = saveAsDialog.FileName;
				this.tabControl.SelectedTab.Text = SelectedProject.Name;
				SaveProject();
			}
		}

		private void SaveProject()
		{
			SelectedProject.Save();
		}

		private void LoadProject()
		{
		}
	}
}
