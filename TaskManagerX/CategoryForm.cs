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
	public partial class CategoryForm : Form
	{
		public CategoryForm(string[] categories)
		{
			InitializeComponent();

			categoriesTextBox.Text = String.Join("\n", categories);
		}

		public string[] GetCategories()
		{
			return categoriesTextBox.Text.Split('\n');
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if(openDialog.ShowDialog() == DialogResult.OK)
			{
				string fullPath = openDialog.FileName;
				if(!File.Exists(fullPath))
				{
					MessageBox.Show(String.Format("File does not exist: {0}", fullPath), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				using(Project fromProject = new Project(fullPath))
				{
					categoriesTextBox.Text = String.Join("\n", fromProject.Categories);
				}
			}
		}
	}
}
