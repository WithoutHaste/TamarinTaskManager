using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
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

		private TaskTableControl SelectedTaskTableControl {
			get {
				TabPage tabPage = this.tabControl.Controls[SelectedIndex] as TabPage;
				foreach(Control control in tabPage.Controls)
				{
					if(control is TaskTableControl)
						return (control as TaskTableControl);
				}
				throw new Exception("No taskTableControl found.");
			}
		}

		//-------------------------------------------------

		public WindowForm()
		{
			InitializeComponent();
			this.Text = "Tamarin v" + Utilities.GetSemanticVersion(Assembly.GetExecutingAssembly());
			this.MinimumSize = new Size(800, 300);
			try
			{
				Assembly myAssembly = Assembly.GetAssembly(typeof(WindowForm));
				Stream iconStream = myAssembly.GetManifestResourceStream("Tamarin.icon.ico");
				this.Icon = new Icon(iconStream);
			}
			catch(Exception)
			{
			}
			this.Activated += new EventHandler(windowForm_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(windowForm_Closing);

			try
			{
				if(Properties.Settings.Default.WindowMaximized)
				{
					this.WindowState = FormWindowState.Maximized;
				}
				else
				{
					this.Size = new Size(Properties.Settings.Default.WindowWidth, Properties.Settings.Default.WindowHeight);
				}
			}
			catch
			{
				this.Size = new Size(600, 1000);
			}

			InitProjects();
		}

		//-------------------------------------------------

		protected override bool ProcessCmdKey(ref Message message, Keys keys)
		{
			switch(keys)
			{
				case Keys.Control | Keys.A:
					SelectedTaskTableControl.ToolStrip.SelectActiveInactive(active: true);
					return true;
				case Keys.Control | Keys.I:
					SelectedTaskTableControl.ToolStrip.SelectActiveInactive(active: false);
					return true;
				case Keys.Control | Keys.S:
					saveButton_Click(new object(), new EventArgs());
					return true;
				case Keys.Control | Keys.Y:
					redoButton_Click(new object(), new EventArgs());
					return true;
				case Keys.Control | Keys.Z:
					undoButton_Click(new object(), new EventArgs());
					return true;
			}
			return base.ProcessCmdKey(ref message, keys);
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
			openDialog.Filter = "MS Excel 2003 XML|*.xml|Excel|*.xlsx";
			if(openDialog.ShowDialog() == DialogResult.OK)
			{
				LoadProject(openDialog.FileName);
			}
		}

		private void settingsButton_Click(object sender, EventArgs e)
		{
			using(SettingsForm settingsForm = new SettingsForm())
			{
				DialogResult result = settingsForm.ShowDialog();
				if(result != DialogResult.OK)
					return;
				foreach(TabPage tabPage in this.tabControl.Controls)
				{
					foreach(Control control in tabPage.Controls)
					{
						if(!(control is TaskTableControl))
							continue;
						(control as TaskTableControl).ShowHideTaskIds();
					}
				}
			}
		}

		private void windowForm_Activated(object sender, EventArgs e)
		{
			SelectedTaskTableControl.CheckForOutsideEdits();
		}

		private void windowForm_Closing(object sender, FormClosingEventArgs e)
		{
			string openFilenames = String.Join(";", projects.Where(x => !x.NotNamed).Select(x => x.FullPath).ToArray());
			Properties.Settings.Default.OpenFilenames = openFilenames;
			Properties.Settings.Default.WindowMaximized = (this.WindowState == FormWindowState.Maximized);
			Properties.Settings.Default.WindowWidth = this.Width;
			Properties.Settings.Default.WindowHeight = this.Height;
			Properties.Settings.Default.Save();

			try
			{
				while(this.tabControl.TabPages.Count > 0)
				{
					CloseProject();
				}
			}
			catch(CancelException)
			{
				e.Cancel = true; //don't close window
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
					try
					{
						CloseProject();
					}
					catch(CancelException)
					{
						//no action
					}
					return;
				}
			}
		}

		private void InitProjects()
		{
			projects.Clear(); //do i need to close files here?
			this.tabControl.Controls.Clear();

			if(OpenPreviousFiles())
				return;

			NewProject();
		}

		private bool OpenPreviousFiles()
		{
			string setting = "";
			try
			{
				setting = Properties.Settings.Default.OpenFilenames;
			}
			catch
			{
			}
			if(String.IsNullOrEmpty(setting))
				return false;

			string[] filenames = setting.Split(';');
			foreach(string filename in filenames)
			{
				LoadProject(filename);
			}

			this.tabControl.SelectTab(0);

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
			tabPage.Location = new System.Drawing.Point(0, 0);
			tabPage.Padding = new System.Windows.Forms.Padding(0);
			tabPage.Size = new System.Drawing.Size(276, 195);
			tabPage.TabIndex = tabCount;
			tabPage.Text = FormatTabName(tabName);
			tabPage.UseVisualStyleBackColor = true;
			this.tabControl.Controls.Add(tabPage);
			this.tabControl.SelectedIndex = this.tabControl.Controls.Count - 1;

			TaskTableControl taskTableControl = new TaskTableControl(SelectedProject);
			TaskTableToolStrip taskTableToolStrip = new TaskTableToolStrip(taskTableControl);
			tabPage.Controls.Add(taskTableControl);
			tabPage.Controls.Add(taskTableToolStrip);
		}

		private void SaveAsProject()
		{
			SaveFileDialog saveAsDialog = new SaveFileDialog();
			saveAsDialog.Filter = "MS Excel 2003 XML|*.xml";
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
			if(SelectedProject.NotNamed || SelectedProject.EditedSinceLastSave)
			{
				DialogResult result = MessageBox.Show("Save " + SelectedProject.Name + " before closing?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Cancel)
				{
					throw new CancelException();
				}
				if(result == DialogResult.Yes)
				{
					if(SelectedProject.NotNamed)
						SaveAsProject();
					else
						SaveProject();
				}
			}
			int selectedIndex = SelectedIndex;
			this.tabControl.TabPages.RemoveAt(selectedIndex);
			projects.RemoveAt(selectedIndex);
		}

		private void undoButton_Click(object sender, EventArgs e)
		{
			SelectedTaskTableControl.Undo();
		}

		private void redoButton_Click(object sender, EventArgs e)
		{
			SelectedTaskTableControl.Redo();
		}
	}
}
