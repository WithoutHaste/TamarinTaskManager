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
	public class WindowForm : Form
	{
		private ToolStrip toolStrip;
		private ToolStripButton newButton;
		private ToolStripButton saveButton;
		private ToolStripButton loadButton;
		private TabPage tabPage1;
		private TabControl tabControl;
		private ToolStripButton saveAsButton;
		private ToolStripButton settingsButton;
		private ToolStripButton undoButton;
		private ToolStripButton redoButton;

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
				if(SelectedIndex == -1)
					return null;
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
			this.Activated += new EventHandler(OnActivated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnClosing);

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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowForm));
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.newButton = new System.Windows.Forms.ToolStripButton();
			this.saveAsButton = new System.Windows.Forms.ToolStripButton();
			this.saveButton = new System.Windows.Forms.ToolStripButton();
			this.loadButton = new System.Windows.Forms.ToolStripButton();
			this.settingsButton = new System.Windows.Forms.ToolStripButton();
			this.undoButton = new System.Windows.Forms.ToolStripButton();
			this.redoButton = new System.Windows.Forms.ToolStripButton();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.toolStrip.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.newButton,
			this.saveAsButton,
			this.saveButton,
			this.loadButton,
			this.settingsButton,
			this.undoButton,
			this.redoButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(402, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip1";
			// 
			// newButton
			// 
			this.newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(35, 22);
			this.newButton.Text = "New";
			this.newButton.ToolTipText = "New Project";
			this.newButton.Click += new System.EventHandler(this.OnNewButtonClick);
			// 
			// saveAsButton
			// 
			this.saveAsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.saveAsButton.Name = "saveAsButton";
			this.saveAsButton.Size = new System.Drawing.Size(51, 22);
			this.saveAsButton.Text = "Save As";
			this.saveAsButton.ToolTipText = "Save Project As";
			this.saveAsButton.Click += new System.EventHandler(this.OnSaveAsButtonClick);
			// 
			// saveButton
			// 
			this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(35, 22);
			this.saveButton.Text = "Save";
			this.saveButton.ToolTipText = "Save Project";
			this.saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
			// 
			// loadButton
			// 
			this.loadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(37, 22);
			this.loadButton.Text = "Load";
			this.loadButton.ToolTipText = "Load Project";
			this.loadButton.Click += new System.EventHandler(this.OnLoadButtonClick);
			// 
			// settingsButton
			// 
			this.settingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.settingsButton.Name = "settingsButton";
			this.settingsButton.Size = new System.Drawing.Size(53, 22);
			this.settingsButton.Text = "Settings";
			this.settingsButton.Click += new System.EventHandler(this.OnSettingsButtonClick);
			// 
			// undoButton
			// 
			this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.undoButton.Name = "undoButton";
			this.undoButton.Size = new System.Drawing.Size(40, 22);
			this.undoButton.Text = "Undo";
			this.undoButton.Click += new System.EventHandler(this.OnUndoButtonClick);
			// 
			// redoButton
			// 
			this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.redoButton.Name = "redoButton";
			this.redoButton.Size = new System.Drawing.Size(38, 22);
			this.redoButton.Text = "Redo";
			this.redoButton.Click += new System.EventHandler(this.OnRedoButtonClick);
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(394, 207);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tabControl.Location = new System.Drawing.Point(0, 28);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(402, 233);
			this.tabControl.TabIndex = 1;
			this.tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnTabControlDrawItem);
			this.tabControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnTabControlMouseUp);
			// 
			// WindowForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(402, 261);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.toolStrip);
			this.Name = "WindowForm";
			this.Text = "Tamarin";
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		//-------------------------------------------------

		#region Event Handlers

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
					OnSaveButtonClick(new object(), new EventArgs());
					return true;
				case Keys.Control | Keys.Y:
					OnRedoButtonClick(new object(), new EventArgs());
					return true;
				case Keys.Control | Keys.Z:
					OnUndoButtonClick(new object(), new EventArgs());
					return true;
			}
			return base.ProcessCmdKey(ref message, keys);
		}

		private void OnNewButtonClick(object sender, EventArgs e)
		{
			NewProject();
		}

		private void OnSaveAsButtonClick(object sender, EventArgs e)
		{
			SaveAsProject();
		}

		private void OnSaveButtonClick(object sender, EventArgs e)
		{
			if (SelectedProject.NotNamed)
			{
				SaveAsProject();
				return;
			}
			SaveProject();
		}

		private void OnLoadButtonClick(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "MS Excel 2003 XML|*.xml|Excel|*.xlsx";
			if(openDialog.ShowDialog() == DialogResult.OK)
			{
				LoadProject(openDialog.FileName);
			}
		}

		private void OnSettingsButtonClick(object sender, EventArgs e)
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

		private void OnActivated(object sender, EventArgs e)
		{
			SelectedTaskTableControl?.CheckForOutsideEdits();
		}

		private void OnClosing(object sender, FormClosingEventArgs e)
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

		private void OnTabControlDrawItem(object sender, DrawItemEventArgs e)
		{
			e.Graphics.DrawString("x", e.Font, Brushes.DarkRed, e.Bounds.Right - 15, e.Bounds.Top + 4);
			e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
			e.DrawFocusRectangle();
		}

		private void OnTabControlMouseUp(object sender, MouseEventArgs e)
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

		private void OnUndoButtonClick(object sender, EventArgs e)
		{
			SelectedTaskTableControl.Undo();
		}

		private void OnRedoButtonClick(object sender, EventArgs e)
		{
			SelectedTaskTableControl.Redo();
		}

		#endregion

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
	}
}
