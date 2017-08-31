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
using OfficeOpenXml;

namespace TaskManagerX
{
	public partial class WindowForm : Form
	{
		public WindowForm()
		{
			InitializeComponent();

			InitProjects();
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			NewProject();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			SaveAsProject();
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			LoadProject();
		}

		private void InitProjects()
		{
			//if projects were open last time, reopen them
			//if not, open a single new project
		}

		private void NewProject()
		{
		}

		private void SaveAsProject()
		{
			SaveFileDialog saveAsDialog = new SaveFileDialog();
			saveAsDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if (saveAsDialog.ShowDialog() == DialogResult.OK)
			{
				SaveProject(saveAsDialog.FileName);
			}
		}

		private void SaveProject(string filename)
		{
			FileInfo file = new FileInfo(filename);
			using (ExcelPackage package = new ExcelPackage(file))
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Default - " + DateTime.Now.ToShortDateString());

				package.Save();
			}
		}

		private void LoadProject()
		{
		}
	}
}
